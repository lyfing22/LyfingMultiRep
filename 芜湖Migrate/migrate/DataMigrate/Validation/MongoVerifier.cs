using DataMigrate.Infrastructure;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.Extensions.Logging;

namespace DataMigrate.Validation;

/// <summary>
/// MongoDB 存在性验证器：对比 Oracle 中的检查号列表，找出哪些在 MongoDB 中缺失。
/// 用于 validate 模式，验证迁移数据完整性。
/// </summary>
public class MongoVerifier
{
    private readonly IMongoCollection<BsonDocument> _collection;
    private readonly ILogger<MongoVerifier> _logger;

    public MongoVerifier(string mongoConn, ILogger<MongoVerifier> logger)
    {
        var client = new MongoClient(mongoConn);
        // archive.migrate 是 MIIS 平台归档集合，_id = MD5(AccessionNumber)
        _collection = client.GetDatabase("MIIS_Archive").GetCollection<BsonDocument>("archive.migrate");
        _logger = logger;
    }

    /// <summary>批量验证：按 AccessionNumber 列表查出缺失的记录</summary>
    public async Task VerifyAsync(List<string> accNumList, CancellationToken ct = default)
    {
        if (accNumList.Count == 0) return;

        _logger.LogInformation("开始 MongoDB 验证, 共 {Count} 条", accNumList.Count);
        var missing = new List<string>();
        var batchSize = 100;

        // 分批查询 MongoDB，避免单次 IN 查询过大
        for (int i = 0; i < accNumList.Count; i += batchSize)
        {
            ct.ThrowIfCancellationRequested();
            var batch = accNumList.Skip(i).Take(batchSize).ToList();
            // AccessionNumber → MD5 GUID（与写入时用同一种算法）
            var guids = batch.Select(IdGenerator.FromString).ToList();
            var filter = Builders<BsonDocument>.Filter.In("_id", guids);
            var existing = await _collection.Find(filter).ToListAsync(ct);
            var existingIds = existing.Select(d => d["_id"].AsGuid).ToHashSet();
            // 当前批中未出现在 MongoDB 的检查号
            missing.AddRange(batch.Where((_, idx) => !existingIds.Contains(guids[idx])));
        }

        _logger.LogInformation("验证结果: 存在 {Exist} 条, 缺失 {Missing} 条",
            accNumList.Count - missing.Count, missing.Count);

        if (missing.Count > 0)
        {
            _logger.LogWarning("缺失 {Count} 条记录:", missing.Count);
            foreach (var m in missing.Take(20))
                _logger.LogWarning("  缺失: {Acc}", m);
            if (missing.Count > 20)
                _logger.LogWarning("  ... 还有 {Count} 条未列出", missing.Count - 20);
        }
    }
}
