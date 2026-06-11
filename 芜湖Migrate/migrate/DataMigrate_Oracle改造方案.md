# DataMigrate Oracle PACS 迁移改造方案

## 背景

现有 DataMigrate 控制台应用（.NET Framework 4.7.2）原从 Impax_XR (SQL Server) 迁移检查数据至 MIIS。现需改为从空军特色医学中心 Oracle PACS 数据库直接迁移。

- Oracle 服务器：192.168.3.86
- 数据库：PACS
- 用户名：test
- Schema：PACS31
- Oracle.ManagedDataAccess NuGet 包

---

## 改造内容

### 1. DataMigrate.csproj — 添加 Oracle 引用

```xml
<Reference Include="Oracle.ManagedDataAccess, Version=..., Culture=neutral, PublicKeyToken=...">
  <HintPath>..\packages\Oracle.ManagedDataAccess.21.12.0\lib\net462\Oracle.ManagedDataAccess.dll</HintPath>
</Reference>
```

或通过 NuGet Package Manager 安装：`Install-Package Oracle.ManagedDataAccess`

---

### 2. App.config — 清理并配置

#### oldRisConn 改为 Oracle 连接串

```xml
<add key="oldRisConn" value="User Id=test;Password=test;Data Source=192.168.3.86/PACS;Connection Timeout=600;" />
```

#### 清理 Impax_XR 脏数据映射（删除全部乱码设备类型）

删除以下所有条目（约 150 行），例如：

```
<add key="-" value="DX" />
<add key="    0 -" value="CT" />
<add key="    Z" value="DX" />
<add key=" CR" value="CR" />
...（全部删除）
```

只保留干净的标准 modality→科室映射：

```xml
<add key="XA" value="{'ExecDepartmentCode':'704','ExecDepartmentName':'放射科'}" />
<add key="CR" value="{'ExecDepartmentCode':'704','ExecDepartmentName':'放射科'}" />
<add key="CT" value="{'ExecDepartmentCode':'704','ExecDepartmentName':'放射科'}" />
<add key="DX" value="{'ExecDepartmentCode':'704','ExecDepartmentName':'放射科'}" />
<add key="MR" value="{'ExecDepartmentCode':'704','ExecDepartmentName':'放射科'}" />
```

#### IsDev 改为 1（调试模式）

```xml
<add key="IsDev" value="1" />
```

---

### 3. Program.cs — 核心改造

#### 3.1 引入

```csharp
using Oracle.ManagedDataAccess.Client;
```

删除不再需要的：

```csharp
// using IBM.Data.DB2.iSeries;  // 删除
```

保留 SqlClient（newRisConn 仍用 SQL Server）。

#### 3.2 简化 IsDev 模式

仅保留三种模式：

| IsDev | 模式 | 说明 |
|-------|------|------|
| `0` | 生产 | 按 SEPERATETIME 倒序批量迁移 |
| `1` | 调试 | 按 SyncAccessionNumber 单条或 SyncTimeRange 范围 |
| `5` | 验证 | 查询 MongoDB 校验 |

**删除的分支：** `IsDev==2`（修设备类型）、`3`（空检查号）、`4`（重复检查号）、`6`/`61`（验证变体）

**删除的方法：** `MigrateOrderListByStudyKey()`（Oracle AccessionNumber 非空唯一，无需此逻辑）

**删除的代码段：**
- `GetArchiveByAccssionNum()` 方法中的 `IsDev==2` 设备类型修正分支
- `GetArchiveByDbOrder()` 中的 `IsDev==2` 分支
- `FillOrderExeInfo()` 中的 `IsDev==2` 设备类型修正逻辑
- `GetArchiveByAccssionNum()` 中空 AccessionNumber 补全逻辑（Oracle 不会出现空值）

#### 3.3 更新 `GetConn()` 支持 Oracle

