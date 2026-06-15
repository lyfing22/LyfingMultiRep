using DataMigrate.Models;

namespace DataMigrate.Core;

/// <summary>数据源适配接口（Strategy 模式）：每种数据源厂商各自实现</summary>
public interface IMigrationSource
{
    string SourceName { get; }
    /// <summary>验证数据源连通性</summary>
    Task ValidateAsync();
    /// <summary>获取时间范围内的预估记录数</summary>
    Task<SourceMetadata> GetMetadataAsync(DateRange range);
    /// <summary>按检查号查询单条</summary>
    Task<MigrateArchive?> GetByAccessionNumberAsync(string accessionNumber);
    /// <summary>获取源库 SEPERATETIME 的实际最小/最大时间</summary>
    Task<DateRange> GetTimeRangeAsync();
    /// <summary>按时间范围枚举所有记录（单计划单次查询，不分页）</summary>
    IAsyncEnumerable<MigrateArchive> EnumerateArchivesAsync(
        DateRange range, CancellationToken ct);
}
