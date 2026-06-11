using Serilog;

namespace DataMigrate.Infrastructure;

/// <summary>
/// Serilog 日志工厂：创建统一的 Logger 配置。
/// - 控制台模板：简洁时间+级别+消息
/// - 文件日志：logs/migrate-{yyyyMMdd}.log，保留 30 天
/// </summary>
public static class LogHelper
{
    /// <summary>创建 Serilog Logger（必须显式 CreateLogger，以便 finally 中 CloseAndFlushAsync）</summary>
    public static Serilog.Core.Logger CreateLogger()
    {
        return new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File("logs/migrate-.log",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();
    }
}
