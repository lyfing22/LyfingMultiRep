# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## 项目性质

.NET 8 控制台应用，用于将 Oracle PACS 检查数据通过 HTTP API 迁移到 MIIS 平台。无测试项目、无 CI、无 linter/formatter 配置。`dotnet build` 是唯一验证手段。

## 构建 & 运行

```powershell
dotnet build .\DataMigrate\DataMigrate.csproj
dotnet run --project .\DataMigrate\DataMigrate.csproj
```

## 运行模式

由 `DataMigrate/appsettings.json` 中 `Migration.Mode` 控制：

| Mode | 说明 |
|------|------|
| `batch`（默认）| 全量迁移：分页查 Oracle -> Channel 队列 -> 并行 HTTP 上传 -> 审计 |
| `debug` | 单条调试：按 `AccessionNumber` 查一条 + 上传；或按 `TimeRange` 分计划预演 |
| `validate` | MongoDB 验证：对比 Oracle 检查号列表是否都在 MongoDB 中存在 |
| `audit` | 迁移后审计：对比 Oracle 源记录数与 MongoDB 目标记录数 |

## 架构要点

- **Strategy 模式**：`IMigrationSource` 接口，当前仅 `NeusoftSource` 实现，通过 .NET 8 Keyed DI 按 `SourceType` 注入。新增数据源只需实现接口并命名为 `{Key}Source`，自动注册
- **Producer-Consumer**：`Channel<MigrateArchive>` 有界队列（容量 = PageSize x 2），N 个消费者并行 HTTP 上传（`Parallelism` 配置，默认 4）
- **零重试策略**：上传失败直接写入 `ZTemp_MigrateError` 表，人工后处理
- **确定性的 GUID**：`GenerateGuid()` = `MD5(AccessionNumber)`，同一条检查号始终生成相同 ID（用于 MongoDB _id 幂等写入）
- **分计划迁移**：`MigrationRunner.SplitRange()` 将全局时间范围按 `PlanIntervalDays` 切割为子计划，`BatchCreatePlanRecordsAsync()` 用 SqlBulkCopy 批量写入 `ZTemp_MigratePlan`，然后 `Parallel.ForEachAsync` 并发执行各计划
- **配置加载**：`BuildInfrastructure.StripJsonLineComments()` 去除 JSON 中的 `//` 注释（Oracle 配置文件中常见），再通过 `AddJsonStream` 加载

## 三数据库连接

- **Oracle**（`ConnectionStrings.Source`）- 源数据，表 `PACS31.STUDYINFO` 等
- **SQL Server**（`ConnectionStrings.Destination`）- 仅用于 `ZTemp_MigratePlan` / `ZTemp_MigrateError` 两张追踪表
- **MongoDB**（`ConnectionStrings.MongoDB`）- 数据库 `MIIS_Archive`，集合 `archive.migrate`

## 文件结构（关键文件）

```
DataMigrate/
Program.cs                         # DI 搭建 + 模式分发（batch/debug/validate/audit）
appsettings.json                   # 全部配置（含数据库连接串、JWT 密钥）
Core/
  IMigrationSource.cs              # Strategy 接口
  MigrationEngine.cs               # Producer-Consumer 核心引擎
  MigrationOptions.cs              # 强类型配置模型
  MigrationRunner.cs               # 模式路由 + 分计划迁移编排
  BuildInfrastructure.cs           # 配置管道 + DI 注册
  SourceDependencies.cs            # 数据源构造参数
  MigrationStatistics.cs           # 全局统计
Infrastructure/
  LogHelper.cs                     # Serilog 配置
  IdGenerator.cs                   # 确定性 GUID 生成
  ConsoleProgressBar.cs            # 实时进度条
Models/
  MigrateArchive.cs                # 上传 JSON 模型（camelCase 序列化）
Sources/Neusoft/
  NeusoftSource.cs                 # 东软 PACS 分页查询 + Archive 构建
  NeusoftRecord.cs                 # SQL 查询 DTO
Upload/
  ArchiveUploader.cs               # HTTP POST + ZTemp 批量缓冲
  JwtTokenManager.cs               # Timer 自动刷新 JWT token
Validation/
  ConfigValidator.cs               # 启动前检查（连接测试、参数校验）
  MongoVerifier.cs                 # MongoDB 存在性验证
  PostMigrationAuditor.cs          # 源 vs 目标计数对比
```

## 关键约定

- `NeusoftRecord` 属性名必须与 SQL 中 `AS` 别名 **完全一致**，否则 Dapper 映射失败
- 日志文件写入 `logs/migrate-{yyyyMMdd}.log`，保留 30 天
- `ArchiveUploader` 使用内存缓冲 + 批量 MERGE 写入 `ZTemp_MigrateError`，HTTP 和 SQL 操作分离以避免阻塞上传管道
- `JwtTokenManager` 使用 Timer 定期刷新 token，刷新时机 = `ExpiryMinutes - 5` 分钟

## 安全注意

`appsettings.json` 包含数据库密码和 JWT 密钥。**切勿提交真实凭据到版本控制**。
