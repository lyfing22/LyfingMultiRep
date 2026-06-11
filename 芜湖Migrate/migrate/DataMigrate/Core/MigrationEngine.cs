using System.Threading.Channels;
using Dapper;
using DataMigrate.Infrastructure;
using DataMigrate.Models;
using DataMigrate.Upload;
using DataMigrate.Validation;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace DataMigrate.Core;

/// <summary>
/// Producer-Consumer 迁移引擎：
/// 1 个生产者分页查 Oracle → Channel&lt;MigrateArchive&gt; 有界队列 → N 个消费者并行 HTTP 上传
/// </summary>
public class MigrationEngine
{
    private readonly IMigrationSource _source;
    private readonly ArchiveUploader _uploader;
    private readonly PostMigrationAuditor _auditor;
    private readonly MigrationStatistics _stats;
    private readonly ConsoleProgressBar _progress;
    private readonly MigrationOptions _options;
    private readonly ILogger<MigrationEngine> _logger;
    private readonly CancellationTokenSource _cts = new();
    private readonly Channel<MigrateArchive> _channel;
    private readonly int _parallelism;

    public MigrationEngine(
        IMigrationSource source,
        ArchiveUploader uploader,
        PostMigrationAuditor auditor,
        MigrationStatistics statistics,
        ConsoleProgressBar progress,
        MigrationOptions options,
        ILogger<MigrationEngine> logger)
    {
        _source = source;
        _uploader = uploader;
        _auditor = auditor;
        _stats = statistics;
        _progress = progress;
        _options = options;
        _logger = logger;
        _parallelism = options.Migration.Parallelism > 0 ? options.Migration.Parallelism : 4;
        // 有界 Channel：容量 = PageSize × 2，生产者写满时自动等待（背压）
        _channel = Channel.CreateBounded<MigrateArchive>(new BoundedChannelOptions(options.Migration.PageSize * 2)
        {
            FullMode = BoundedChannelFullMode.Wait
        });
    }

