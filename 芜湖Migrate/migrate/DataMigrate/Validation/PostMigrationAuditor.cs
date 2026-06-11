using Dapper;
using DataMigrate.Core;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.Extensions.Logging;

namespace DataMigrate.Validation;

/// <summary>
/// 迁移后审计：分别统计 Oracle 源记录数与 MongoDB 目标记录数，对比差异。
/// 将审计结果写入 ZTemp_MigratePlan 表的 ErrorSummary 字段供外部查看。
/// </summary>
public class PostMigrationAuditor
{
    private readonly string _sourceConn;
    private readonly string _mongoConn;
    private readonly string _destConn;
    private readonly string _dbFlag;
    private readonly ILogger<PostMigrationAuditor> _logger;

    public PostMigrationAuditor(
        string sourceConn,
        string mongoConn,
        string destConn,
        string dbFlag,
        ILogger<PostMigrationAuditor> logger)
    {
        _sourceConn = sourceConn;
        _mongoConn = mongoConn;
        _destConn = destConn;
        _dbFlag = dbFlag;
        _logger = logger;
    }

    /// <summary>对比源和目标记录数，差异写入 ZTemp 追踪表</summary>
    public async Task AuditAsync(DateRange range)
    {
        _logger.LogInformation("=== 迁移后审计 ===");

        try
        {
            // Oracle 源计数（与迁移相同的筛选条件）
            using var srcConn = new Oracle.ManagedDataAccess.Client.OracleConnection(_sourceConn);
            await srcConn.OpenAsync();
            var sourceCount = await srcConn.QuerySingleAsync<int>(@"
                SELECT COUNT(*) FROM PACS31.STUDYINFO
                WHERE SEPERATETIME BETWEEN :start AND :end AND ISAVAILABLE=1",
                new { start = range.Start, end = range.End });
            _logger.LogInformation("Oracle 源记录数: {Count}", sourceCount);

            // MongoDB 目标集合全量计数
            var client = new MongoClient(_mongoConn);
            var db = client.GetDatabase("MIIS_Archive");
            var collection = db.GetCollection<BsonDocument>("archive.migrate");
            var targetCount = (int)await collection.CountDocumentsAsync(FilterDefinition<BsonDocument>.Empty);
            _logger.LogInformation("MongoDB 目标记录数: {Count}", targetCount);

            var diff = sourceCount - targetCount;
            if (diff == 0)
                _logger.LogInformation("✓ 审计通过: 源和目标记录数一致");
            else
                _logger.LogWarning("✗ 审计差异: 源 {Src} 条, 目标 {Tgt} 条, 差异 {Diff} 条",
                    sourceCount, targetCount, diff);

            await SaveAuditResultAsync(sourceCount, targetCount, diff);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "审计执行异常");
        }
    }

    /// <summary>将审计结果追加到最近一次迁移计划的 ErrorSummary</summary>
    private async Task SaveAuditResultAsync(int source, int target, int diff)
    {
        try
        {
            using var conn = new Microsoft.Data.SqlClient.SqlConnection(_destConn);
            await conn.ExecuteAsync(@"
UPDATE ZTemp_MigratePlan
SET ErrorSummary = ErrorSummary + ' | 审计: 源 ' + @Src + ', 目标 ' + @Tgt + ', 差异 ' + @Diff
WHERE DbFlag=@DbFlag AND Status='Completed'",
                new { Src = source.ToString(), Tgt = target.ToString(), Diff = diff.ToString(), DbFlag = _dbFlag });
        }
        catch { }
    }
}
