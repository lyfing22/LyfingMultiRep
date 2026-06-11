using Dapper;
using DataMigrate.Infrastructure;
using DataMigrate.Models;
using DataMigrate.Upload;
using DataMigrate.Validation;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DataMigrate.Core;

public class MigrationRunner
{
    private readonly IMigrationSource _source;
    private readonly MigrationOptions _options;
    private readonly IServiceProvider _sp;
    private readonly ArchiveUploader _uploader;
    private readonly PostMigrationAuditor _auditor;
    private readonly MigrationStatistics _stats;
    private readonly ConsoleProgressBar _progress;
    private readonly ILogger<MigrationRunner> _logger;
    private readonly ILoggerFactory _loggerFactory;

    public MigrationRunner(
        IMigrationSource source,
        MigrationOptions options,
        IServiceProvider sp,
        ArchiveUploader uploader,
        PostMigrationAuditor auditor,
        MigrationStatistics statistics,
        ConsoleProgressBar progress,
        ILogger<MigrationRunner> logger,
        ILoggerFactory loggerFactory)
    {
        _source = source;
        _options = options;
        _sp = sp;
        _uploader = uploader;
        _auditor = auditor;
        _stats = statistics;
        _progress = progress;
        _logger = logger;
        _loggerFactory = loggerFactory;
    }

    public async Task RunAsync()
    {
        await _source.ValidateAsync();

        switch (_options.Migration.Mode.ToLowerInvariant())
        {
            case "debug":
                await RunDebugAsync();
                break;
            case "validate":
                await RunValidateAsync();
                break;
            case "audit":
                await RunAuditAsync();
                break;
            default:
                await RunBatchAsync();
                break;
        }
    }

    // ───────── Debug 模式 ─────────
    // 两种子模式：
    //   1) 配置了 AccessionNumber → 单条查询 + 打印 JSON + 上传（原行为）
    //   2) 未配置 AccessionNumber → 使用配置的 TimeRange，按 PlanIntervalDays 切割计划后执行（预演）

    private async Task RunDebugAsync()
    {
        var accNum = _options.Migration.AccessionNumber;
        if (!string.IsNullOrWhiteSpace(accNum))
        {
            await RunDebugSingleAsync(accNum);
            return;
        }

        var cfg = _options.Migration.TimeRange;
        if (cfg.Start == default || cfg.End == default)
        {
            _logger.LogWarning("debug 模式需配置 AccessionNumber 或 TimeRange");
            return;
        }

        var range = new DateRange(cfg.Start, cfg.End);
        _logger.LogInformation("调试模式(时间区间): {Start:yyyy-MM-dd HH:mm} ~ {End:yyyy-MM-dd HH:mm}, 将按计划切割迁移",
            range.Start, range.End);
        await RunPlanBasedMigrationAsync(range);
    }

    private async Task RunDebugSingleAsync(string accNum)
    {
        _logger.LogInformation("调试模式(单条): {Acc}", accNum);
        var archive = await _source.GetByAccessionNumberAsync(accNum);
        if (archive == null)
        {
            _logger.LogWarning("未找到检查号: {Acc}", accNum);
            return;
        }

        Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(archive, ArchiveJson.PrettyPrint));

