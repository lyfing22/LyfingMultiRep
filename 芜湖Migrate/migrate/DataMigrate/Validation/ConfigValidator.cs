using Dapper;
using DataMigrate.Core;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;

namespace DataMigrate.Validation;

/// <summary>
/// 启动前配置自检：校验必填字段 + 测试三数据库连通性 + API 可达性。
/// 未通过则抛 InvalidOperationException 阻止程序启动，避免运行时靠运气。
/// </summary>
public class ConfigValidator
{
    private readonly MigrationOptions _options;
    private readonly ILogger<ConfigValidator> _logger;

    public ConfigValidator(MigrationOptions options, ILogger<ConfigValidator> logger)
    {
        _options = options;
        _logger = logger;
    }

    /// <summary>全量检查入口：身份信息 → JWT → 时间范围 → 数据库连通性 → API 可达</summary>
    public async Task ValidateAllAsync()
    {
        var errors = new List<string>();
        _logger.LogInformation("=== 启动前配置检查 ===");

        // ── 身份信息不可为空 ──
        var identity = _options.Identity;
        if (string.IsNullOrWhiteSpace(identity.StorageNode))
            errors.Add("StorageNode 未配置");
        if (string.IsNullOrWhiteSpace(identity.ServerNode))
            errors.Add("ServerNode 未配置");
        if (string.IsNullOrWhiteSpace(identity.HospitalCode))
            errors.Add("HospitalCode 未配置");
        if (string.IsNullOrWhiteSpace(identity.HospitalName))
            errors.Add("HospitalName 未配置");

        // ── JWT 参数校验 ──
        var validAppIds = new HashSet<string> { "MIIS", "RIS", "PACS", "MPACS", "USES" };
        if (!validAppIds.Contains(_options.Upload.JwtAppId))
            errors.Add($"无效的 JWT_AppId: {_options.Upload.JwtAppId}，有效值: {string.Join(", ", validAppIds)}");
        if (string.IsNullOrWhiteSpace(_options.Upload.JwtAppSecret))
            errors.Add("JWT_AppSecret 未配置");

        // ── 根据模式校验必要参数 ──
        var mode = _options.Migration.Mode;
        if (mode is "debug")
        {
            var hasAccNum = !string.IsNullOrWhiteSpace(_options.Migration.AccessionNumber);
            var hasTimeRange = _options.Migration.TimeRange.Start != default
                               && _options.Migration.TimeRange.End != default;
            if (!hasAccNum && !hasTimeRange)
                errors.Add("Debug 模式需配置 AccessionNumber（单条）或 TimeRange（区间预演）");
        }

        // batch/validate/audit 不再依赖配置中的 TimeRange，改为从源库自动获取

        // ── SQL Server 连通性测试（ZTemp 追踪表需要） ──
        if (!string.IsNullOrWhiteSpace(_options.ConnectionStrings.Destination))
        {
            try
            {
                using var conn = new SqlConnection(_options.ConnectionStrings.Destination);
                await conn.OpenAsync();
                await conn.QuerySingleAsync<int>("SELECT 1");
                _logger.LogInformation("SQL Server 连接正常");
            }
            catch (Exception ex)
            {
                errors.Add($"SQL Server 连接失败: {ex.Message}");
            }
        }

        // ── MongoDB 连通性测试（仅 validate/audit 模式需要） ──
        if (!string.IsNullOrWhiteSpace(_options.ConnectionStrings.MongoDB) && mode is "validate" or "audit")
        {
            try
            {
                var client = new MongoDB.Driver.MongoClient(_options.ConnectionStrings.MongoDB);
                client.GetDatabase("MIIS_Archive").RunCommand<MongoDB.Bson.BsonDocument>("{ ping: 1 }");
                _logger.LogInformation("MongoDB 连接正常");
            }
            catch (Exception ex)
            {
                errors.Add($"MongoDB 连接失败: {ex.Message}");
            }
        }

        // ── HTTP API 可达性测试 ──
        if (!string.IsNullOrWhiteSpace(_options.Upload.Url))
        {
            try
            {
                using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
                var resp = await http.GetAsync(_options.Upload.Url);
                _logger.LogInformation("API 可达, 状态: {Status}", (int)resp.StatusCode);
            }
            catch (Exception ex)
            {
                errors.Add($"API 不可达: {ex.Message}");
            }
        }

        // ── 汇总报告 ──
        if (errors.Count > 0)
        {
            _logger.LogError("配置检查未通过: {Count} 项错误", errors.Count);
            foreach (var e in errors)
                _logger.LogError("  ✗ {Error}", e);
            throw new InvalidOperationException(
                $"启动前检查失败 ({errors.Count} 项):\n" + string.Join("\n", errors));
        }

        _logger.LogInformation("=== 配置检查全部通过 ===");
    }
}