```csharp
enum DbType { SqlServer, Oracle } // 删除 Db2

static DbConnection GetConn(string connStr, DbType dbType = DbType.SqlServer)
{
    if (dbType == DbType.Oracle)
    {
        var conn = new OracleConnection(connStr);
        conn.Open();
        return conn;
    }
    // SqlServer
    var sqlConn = new SqlConnection(connStr);
    sqlConn.Open();
    return sqlConn;
}
```

#### 3.4 重写 `GetDbOrderList()` — Oracle 查询

**查询方式不同：**
- Oracle Dapper 参数用 `:name` 而非 `@name`
- Oracle 日期字段用 `TO_CHAR` 转字符串
- 表名加 `PACS31.` Schema 前缀

**按西交大脚本提取的 SQL 查询（查单个检查号）：**

```sql
SELECT 
  s.STUDYID                          AS GlobalPatientId,
  p.PATIENTNAME                       AS Name,
  p.PatientSpellName                  AS SpellName,
  CASE WHEN p.SEX='男' THEN 'M'
       WHEN p.SEX='女' THEN 'F'
       ELSE 'U' END                   AS Gender,
  TO_CHAR(p.BIRTHDAY,'yyyy-mm-dd')    AS DateOfBirth,
  p.Address                           AS Address,
  p.Phonenumber                       AS Telephone,
  s.CHECKSERIALNUM                    AS AccessionNumber,
  s.SICKROOM                          AS BedNumber,
  DECODE(p.hispatienttype,'1','OP','2','IH','OP') AS PatientType,
  CASE WHEN s.IFEMERGENCY=1 THEN '1' ELSE '3' END AS EmergencyDegree,
  p.clinicpatientid                   AS ClinicalNumber,
  p.infeepatientid                    AS InpatientNumber,
  s.DIAGID                            AS HisOrderCode,
  s.STUDYSCRIPTION,
  s.FEETOTAL                          AS TotalFee,
  s.AGE,
  s.AGEUNIT,
  s.DSTUDYUID                         AS StudyInstanceUID,
  TO_CHAR(s.STUDYTIME,'yyyy-mm-dd HH24:MI:SS') AS StudyDate,
  s.CHKDEPTID                         AS ExecDepartmentCode,
  dt.Modality                         AS ModalityCode,
  s.DEVICEID                          AS DeviceCode,
  s.DEPARTMENTID                      AS ApplyDepartmentCode,
  TO_CHAR(s.SEPERATETIME,'yyyy-mm-dd HH24:MI:SS') AS ArriveTime,
  r.REPORTDESCRIBE                    AS Findings,
  r.REPORTDIAGNOSE                    AS Impression,
  TO_CHAR(r.OPERATETIME,'yyyy-mm-dd HH24:MI:SS') AS SubmitDateTime,
  r.DOCID1                            AS SubmitDoctorCode,
  r.OPERATORID                        AS ApproveDoctorCode,
  (SELECT USERNAME FROM PACS31.PACSUSER WHERE USERID=r.DOCID1) AS SubmitDoctorName,
  (SELECT USERNAME FROM PACS31.PACSUSER WHERE USERID=r.OPERATORID) AS ApproveDoctorName,
  s.OPERATORID                        AS CheckInDoctorCode,
  (SELECT USERNAME FROM PACS31.PACSUSER WHERE USERID=s.OPERATORID) AS CheckInDoctorName
FROM PACS31.STUDYINFO s
LEFT JOIN PACS31.PATIENTINFO p           ON s.CHECKSERIALNUM = p.CHECKSERIALNUM
LEFT JOIN PACS31.PATIENTDIAGRPTINFO r    ON s.DIAGRPTID = r.DIAGRPTID
LEFT JOIN PACS31.DEVICETYPEINFO dt       ON s.DEVICETYPEID = dt.DevicetypeId
WHERE s.CHECKSERIALNUM = :AccessionNumber
  AND s.ISAVAILABLE = 1
```

**时间范围查询（生产/调试按时间段）：**

