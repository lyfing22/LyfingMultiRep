using System.Reflection;
using System.Text;
using DataMigrate.Infrastructure;
using DataMigrate.Models;
using DataMigrate.Upload;
using DataMigrate.Validation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace DataMigrate.Core;

/// <summary>配置管道 + DI 容器的构建入口，从 Program.cs 拆分出来</summary>
internal static class BuildInfrastructure
{
    /// <summary>配置管道：清空默认源，只从 appsettings.json 加载（支持 // 注释）</summary>
    public static void ConfigureConfiguration(HostApplicationBuilder builder)
    {
        // 清空默认配置源（环境变量、命令行等），仅保留 appsettings.json
        // 防止环境变量或命令行意外覆盖关键配置
        builder.Configuration.Sources.Clear();

        var raw = File.ReadAllText("appsettings.json");
        var clean = StripJsonLineComments(raw);
        builder.Configuration.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(clean)));
    }

    /// <summary>DI 注册：日志 + 全部服务 + HttpClientFactory + Keyed DataSource</summary>
    public static void ConfigureServices(HostApplicationBuilder builder, MigrationOptions options)
    {
        // ── 日志管道：移除默认的 Console/Debug/EventSource 提供程序，替换为 Serilog ──
        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog();

        // ── 配置对象作为 Singleton 注册，供所有服务直接引用 ──
        builder.Services.AddSingleton(options);

        // ── 无参构造，DI 自动注入 ──
        builder.Services.AddSingleton<MigrationStatistics>();
        builder.Services.AddSingleton<ConfigValidator>();

        // ── 手动构造：参数来自配置（非 DI 注册的），DI 只负责持有实例 ──
        builder.Services.AddSingleton(new JwtTokenManager(
            options.Upload.JwtAppId, options.Identity.ServerNode,
            options.Upload.JwtExpiryMinutes, options.Upload.JwtAppSecret));

        // ── 工厂委托：延迟构造，需要从 DI 获取 ILogger<T>（只在首次解析时才 new） ──
        builder.Services.AddSingleton(sp => new PostMigrationAuditor(
            options.ConnectionStrings.Source, options.ConnectionStrings.MongoDB,
            options.ConnectionStrings.Destination, options.Migration.DbFlag,
            sp.GetRequiredService<ILogger<PostMigrationAuditor>>()));

        builder.Services.AddSingleton<ConsoleProgressBar>();
        builder.Services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<MigrationOptions>();
            var source = sp.GetRequiredKeyedService<IMigrationSource>(options.SourceType);
            return ActivatorUtilities.CreateInstance<MigrationRunner>(sp, source);
        });

        // ── Typed HttpClient：连接池化管理，避免端口耗尽 ──
        builder.Services.AddHttpClient<ArchiveUploader>(client =>
        {
            client.BaseAddress = new Uri(options.Upload.Url);
            client.Timeout = TimeSpan.FromSeconds(options.Upload.TimeoutSeconds);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            // 连接池上限 = 并行消费数 × 4，避免消费者之间串连等待连接
            MaxConnectionsPerServer = options.Migration.Parallelism * 4
        });

        // ── Keyed DI：自动扫描 IMigrationSource 实现，按类名去除 "Source" 后缀作为 key ──
        var sourceTypes = typeof(BuildInfrastructure).Assembly
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false }
                        && t.IsAssignableTo(typeof(IMigrationSource)));

        var deps = new SourceDependencies(
            options.ConnectionStrings.Source, options.Identity, options.ModalityDepartment);
        builder.Services.AddSingleton(deps);

        const string suffix = "Source";
        foreach (var type in sourceTypes)
        {
            var key = type.Name.EndsWith(suffix, StringComparison.Ordinal)
                ? type.Name[..^suffix.Length]
                : type.Name;

            builder.Services.AddKeyedSingleton<IMigrationSource>(key, (sp, _) =>
                (IMigrationSource)ActivatorUtilities.CreateInstance(sp, type));
        }
    }

    /// <summary>配置绑定：IConfiguration → 强类型 MigrationOptions</summary>
    public static MigrationOptions BindOptions(IConfiguration config)
    {
        var options = new MigrationOptions();
        config.Bind(options);
        return options;
    }

    /// <summary>去除 JSON 中的 // 行注释（不处理字符串内的 //）</summary>
    private static string StripJsonLineComments(string json)
    {
        var sb = new StringBuilder(json.Length);
        bool inString = false;
        for (int i = 0; i < json.Length; i++)
        {
            var c = json[i];
            if (c == '"' && (i == 0 || json[i - 1] != '\\'))
                inString = !inString;

            if (!inString && c == '/' && i + 1 < json.Length && json[i + 1] == '/')
            {
                while (i < json.Length && json[i] != '\n') i++;
                sb.Append('\n');
            }
            else
            {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }
}
