using System.Threading.Channels;
using Dapper;
using DataMigrate.Infrastructure;
using DataMigrate.Models;
using DataMigrate.Upload;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace DataMigrate.Core;

/// <summary>
/// Producer-Consumer 迁移引擎（单个计划执行单元）：
/// 1 个生产者查 Oracle → Channel&lt;MigrateArchive&gt; 有界队列 → N 个消费者并行 HTTP 上传
/// 不再管理全局统计/进度/审计，这些由 MigrationRunner 统一控制。
/// </summary>
public class MigrationEngine
{
    private readonly IMigrationSource _source;
    private readonly ArchiveUploader _uploader;
    private readonly MigrationStatistics _stats;
    private readonly MigrationOptions _options;
    private readonly ILogger<MigrationEngine> _logger;
    private readonly CancellationTokenSource _cts = new();
    private readonly Channel<MigrateArchive> _channel;
    private readonly int _parallelism;
    private readonly Guid _planId;

    public MigrationEngine(
        IMigrationSource source,
        ArchiveUploader uploader,
        MigrationStatistics statistics,
        MigrationOptions options,
        ILogger<MigrationEngine> logger,
        Guid planId)
    {
        _source = source;
        _uploader = uploader;
        _stats = statistics;
        _options = options;
        _logger = logger;
        _planId = planId;
        _parallelism = options.Migration.Parallelism > 0 ? options.Migration.Parallelism : 4;
        _channel = Channel.CreateBounded<MigrateArchive>(new BoundedChannelOptions(options.Migration.ChannelCapacity)
        {
            FullMode = BoundedChannelFullMode.Wait
        });
    }

    /// <summary>执行单个计划：Produce → Consume → 刷缓冲 → 更新计划状态</summary>
    public async Task RunAsync(DateRange range, CancellationToken ct)
    {
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, _cts.Token);
        var token = linkedCts.Token;

        _logger.LogInformation("计划执行: PlanId={PlanId}, Range={Start:yyyy-MM-dd HH:mm} ~ {End:yyyy-MM-dd HH:mm}",
            _planId, range.Start, range.End);

        try
        {
            var meta = await _source.GetMetadataAsync(range);
            if (meta.EstimatedCount == 0)
            {
                _logger.LogInformation("计划范围内无数据");
                await UpdatePlanStatusAsync("Completed", "无数据");
                return;
            }

            await UpdatePlanStatusAsync("Running", null);

            var producer = ProduceAsync(range, token);
            var consumers = new Task[_parallelism];
            for (int i = 0; i < _parallelism; i++)
            {
                var id = i;
                consumers[i] = ConsumeAsync(id, token);
            }

            await producer;
            _channel.Writer.TryComplete();
            await Task.WhenAll(consumers);

            await _uploader.FlushRemainingAsync();
            var summary = _stats.Failed > 0 ? $"{_stats.Failed} 条失败，请检查 ZTemp_MigrateError" : "全部成功";
            await UpdatePlanStatusAsync("Completed", summary);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("计划被取消: {PlanId}", _planId);
            _channel.Writer.TryComplete();
            await _uploader.FlushRemainingAsync();
            await UpdatePlanStatusAsync("Cancelled", "用户取消");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "计划异常终止: {PlanId}", _planId);
            _channel.Writer.TryComplete();
            await _uploader.FlushRemainingAsync();
            await UpdatePlanStatusAsync("Failed", ex.Message);
        }
    }

    private async Task ProduceAsync(DateRange range, CancellationToken ct)
    {
        await foreach (var archive in _source.EnumerateArchivesAsync(range, ct))
        {
            await _channel.Writer.WriteAsync(archive, ct);
        }
    }

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

            if (_stats.Processed % (_options.Migration.ZTempBatchSize * 2) == 0)
                await UpdatePlanProgressAsync();
        }
    }

    // ────── ZTemp_MigratePlan 追踪表操作 ──────

    private async Task UpdatePlanStatusAsync(string status, string? summary)
    {
        try
        {
            using var conn = new SqlConnection(_options.ConnectionStrings.Destination);
            if (status == "Running")
            {
                await conn.ExecuteAsync(@"
UPDATE ZTemp_MigratePlan
SET Status=@Status, StartedAt=GETDATE()
WHERE Id=@Id",
                    new { Id = _planId, Status = status });
            }
            else
            {
                await conn.ExecuteAsync(@"
UPDATE ZTemp_MigratePlan
SET Status=@Status, SuccessCount=@Ok, FailedCount=@Fail,
    CompletedAt=GETDATE(), ErrorSummary=@Summary
WHERE Id=@Id",
                    new { Id = _planId, Status = status, Ok = _stats.Succeeded, Fail = _stats.Failed, Summary = summary });
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "更新计划表状态失败");
        }
    }

    private async Task UpdatePlanProgressAsync()
    {
        try
        {
            using var conn = new SqlConnection(_options.ConnectionStrings.Destination);
            await conn.ExecuteAsync(@"
UPDATE ZTemp_MigratePlan SET SuccessCount=@Ok, FailedCount=@Fail
WHERE Id=@Id",
                new { Id = _planId, Ok = _stats.Succeeded, Fail = _stats.Failed });
        }
        catch { }
    }
}