```sql
SELECT s.CHECKSERIALNUM
FROM PACS31.STUDYINFO s
WHERE s.SEPERATETIME BETWEEN :start AND :end
  AND s.ISAVAILABLE = 1
```

#### 3.5 更新 `GetArchiveByDbOrder()` 字段映射

按西交大脚本逻辑修改以下处理：

##### 年龄单位翻译

```csharp
// 在 HisDbOrder 中增加 AgeUnit 字段（来自 Oracle AGEUNIT）
// 放到 AgeDisplay 属性中使用
string ageUnitDisplay = "";
switch (dbOrder.AgeUnit)
{
    case "年": ageUnitDisplay = "year"; break;
    case "月": ageUnitDisplay = "month"; break;
    case "天": ageUnitDisplay = "day"; break;
    case "小时": ageUnitDisplay = "hour"; break;
    default: ageUnitDisplay = "year"; break;
}
PatientInfo.AgeDisplay = dbOrder.Age.ToString() + ageUnitDisplay;
```

（注：HisDbOrder 需增加 `Age` 和 `AgeUnit` 字段，或复用现有 `AgeDisplay`）

##### 状态推算（按西交大脚本逻辑）

```csharp
string status;
if (!string.IsNullOrEmpty(dbOrder.SubmitDoctorCode))
    status = "Reported";
else if (!string.IsNullOrEmpty(dbOrder.StudyInstanceUID))
    status = "Studyed";
else if (/* dbOrder has scheduled date and original status=预约 */)
    status = "Ordered";
else
    status = "Arrived";
OrderInfo.Status = status;
```

##### Gender 映射修正

原有代码为 `"男" ? "M" : "F"`，需改为：

```csharp
Gender = dbOrder.Gender  // 已在查询中转为 M/F/U
```

---

### 4. 生产模式（IsDev=0）时间边界查询

原查询 dbo.Study 表时间边界，改为：

```csharp
// 查询最老时间
EndTimeFlag = conn.Query<DateTime>(
    "SELECT MIN(s.SEPERATETIME) FROM PACS31.STUDYINFO s WHERE s.ISAVAILABLE=1"
).FirstOrDefault();

// 查询最新时间
endTime = conn.Query<DateTime>(
    "SELECT MAX(s.SEPERATETIME) FROM PACS31.STUDYINFO s WHERE s.ISAVAILABLE=1"
).FirstOrDefault();
```

---

### 5. 不需要修改的文件

| 文件 | 原因 |
|------|------|
| `RestService.cs` | REST API 上传逻辑完全不变 |
| `JWTUtil.cs` | JWT 令牌获取逻辑完全不变 |
| `MigrateArchive.cs` | 目标模型完全复用（HisDbOrder 需增加 Age/AgeUnit 字段） |
| `StringUtil.cs` | 工具方法完全不变 |
| `ArchiveViewMongo/*` | MongoDB 验证逻辑完全不变 |

---

### 6. HisDbOrder 需补充的字段

为完整映射 Oracle 字段，建议在 `HisDbOrder` 类中补充：

```csharp
public string Age { get; set; }         // PACS31.STUDYINFO.AGE
public string AgeUnit { get; set; }     // PACS31.STUDYINFO.AGEUNIT
```

---

### 7. 验证方式

1. **调试模式**（IsDev=1）：用已知 AccessionNumber 迁移单条，检查上传 JSON 字段映射是否正确
2. **验证模式**（IsDev=5）：对已迁移数据查询 MongoDB 确认存在
3. **生产模式**（IsDev=0）：按时间倒序全量迁移

---

### 8. 回退方案

改造前 `App.config` 和 `Program.cs` 已有 git 版本管理，如需回退到 Impax_XR 源：
1. `git checkout -- App.config` 恢复配置
2. `git checkout -- Program.cs` 恢复代码
3. 重新添加 `IBM.Data.DB2.iSeries` 引用（若已删除）
