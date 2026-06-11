# DataMigrate — Agent 指南

## 项目性质

.NET 8 控制台应用（非 Web API/库），用于将 Oracle PACS 检查数据通过 HTTP API 迁移到 MIIS 平台。

## 构建 & 运行

```powershell
dotnet build .\DataMigrate\DataMigrate.csproj
dotnet run --project .\DataMigrate\DataMigrate.csproj
```

无测试项目、无 CI、无 linter/formatter 配置。`dotnet build` 是唯一验证手段。

## 运行模式

由 `appsettings.json` 中 `Migration.Mode` 控制（也可不修改 appsettings，但程序不支持命令行覆盖 mode）：

| Mode | 说明 |
|------|------|
| `batch`（默认）| 全量迁移：分页查 Oracle → Channel 队列 → 并行 HTTP 上传 → 审计 |
| `debug` | 单条调试：按 `AccessionNumber` 查一条 + 上传 |
| `validate` | MongoDB 验证：对比 Oracle 检查号列表是否都在 MongoDB 中存在 |
| `audit` | 迁移后审计：对比 Oracle 源记录数与 MongoDB 目标记录数 |

## 架构要点

- **Strategy 模式**：`IMigrationSource` 接口，当前仅 `OracleSource` 实现，通过 .NET 8 Keyed DI 按 `SourceType` 注入
- **Producer-Consumer**：`Channel<MigrateArchive>` 有界队列（容量 = PageSize × 2），N 个消费者并行 HTTP 上传（`Parallelism` 配置，默认 4）
- **零重试策略**：上传失败直接写入 `ZTemp_MigrateError` 表，人工后处理
- **确定性的 GUID**：`GenerateGuid()` = `MD5(AccessionNumber)`，同一条检查号始终生成相同 ID（用于 MongoDB _id）

## 三数据库连接

- **Oracle**（`ConnectionStrings.Source`）— 源数据，表 `PACS31.STUDYINFO` 等
- **SQL Server**（`ConnectionStrings.Destination`）— 仅用于 `ZTemp_MigratePlan` / `ZTemp_MigrateError` 两张追踪表
- **MongoDB**（`ConnectionStrings.MongoDB`）— 数据库 `MIIS_Archive`，集合 `archive.migrate`，仅 validate/audit 模式使用

## 文件结构（关键文件）

```
DataMigrate/
├── Program.cs                         # DI 搭建 + 模式分发（batch/debug/validate/audit）
├── appsettings.json                   # 全部配置（含数据库连接串、JWT 密钥）
├── Core/
│   ├── IMigrationSource.cs            # Strategy 接口
│   ├── MigrationEngine.cs             # Producer-Consumer 核心引擎
│   └── MigrationOptions.cs            # 强类型配置模型
├── Sources/Oracle/
│   ├── OracleSource.cs                # Oracle 分页查询 + Archive 构建
│   └── OracleRecord.cs                # SQL 查询 DTO（字段名必须与 SQL AS 别名一致）
├── Upload/
│   ├── ArchiveUploader.cs             # HTTP POST + ZTemp 批量缓冲
│   └── JwtTokenManager.cs             # Timer 自动刷新 JWT token
├── Validation/
│   ├── ConfigValidator.cs             # 启动前检查（连接测试、参数校验）
│   ├── MongoVerifier.cs               # MongoDB 存在性验证
│   └── PostMigrationAuditor.cs        # 源 vs 目标计数对比
├── Models/MigrateArchive.cs           # 上传 JSON 模型（camelCase 序列化）
└── Infrastructure/
    ├── ConsoleProgressBar.cs          # 实时进度条
    └── LogHelper.cs                   # Serilog 配置
```

## 关键约定

- `OracleRecord` 属性名必须与 SQL 中 `AS` 别名 **完全一致**，否则 Dapper 映射失败
- 添加新数据源需：实现 `IMigrationSource` → 在 `Program.cs` 用 `AddKeyedSingleton<IMigrationSource, XxxSource>("Xxx")` 注册
- 日志文件写入 `logs/migrate-{yyyyMMdd}.log`，保留 30 天

## ⚠ 安全注意

`appsettings.json` 包含数据库密码和 JWT 密钥。**切勿提交真实凭据到版本控制**。
