using DataMigrate.Models;

namespace DataMigrate.Core;

/// <summary>数据源适配接口（Strategy 模式）：每种数据源（Oracle、SQL Server 等）各自实现</summary>
public interface IMigrationSource
{
    string SourceName { get; }
    /// <summary>验证数据源连通性</summary>
    Task ValidateAsync();
    /// <summary>获取时间范围内的预估记录数</summary>
    Task<SourceMetadata> GetMetadataAsync(DateRange range);
    /// <summary>按检查号查询单条</summary>
    Task<MigrateArchive?> GetByAccessionNumberAsync(string accessionNumber);
    /// <summary>按时间范围分页枚举，用于全量迁移/验证</summary>
    IAsyncEnumerable<MigrateArchive> EnumerateArchivesAsync(
        DateRange range, int pageSize, CancellationToken ct);
}