    /// <summary>启动迁移：Produce → Consume → 刷缓冲 → 审计</summary>
    public async Task RunAsync(DateRange range, CancellationToken ct)
    {
        // 注册 Ctrl+C 信号处理，触发优雅退出
        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            _cts.Cancel();
            _logger.LogWarning("收到终止信号，正在优雅退出...");
        };

        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, _cts.Token);
        var token = linkedCts.Token;
        var planId = Guid.NewGuid();

        _logger.LogInformation("迁移启动: Source={Source}, Range={Start} ~ {End}",
            _source.SourceName, range.Start.ToString("yyyy-MM-dd HH:mm"), range.End.ToString("yyyy-MM-dd HH:mm"));

        try
        {
            var meta = await _source.GetMetadataAsync(range);
            if (meta.EstimatedCount == 0)
            {
                _logger.LogWarning("时间范围内无数据");
                return;
            }

            _stats.TotalRecords = meta.EstimatedCount;
            _logger.LogInformation("预估记录数: {Count}", meta.EstimatedCount);

            await InitPlanRecordAsync(planId, range);
            _stats.Start();
            _progress.Start();

            // 启动 1 个生产者 + N 个消费者
            var producer = ProduceAsync(range, token);
            var consumers = new Task[_parallelism];
            for (int i = 0; i < _parallelism; i++)
            {
                var id = i;
                consumers[i] = ConsumeAsync(id, token);
            }

            // 先等生产者完成，再通知消费者结束，最后等所有消费者完成
            await producer;
            _channel.Writer.TryComplete();
            await Task.WhenAll(consumers);

            _progress.Stop();
            _stats.Stop();
            await _uploader.FlushRemainingAsync();

            await CompletePlanRecordAsync(planId, range);
            await _auditor.AuditAsync(range);
            _stats.PrintSummary();
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("迁移被取消");
            _channel.Writer.TryComplete();
            _progress.Stop();
            _stats.Stop();
            await _uploader.FlushRemainingAsync();
            await FailPlanRecordAsync(planId, "用户取消");
            _stats.PrintSummary();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "迁移异常终止");
            _channel.Writer.TryComplete();
            _progress.Stop();
            _stats.Stop();
            await _uploader.FlushRemainingAsync();
            await FailPlanRecordAsync(planId, ex.Message);
            _stats.PrintSummary();
            throw;
        }
    }

    /// <summary>生产者：从数据源分页读取，逐条写入 Channel</summary>
    private async Task ProduceAsync(DateRange range, CancellationToken ct)
    {
        await foreach (var archive in _source.EnumerateArchivesAsync(range, _options.Migration.PageSize, ct))
        {
            // 如果 Channel 满，WriteAsync 会自动等待（背压）
            await _channel.Writer.WriteAsync(archive, ct);
        }
    }

    /// <summary>消费者：从 Channel 读取，逐条 HTTP 上传</summary>
    private async Task ConsumeAsync(int id, CancellationToken ct)
    {
        await foreach (var archive in _channel.Reader.ReadAllAsync(ct))
        {
            var result = await _uploader.UploadAsync(archive, ct);
            if (result.Success)
            {
                _stats.IncrementSucceeded();
                _logger.LogDebug("Consumer#{Id} 成功: {Acc}", id, archive.Order.AccessionNumber);
            }
            else
            {
                _stats.IncrementFailed();
                _logger.LogWarning("Consumer#{Id} 失败: {Acc} => {Msg}",
                    id, archive.Order.AccessionNumber, result.ErrorMessage);
            }

            // 每隔 ZTempBatchSize×2 条更新一次计划表进度
            if (_stats.Processed % (_options.Migration.ZTempBatchSize * 2) == 0)
                await UpdatePlanProgressAsync();
        }
    }

    // ────── ZTemp_MigratePlan 追踪表操作 ──────

    // 注意：以下 4 个方法操作 SQL Server 的 ZTemp_MigratePlan 表，
    // 目的不是在当前进程使用，而是让外部审计人员通过查表了解迁移进度和结果

    private async Task InitPlanRecordAsync(Guid planId, DateRange range)
    {
        try
        {
            using var conn = new SqlConnection(_options.ConnectionStrings.Destination);
            await conn.ExecuteAsync(@"
INSERT INTO ZTemp_MigratePlan(Id, DbFlag, TimeRangeStart, TimeRangeEnd, TotalRecords, SuccessCount, FailedCount, Status, CreatedAt)
VALUES (@Id, @DbFlag, @Start, @End, @Total, 0, 0, 'Running', GETDATE())",
                new { Id = planId, DbFlag = _options.Migration.DbFlag, Start = range.Start, End = range.End, Total = _stats.TotalRecords });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "写入计划表失败");
        }
    }

    private async Task UpdatePlanProgressAsync()
    {
        try
        {
            using var conn = new SqlConnection(_options.ConnectionStrings.Destination);
            await conn.ExecuteAsync(@"
UPDATE ZTemp_MigratePlan SET SuccessCount=@Ok, FailedCount=@Fail
WHERE DbFlag=@DbFlag AND Status='Running'",
                new { Ok = _stats.Succeeded, Fail = _stats.Failed, DbFlag = _options.Migration.DbFlag });
        }
        catch { }
    }

    private async Task CompletePlanRecordAsync(Guid planId, DateRange range)
    {
        try
        {
            using var conn = new SqlConnection(_options.ConnectionStrings.Destination);
            await conn.ExecuteAsync(@"
UPDATE ZTemp_MigratePlan
SET Status='Completed', SuccessCount=@Ok, FailedCount=@Fail, CompletedAt=GETDATE(), ErrorSummary=@Summary
WHERE Id=@Id",
                new
                {
                    Id = planId,
                    Ok = _stats.Succeeded,
                    Fail = _stats.Failed,
                    Summary = _stats.Failed > 0
                        ? $"{_stats.Failed} 条失败，请检查 ZTemp_MigrateError"
                        : "全部成功"
                });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "更新计划表状态失败");
        }
    }

    private async Task FailPlanRecordAsync(Guid planId, string reason)
    {
        try
        {
            using var conn = new SqlConnection(_options.ConnectionStrings.Destination);
            await conn.ExecuteAsync(@"
UPDATE ZTemp_MigratePlan
SET Status='Failed', SuccessCount=@Ok, FailedCount=@Fail, CompletedAt=GETDATE(), ErrorSummary=@Reason
WHERE Id=@Id",
                new { Id = planId, Ok = _stats.Succeeded, Fail = _stats.Failed, Reason = reason });
        }
        catch { }
    }
}