        var result = await _uploader.UploadAsync(archive, CancellationToken.None);
        _logger.LogInformation(result.Success ? "上传成功" : "上传失败: {Msg}", result.ErrorMessage);
        await _uploader.FlushRemainingAsync();
    }

    // ───────── Batch 模式 ─────────

    private async Task RunBatchAsync()
    {
        var range = await _source.GetTimeRangeAsync();
        _logger.LogInformation("批量迁移: 源库时间范围 {Start:yyyy-MM-dd HH:mm} ~ {End:yyyy-MM-dd HH:mm}",
            range.Start, range.End);
        await RunPlanBasedMigrationAsync(range);
    }

    // ───────── Validate 模式 ─────────

    private async Task RunValidateAsync()
    {
        var range = await _source.GetTimeRangeAsync();
        _logger.LogInformation("验证模式: 源库时间范围 {Start:yyyy-MM-dd HH:mm} ~ {End:yyyy-MM-dd HH:mm}",
            range.Start, range.End);

        var mongoConnStr = _options.ConnectionStrings.MongoDB;
        var verifier = new MongoVerifier(mongoConnStr, _loggerFactory.CreateLogger<MongoVerifier>());
        var accNums = new List<string>();

        _logger.LogInformation("拉取检查号列表...");
        await foreach (var archive in _source.EnumerateArchivesAsync(range, 1000, CancellationToken.None))
            accNums.Add(archive.Order.AccessionNumber);

        _logger.LogInformation("共 {Count} 条, 开始 MongoDB 验证", accNums.Count);
        await verifier.VerifyAsync(accNums);
    }

    // ───────── Audit 模式 ─────────

    private async Task RunAuditAsync()
    {
        var range = await _source.GetTimeRangeAsync();
        _logger.LogInformation("审计模式: 源库时间范围 {Start:yyyy-MM-dd HH:mm} ~ {End:yyyy-MM-dd HH:mm}",
            range.Start, range.End);
        await _auditor.AuditAsync(range);
    }

    // ───────── 分计划迁移核心逻辑 ─────────

    private async Task RunPlanBasedMigrationAsync(DateRange fullRange)
    {
        var plans = SplitRange(fullRange, _options.Migration.PlanIntervalDays);
        _logger.LogInformation("计划切割: 共 {Count} 个子计划, 间隔 {IntervalDays} 天",
            plans.Count, _options.Migration.PlanIntervalDays);

        // 1. 批量写入 ZTemp_MigratePlan（status = Pending）
        await BatchCreatePlanRecordsAsync(plans);

        // 2. 设置全局统计总数
        var totalMeta = await _source.GetMetadataAsync(fullRange);
        _stats.TotalRecords = totalMeta.EstimatedCount;
        _logger.LogInformation("全局预估记录数: {Count}", _stats.TotalRecords);

        // 3. 按配置排序
        List<PlanInfo> sortedPlans = _options.Migration.PlanOrder.Equals("ascending", StringComparison.OrdinalIgnoreCase)
            ? [.. plans.OrderBy(p => p.Range.Start)]
            : [.. plans.OrderByDescending(p => p.Range.Start)];

        // 4. 启动全局进度与计时
        _stats.Start();
        _progress.Start();

        // 5. 并发执行子计划
        var parallelOpts = new ParallelOptions
        {
            MaxDegreeOfParallelism = _options.Migration.MaxParallelPlans
        };

        await Parallel.ForEachAsync(sortedPlans, parallelOpts, async (plan, ct) =>
        {
            var engine = ActivatorUtilities.CreateInstance<MigrationEngine>(
                _sp, _source, plan.Id);
            _logger.LogInformation("启动计划: {PlanId}, 范围 {Start:yyyy-MM-dd HH:mm} ~ {End:yyyy-MM-dd HH:mm}",
                plan.Id, plan.Range.Start, plan.Range.End);
            await engine.RunAsync(plan.Range, ct);
        });

        // 6. 停止全局进度
        _progress.Stop();
        _stats.Stop();

        // 7. 审计与汇总
        await _auditor.AuditAsync(fullRange);
        _stats.PrintSummary();
    }

    // ───────── 工具方法 ─────────

    /// <summary>
    /// 将完整时间范围按 intervalDays 切割为半开区间 [start, end) 的子计划。
    /// 最后一个子计划的 end 略微延伸（+1秒）以确保包含源库最大时间点的数据。
    /// </summary>
    internal static List<PlanInfo> SplitRange(DateRange full, int intervalDays)
    {
        var plans = new List<PlanInfo>();
        var cur = full.Start;
        var lastEnd = full.End.AddSeconds(1);

        while (cur < full.End)
        {
            var candidateEnd = cur.AddDays(intervalDays);
            var actualEnd = candidateEnd >= full.End ? lastEnd : candidateEnd;

            plans.Add(new PlanInfo(Guid.NewGuid(), new DateRange(cur, actualEnd)));
            cur = actualEnd;
        }

        return plans;
    }

    /// <summary>批量写入 ZTemp_MigratePlan 记录，初始 status = Pending</summary>
    private async Task BatchCreatePlanRecordsAsync(List<PlanInfo> plans)
    {
        try
        {
            using var conn = new SqlConnection(_options.ConnectionStrings.Destination);
            await conn.OpenAsync();
            using var tx = conn.BeginTransaction();

            foreach (var plan in plans)
            {
                await conn.ExecuteAsync(@"
INSERT INTO ZTemp_MigratePlan(Id, DbFlag, TimeRangeStart, TimeRangeEnd, TotalRecords, SuccessCount, FailedCount, Status, CreatedAt)
VALUES (@Id, @DbFlag, @Start, @End, 0, 0, 0, 'Pending', GETDATE())",
                    new
                    {
                        Id = plan.Id,
                        DbFlag = _options.Migration.DbFlag,
                        Start = plan.Range.Start,
                        End = plan.Range.End
                    }, tx);
            }

            tx.Commit();
            _logger.LogInformation("已写入 {Count} 条计划记录", plans.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "批量写入计划表失败");
            throw;
        }
    }
}
