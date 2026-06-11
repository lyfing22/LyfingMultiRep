# DI + Keyed Service + ActivatorUtilities 核心知识点

> 项目重构过程中提炼的 .NET DI 知识点汇总

---

## 一、Keyed DI + 反射自动注册

**知识点**：通过反射扫描程序集，按命名约定自动注册服务。

```csharp
// 扫描所有 IMigrationSource 非抽象实现，类名去 "Source" 后缀作为 key
var types = Assembly.GetExecutingAssembly().GetTypes()
    .Where(t => t is { IsClass: true, IsAbstract: false }
                && t.IsAssignableTo(typeof(IMigrationSource)));

foreach (var type in types)
{
    var key = type.Name.EndsWith("Source")
        ? type.Name[..^"Source".Length]
        : type.Name;

    builder.Services.AddKeyedSingleton<IMigrationSource>(key, (sp, _) =>
        (IMigrationSource)ActivatorUtilities.CreateInstance(sp, type));
}
```

**取用**：

```csharp
// 运行时根据配置 SourceType 决定用哪个实现
var source = sp.GetRequiredKeyedService<IMigrationSource>(options.SourceType);
```

---

## 二、工厂委托 (Factory Delegate)

**知识点**：当服务既有 DI 可解析参数、又有来自配置或上下文的参数时，用 lambda 注册比 `new` 更灵活。

```csharp
// ⚠ 错误：JwtTokenManager 的参数来自配置，不是 DI 注册类型
builder.Services.AddSingleton<JwtTokenManager>(); // ❌ DI 无法推导

// ✅ 正确：手动 new，DI 只负责持有单例
builder.Services.AddSingleton(new JwtTokenManager(
    options.JwtAppId, options.ServerNode, options.JwtExpiryMinutes, options.JwtSecret));

// 混合场景：部分 DI + 部分配置，用 lambda
builder.Services.AddSingleton(sp => new PostMigrationAuditor(
    options.ConnectionStrings.Source,               // 来自配置
    options.ConnectionStrings.MongoDB,               // 来自配置
    sp.GetRequiredService<ILogger<PostMigrationAuditor>>())); // 来自 DI
```

---

## 三、`new` vs `ActivatorUtilities.CreateInstance`

**知识点**：后者按类型匹配显式参数，剩余从 DI 自动填充。

```csharp
// ---- new：所有参数都要自己传 ----
var engine = new MigrationEngine(
    _source,                                    // 手动
    _uploader,                                  // 手动
    _auditor,                                   // 手动
    _stats,                                     // 手动
    _progress,                                  // 手动
    _options,                                   // 手动
    loggerFactory.CreateLogger<MigrationEngine>() // 手动
);

// ---- ActivatorUtilities：只需传 DI 无法推导的 ----
var engine = ActivatorUtilities.CreateInstance<MigrationEngine>(_sp, _source);
// _source → 匹配 IMigrationSource 参数，其余 6 个 _sp.GetRequiredService 自动填
```

**内部逻辑**：

```
1. 反射获取构造函数列表
2. 选参数最多的（或 [ActivatorUtilitiesConstructor] 标记的）
3. 遍历参数，按类型匹配显式传入值，匹配到的消耗掉
4. 剩余参数逐一 sp.GetRequiredService(type) 填充
5. 构造实例
```

---

## 四、Keyed Service 的注入限制

**知识点**：Keyed 注册的服务不能被普通构造函数参数自动推断。

```csharp
// 注册方式 ↓
services.AddKeyedSingleton<IMigrationSource>("Neusoft"); // keyed

// 使用 ↓
class Engine
{
    public Engine(IMigrationSource source) { } // ❌ DI 不会用 keyed 填这个
}

class Engine2
{
    public Engine2([FromKeyedServices("Neusoft")] IMigrationSource src) { } // ✅
}

// 运行时决定 key → 只能用工厂委托 + GetRequiredKeyedService
services.AddSingleton<MigrationRunner>(sp =>
{
    var opts = sp.GetRequiredService<MigrationOptions>();
    var src = sp.GetRequiredKeyedService<IMigrationSource>(opts.SourceType);
    return ActivatorUtilities.CreateInstance<MigrationRunner>(sp, src);
});
```

---

## 五、`finally` + `await` 的陷阱

**知识点**：`finally` 支持 `await`，但要小心掩盖异常和取消语义。

```csharp
// ❌ finally + await 的风险
try
{
    await http.SendAsync(request, ct);
    // ... 成功逻辑
}
catch (OperationCanceledException)
{
    throw; // 取消的不该进 buffer
}
catch (Exception ex)
{
    msg = ex.Message;
}
finally
{
    // ⚠ 取消请求也会走到这里，需要额外 flag 跳过
    // ⚠ 如果 FlushIfNeededAsync 抛异常，会掩盖原始异常
    if (!cancelled)
    {
        _buffer.Add(...);
        await FlushIfNeededAsync();
    }
}

// ✅ 显式重复：3 条路径各写一次，意图清晰
try
{
    // success → buffer + flush + return
    _buffer.Add((acc, true, null));
    await FlushIfNeededAsync();
    return ...;
}
catch (OperationCanceledException) { throw; } // 不进 buffer
catch (Exception ex)
{
    _buffer.Add((acc, false, ex.Message));
    await FlushIfNeededAsync();
    return ...;
}
```

---

## 六、延迟构造 (Lazy Creation)

**知识点**：只在需要时才创建对象，避免不必要的初始化。

```csharp
public class MigrationRunner
{
    public async Task RunAsync()
    {
        switch (mode)
        {
            case "debug":
                await RunDebugAsync();     // 只用 _uploader
                break;
            case "validate":
                await RunValidateAsync();   // 只用 _source
                break;
            case "audit":
                await RunAuditAsync();      // 只用 _auditor
                break;
            default:
                await RunBatchAsync();      // 这里才需要 MigrationEngine
                break;
        }
    }

    private async Task RunBatchAsync()
    {
        // 延迟创建：只有 batch 模式才构造 MigrationEngine
        var engine = ActivatorUtilities.CreateInstance<MigrationEngine>(_sp, _source);
        await engine.RunAsync(range, ct);
    }
}
```

---

## 七、Strategy + Keyed DI 模式

**知识点**：接口 + Keyed DI 实现策略模式，运行时按配置切换实现。

```
┌──────────────────┐
│   IMigrationSource│  ← 接口
├──────────────────┤
│ + ValidateAsync() │
│ + GetByAccNum()   │
│ + Enumerate()     │
└────────┬─────────┘
         ▲
         │ 实现
    ┌────┴────┐
    │Neusoft  │
    │Source   │  ← Keyed("Neusoft")
    └─────────┘
               (未来可加 HisPacsSource → Keyed("HisPacs"))
```

```csharp
// 注册：自动扫描
services.AddKeyedSingleton<IMigrationSource>("Neusoft", ...);

// 消费：按配置切换
var source = sp.GetRequiredKeyedService<IMigrationSource>(options.SourceType);
// appsettings.json:  "SourceType": "Neusoft"
```
