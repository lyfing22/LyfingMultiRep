# DataMigrate — .NET 8 通用迁移工具

## 设计原则

- **通用架构**：`IMigrationSource` 接口，每个厂商一个实现（目前 Oracle PACS，后续可扩展）
- **零重试**：上传失败直接写入错误表，人工后处理
- **效率优先**：分页联表查询 + 批量写 ZTemp + 并行上传
- **可观测**：实时进度条 + 计划表进度追踪 + 迁移后审计

## 文件结构

```
migrateWH/DataMigrate/
├── DataMigrate.sln
└── DataMigrate/
    ├── DataMigrate.csproj
    ├── Program.cs
    ├── appsettings.json
    │
    ├── Core/
    │   ├── IMigrationSource.cs
    │   ├── MigrationEngine.cs
    │   └── MigrationOptions.cs
    │
    ├── Sources/
    │   └── Oracle/
    │       ├── OracleSource.cs
    │       └── OracleRecord.cs
    │
    ├── Upload/
    │   ├── ArchiveUploader.cs
    │   └── JwtTokenManager.cs
    │
    ├── Validation/
    │   ├── ConfigValidator.cs
    │   ├── MongoVerifier.cs
    │   └── PostMigrationAuditor.cs
    │
    ├── Models/
    │   └── MigrateArchive.cs
    │
    └── Infrastructure/
        ├── ConsoleProgressBar.cs
        └── LogHelper.cs
```

## 核心设计模式

| 模式 | 位置 | 说明 |
|------|------|------|
| **Strategy** | `IMigrationSource` | 不同厂商不同的查询/构建策略 |
| **Facade** | `OracleSource` | 对 Engine 屏蔽 Oracle 内部细节 |
| **Template Method** | `MigrationEngine.RunAsync()` | 固定流程骨架 |
| **Factory** | .NET 8 Keyed DI | 按 `appsettings.SourceType` 创建对应 Source |
| **Options** | `MigrationOptions` | 强类型配置验证 |
| **Singleton** | `JwtTokenManager` | 全局唯一 token 持有者 |

## IMigrationSource 接口

```csharp
interface IMigrationSource {
    Task ValidateAsync();
    Task<SourceMetadata> GetMetadataAsync(DateTime start, DateTime end);
    IAsyncEnumerable<MigrateArchive> EnumerateArchivesAsync(
        DateTime start, DateTime end, int pageSize, CancellationToken ct);
}
```

## 关键性能优化

| # | 优化 | 效果 |
|---|------|------|
| 1 | 分页联表查询 (JOIN + OFFSET FETCH) | 消除 N+1，50万条只需 5000 次 Oracle 往返 |
| 2 | ZTemp 批量写入 (攒 100 条一次 MERGE) | 50万条只需 5000 次 SQL Server 往返 |
| 3 | Producer-Consumer (N 个消费者并行上传) | 吞吐量 xN (默认 4) |
| 4 | System.Text.Json 替代 Newtonsoft | 序列化快 2-3x |
| 5 | Channel<T> 有界队列自动背压 | 内存稳定，不会 OOM |
| 6 | Oracle FetchSize 调大 | 减少分页读取往返 |

## ZTemp 表结构

```sql
-- 错误表 (单行幂等，每条检查号只存最后一次错误)
CREATE TABLE ZTemp_MigrateError (
    DbFlag          NVARCHAR(50) NOT NULL,
    AccessionNumber NVARCHAR(100) NOT NULL,
    ErrorMessage    NVARCHAR(MAX),
    LastOccurredAt  DATETIME NOT NULL,
    CONSTRAINT PK_MigrateError PRIMARY KEY (DbFlag, AccessionNumber)
)

-- 计划表 (带进度追踪)
CREATE TABLE ZTemp_MigratePlan (
    Id              UNIQUEIDENTIFIER PRIMARY KEY,
    DbFlag          NVARCHAR(50) NOT NULL,
    TimeRangeStart  DATETIME NOT NULL,
    TimeRangeEnd    DATETIME NOT NULL,
    TotalRecords    INT NOT NULL DEFAULT 0,
    SuccessCount    INT NOT NULL DEFAULT 0,
    FailedCount     INT NOT NULL DEFAULT 0,
    Status          NVARCHAR(20) NOT NULL DEFAULT 'Running',
    CreatedAt       DATETIME NOT NULL DEFAULT GETDATE(),
    CompletedAt     DATETIME NULL,
    ErrorSummary    NVARCHAR(MAX) NULL
)
```

## 运行模式

| 参数 | 行为 |
|------|------|
| `--mode=batch --from=2024-01-01 --to=2024-12-31` | 生产迁移 |
| `--mode=debug --accession=DX23380160` | 单条调试 |
| `--mode=validate --from=2024-01-01 --to=2024-12-31` | MongoDB 验证 |
| `--mode=audit --from=2024-01-01 --to=2024-12-31` | 迁移后审计 |
