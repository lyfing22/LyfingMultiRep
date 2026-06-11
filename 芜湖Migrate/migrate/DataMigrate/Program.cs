using DataMigrate.Core;
using DataMigrate.Infrastructure;
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

    // === Phase 2: 配置校验 + 数据源校验（统一由 MigrationRunner 处理） ===
    await sp.GetRequiredService<ConfigValidator>().ValidateAllAsync();
    await sp.GetRequiredService<MigrationRunner>().RunAsync();

    logger.LogInformation("程序执行完毕");
    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "程序异常退出");
    return 1;
}
finally
{
    await Log.CloseAndFlushAsync();
}

static IHost BuildHost(string[] args)
{
    var builder = Host.CreateApplicationBuilder(args);
    BuildInfrastructure.ConfigureConfiguration(builder);
    var options = BuildInfrastructure.BindOptions(builder.Configuration);
    BuildInfrastructure.ConfigureServices(builder, options);
    return builder.Build();
}
