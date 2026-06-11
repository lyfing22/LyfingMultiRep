using DataMigrate.Core;
using DataMigrate.Infrastructure;
using DataMigrate.Models;
using DataMigrate.Upload;
using DataMigrate.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

// Serilog 静态 Logger：必须在 Host 启动前创建，用于捕获启动期间的异常
Log.Logger = LogHelper.CreateLogger();

try
{
    // === Phase 1: 搭建 Host（配置 → 绑定 → DI 注册 → Build） ===
    using var host = BuildHost(args);
    var sp = host.Services;
    var logger = sp.GetRequiredService<ILogger<Program>>();
    var options = sp.GetRequiredService<MigrationOptions>();

    // === Phase 2: 前置检查（配置校验 + 数据源连通性） ===
    await sp.GetRequiredService<ConfigValidator>().ValidateAllAsync();

    var source = sp.GetRequiredKeyedService<IMigrationSource>(options.SourceType);
    await source.ValidateAsync();
    logger.LogInformation("数据源 {Source} 连接正常", source.SourceName);

    // === Phase 3: 按模式分派 ===
    var range = new DateRange(options.Migration.TimeRange.Start, options.Migration.TimeRange.End);

    switch (options.Migration.Mode.ToLowerInvariant())
    {
        case "debug":
            await RunDebugAsync(sp, source, options, logger);
            break;
        case "validate":
            await RunValidateAsync(sp, source, range, logger);
            break;
        case "audit":
            await RunAuditAsync(sp, range, logger);
            break;
        default:
            await RunBatchAsync(sp, source, range, logger);
            break;
    }

    logger.LogInformation("程序执行完毕");
    return 0;
}
catch (Exception ex)
{
    // Host 之外的全局异常捕获（Host 内部异常由各自 catch 处理）
    Log.Fatal(ex, "程序异常退出");
    return 1;
}
finally
{
    // 确保 Serilog 内部缓冲全部刷入文件/控制台
    await Log.CloseAndFlushAsync();
}

// ───────── Host 搭建 ─────────

/// <summary>搭建 .NET Generic Host：配置管道 → 绑定 Options → DI 注册</summary>
static IHost BuildHost(string[] args)
{
    var builder = Host.CreateApplicationBuilder(args);
    BuildInfrastructure.ConfigureConfiguration(builder);
    var options = BuildInfrastructure.BindOptions(builder.Configuration);
    BuildInfrastructure.ConfigureServices(builder, options);
    return builder.Build();
}

// ───────── 模式分派 ─────────

/// <summary>批量迁移：Producer-Consumer 全量迁移</summary>
static async Task RunBatchAsync(
    IServiceProvider sp, IMigrationSource source, DateRange range, Microsoft.Extensions.Logging.ILogger logger)
{    
    // ActivatorUtilities: 手动传 source（非 DI 注册的），其余参数从 DI 自动注入
    var engine = ActivatorUtilities.CreateInstance<MigrationEngine>(sp, source);
    logger.LogInformation("批量迁移: {Start} ~ {End}",
        range.Start.ToString("yyyy-MM-dd HH:mm"), range.End.ToString("yyyy-MM-dd HH:mm"));
    await engine.RunAsync(range, CancellationToken.None);
}

/// <summary>调试模式：单条查询 + 打印 JSON + 上传</summary>
static async Task RunDebugAsync(
    IServiceProvider sp, IMigrationSource source, MigrationOptions options, Microsoft.Extensions.Logging.ILogger logger)
{
    var accNum = options.Migration.AccessionNumber;
    if (string.IsNullOrWhiteSpace(accNum))
    {
        logger.LogWarning("debug 模式需配置 AccessionNumber");
        return;
    }

    logger.LogInformation("调试模式: 单条 {Acc}", accNum);
    var archive = await source.GetByAccessionNumberAsync(accNum);
    if (archive == null)
    {
        logger.LogWarning("未找到检查号: {Acc}", accNum);
        return;
    }

    // 打印完整 JSON 供人工检查字段映射是否正确
    Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(archive, ArchiveJson.PrettyPrint));

    var uploader = sp.GetRequiredService<ArchiveUploader>();
    var result = await uploader.UploadAsync(archive, CancellationToken.None);
    logger.LogInformation(result.Success ? "上传成功" : "上传失败: {Msg}", result.ErrorMessage);
    await uploader.FlushRemainingAsync();
}

/// <summary>验证模式：对比 Oracle 检查号是否都在 MongoDB 中存在</summary>
static async Task RunValidateAsync(
    IServiceProvider sp, IMigrationSource source, DateRange range, Microsoft.Extensions.Logging.ILogger logger)
{
    var mongoConn = sp.GetRequiredService<MigrationOptions>().ConnectionStrings.MongoDB;
    var verifier = new MongoVerifier(mongoConn, sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<MongoVerifier>>());
    var accNums = new List<string>();

    logger.LogInformation("验证模式: 拉取检查号列表...");
    await foreach (var archive in source.EnumerateArchivesAsync(range, 1000, CancellationToken.None))
        accNums.Add(archive.Order.AccessionNumber);

    logger.LogInformation("共 {Count} 条, 开始 MongoDB 验证", accNums.Count);
    await verifier.VerifyAsync(accNums);
}

/// <summary>审计模式：对比 Oracle 源记录数与 MongoDB 目标记录数</summary>
static async Task RunAuditAsync(
    IServiceProvider sp, DateRange range, Microsoft.Extensions.Logging.ILogger logger)
{
    var auditor = sp.GetRequiredService<PostMigrationAuditor>();
    logger.LogInformation("审计模式: {Start} ~ {End}",
        range.Start.ToString("yyyy-MM-dd HH:mm"), range.End.ToString("yyyy-MM-dd HH:mm"));
    await auditor.AuditAsync(range);
}
