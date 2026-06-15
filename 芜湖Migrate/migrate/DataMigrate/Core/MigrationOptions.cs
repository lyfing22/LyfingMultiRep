namespace DataMigrate.Core;

/// <summary>appsettings.json 的强类型映射，所有配置节一一对应</summary>
public class MigrationOptions
{
    /// <summary>数据源类型，与 Keyed DI 键名一致。当前仅 "Neusoft"</summary>
    public string SourceType { get; init; } = "Neusoft";
    public ConnectionStringsOption ConnectionStrings { get; init; } = new();
    public MigrationConfig Migration { get; init; } = new();
    public UploadConfig Upload { get; init; } = new();
    public IdentityConfig Identity { get; init; } = new();
    /// <summary>设备类型→执行科室映射（Key=ModalityCode, Value=DeviceDptInfo）</summary>
    public Dictionary<string, Models.DeviceDptInfo> ModalityDepartment { get; init; } = new();
}

/// <summary>三数据库连接串：Oracle（源）/ SQL Server（追踪）/ MongoDB（验证）</summary>
public class ConnectionStringsOption
{
    /// <summary>Oracle 连接串（源数据库，表 PACS31.STUDYINFO）</summary>
    public string Source { get; init; } = "";
    /// <summary>SQL Server 连接串（目标追踪库，表 ZTemp_MigratePlan/Error）</summary>
    public string Destination { get; init; } = "";
    /// <summary>MongoDB 连接串（目标档案库，数据库 MIIS_Archive）</summary>
    public string MongoDB { get; init; } = "";
}

/// <summary>迁移运行参数</summary>
public class MigrationConfig
{
    /// <summary>
    /// 运行模式。合法值：
    ///   batch    - 全量迁移（自动获取源库 min/max → 切割计划 → 并行执行 → 审计）
    ///   debug    - 两种方式：AccessionNumber(单条) 或 TimeRange(分计划预演)
    ///   validate - MongoDB 验证（对比 Oracle 检查号是否都在 MongoDB 中存在）
    ///   audit    - 迁移后审计（对比 Oracle 源记录数与 MongoDB 目标记录数）
    /// </summary>
    public string Mode { get; init; } = "batch";
    /// <summary>debug 模式：配置此项则查单条 + 打印 JSON + 上传；留空则使用 TimeRange 分计划迁移</summary>
    public string? AccessionNumber { get; init; }
    /// <summary>debug 模式（无 AccessionNumber 时）使用的时间窗口</summary>
    public DateRangeConfig TimeRange { get; init; } = new();
    /// <summary>计划切割天数间隔（默认 1 天）</summary>
    public int PlanIntervalDays { get; init; } = 1;
    /// <summary>最大同时执行的计划数（默认 2）</summary>
    public int MaxParallelPlans { get; init; } = 2;
    /// <summary>计划执行顺序：descending（从新到旧）| ascending（从旧到新）</summary>
    public string PlanOrder { get; init; } = "descending";
    /// <summary>Channel 有界队列容量（默认 200），控制生产者背压阈值</summary>
    public int ChannelCapacity { get; init; } = 200;
    /// <summary>并行上传消费者数（默认 4）</summary>
    public int Parallelism { get; init; } = 4;
    /// <summary>ZTemp_MigrateError 批量刷入批次大小（默认 100）</summary>
    public int ZTempBatchSize { get; init; } = 100;
    /// <summary>数据库标识，用于区分不同医院/环境的迁移计划（对应 ZTemp_MigratePlan.DbFlag）</summary>
    public string DbFlag { get; init; } = "";
}

public class DateRangeConfig
{
    /// <summary>开始时间（含）</summary>
    public DateTime Start { get; init; }
    /// <summary>结束时间（含）</summary>
    public DateTime End { get; init; }
}

/// <summary>HTTP 上传参数</summary>
public class UploadConfig
{
    /// <summary>MIIS 平台归档 API 基地址</summary>
    public string Url { get; init; } = "";
    /// <summary>
    /// JWT 应用标识，用于生成 token。合法值：MIIS / RIS / PACS / MPACS / USES
    /// </summary>
    public string JwtAppId { get; init; } = "";
    /// <summary>JWT 签名密钥</summary>
    public string JwtAppSecret { get; init; } = "";
    /// <summary>Token 有效期（分钟），默认 40。Timer 在到期前 5 分钟自动刷新</summary>
    public int JwtExpiryMinutes { get; init; } = 40;
    /// <summary>HTTP 请求超时时间（秒，默认 30）</summary>
    public int TimeoutSeconds { get; init; } = 30;
}

/// <summary>写入 MigrateArchive 的身份信息</summary>
public class IdentityConfig
{
    /// <summary>PACS 影像存储节点标识</summary>
    public string StorageNode { get; init; } = "";
    /// <summary>当前服务节点标识</summary>
    public string ServerNode { get; init; } = "";
    /// <summary>医院编码</summary>
    public string HospitalCode { get; init; } = "";
    /// <summary>医院名称</summary>
    public string HospitalName { get; init; } = "";
}

public record DateRange(DateTime Start, DateTime End);
public record SourceMetadata(int EstimatedCount, DateTime? MinDate, DateTime? MaxDate);
public record PlanInfo(Guid Id, DateRange Range);
