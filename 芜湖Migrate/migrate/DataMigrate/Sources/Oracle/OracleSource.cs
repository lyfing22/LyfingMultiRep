using System.Data;
using System.Runtime.CompilerServices;
using Dapper;
using DataMigrate.Core;
using DataMigrate.Infrastructure;
using DataMigrate.Models;
using Oracle.ManagedDataAccess.Client;

namespace DataMigrate.Sources.Oracle;

/// <summary>IMigrationSource 的 Oracle 实现：联表查询 PACS31 schema → 构建 MigrateArchive</summary>
public class OracleSource : IMigrationSource
{
    private readonly string _connStr;
    private readonly IdentityConfig _identity;
    // 设备类型 → 执行科室映射（如 CT → 放射科）
    private readonly Dictionary<string, DeviceDptInfo> _modalityMap;

    public string SourceName => "Oracle PACS";

    public OracleSource(string connStr, IdentityConfig identity, Dictionary<string, DeviceDptInfo> modalityMap)
    {
        _connStr = connStr;
        _identity = identity;
        _modalityMap = modalityMap;
    }

    public async Task ValidateAsync()
    {
        using var conn = new OracleConnection(_connStr);
        await conn.OpenAsync();
        await conn.QuerySingleAsync<int>("SELECT 1 FROM DUAL");
    }

    /// <summary>按检查号查询单条（debug 模式用）</summary>
    public async Task<MigrateArchive?> GetByAccessionNumberAsync(string accessionNumber)
    {
        using var conn = new OracleConnection(_connStr);
        await conn.OpenAsync();

        var row = await conn.QuerySingleOrDefaultAsync<OracleRecord>(@"
            SELECT
              s.STUDYID                          AS GlobalPatientId,
              p.PATIENTNAME                       AS Name,
              p.PatientSpellName                  AS SpellName,
              CASE WHEN p.SEX='男' THEN 'M'      -- M=男/F=女/U=未知
                   WHEN p.SEX='女' THEN 'F'
                   ELSE 'U' END                   AS Gender,
              p.BIRTHDAY                           AS DateOfBirth,
              p.Address,
              p.Phonenumber                       AS Telephone,
              s.CHECKSERIALNUM                    AS AccessionNumber,
              s.SICKROOM                          AS BedNumber,
              DECODE(p.hispatienttype,'1','OP','2','IH','OP') AS PatientType,  -- OP=门诊/IH=住院
              CASE WHEN s.IFEMERGENCY=1 THEN '1' ELSE '3' END AS EmergencyDegree,  -- 1=急诊/3=普通
              p.clinicpatientid                   AS ClinicalNumber,
              p.infeepatientid                    AS InpatientNumber,
              s.DIAGID                            AS HisOrderCode,
              s.FEETOTAL                          AS TotalFee,
              s.AGE,
              s.AGEUNIT,
              s.DSTUDYUID                         AS StudyInstanceUID,
              s.STUDYTIME                          AS StudyDate,
              s.CHKDEPTID                         AS ExecDepartmentCode,
              dt.Modality                         AS ModalityCode,
              s.DEVICEID                          AS DeviceCode,
              s.DEPARTMENTID                      AS ApplyDepartmentCode,
              s.SEPERATETIME                       AS ArriveTime,
              s.SEPERATETIME                       AS RegisterTime,
              s.OPERATORID                        AS CheckInDoctorCode,
              (SELECT USERNAME FROM PACS31.PACSUSER WHERE USERID=s.OPERATORID) AS CheckInDoctorName,
              r.REPORTDESCRIBE                    AS Findings,
              r.REPORTDIAGNOSE                    AS Impression,
              r.OPERATETIME                        AS SubmitDateTime,
              r.DOCID1                            AS SubmitDoctorCode,
              (SELECT USERNAME FROM PACS31.PACSUSER WHERE USERID=r.DOCID1) AS SubmitDoctorName,
              r.OPERATORID                        AS ApproveDoctorCode,
              (SELECT USERNAME FROM PACS31.PACSUSER WHERE USERID=r.OPERATORID) AS ApproveDoctorName,
              psi.IMAGECOUNT                      AS ImageCount,
              s.STUDYSCRIPTION                     AS StudyScription,
              s.BESPEAKTIME                        AS ScheduledDate,
              s.QUEUENO                            AS QueueNo
            FROM PACS31.STUDYINFO s
            LEFT JOIN PACS31.PATIENTINFO p           ON s.CHECKSERIALNUM = p.CHECKSERIALNUM
            LEFT JOIN PACS31.PATIENTDIAGRPTINFO r    ON s.DIAGRPTID = r.DIAGRPTID
            LEFT JOIN PACS31.DEVICETYPEINFO dt       ON s.DEVICETYPEID = dt.DevicetypeId
            LEFT JOIN PACS31.PACS_STUDYINFO psi      ON s.CHECKSERIALNUM = psi.ACCESSIONNUMBER
            WHERE s.CHECKSERIALNUM = :acc AND s.ISAVAILABLE = 1",
            new { acc = accessionNumber });

        return row != null ? BuildArchive(row) : null;
    }

    /// <summary>获取时间范围内预估记录数</summary>
    public async Task<SourceMetadata> GetMetadataAsync(DateRange range)
    {
        using var conn = new OracleConnection(_connStr);
        await conn.OpenAsync();

        var count = await conn.QuerySingleAsync<int>(@"
            SELECT COUNT(*)
            FROM PACS31.STUDYINFO s
            WHERE s.SEPERATETIME BETWEEN :start AND :end
              AND s.ISAVAILABLE = 1", new { start = range.Start, end = range.End });

        return new SourceMetadata(count, range.Start, range.End);
    }

    /// <summary>
    /// 按时间范围分页枚举（配合 MigrationEngine 的 Producer-Consumer 模式）。
    /// 使用 Oracle 12c+ OFFSET FETCH 分页语法。
    /// </summary>
    public async IAsyncEnumerable<MigrateArchive> EnumerateArchivesAsync(
        DateRange range, int pageSize, [EnumeratorCancellation] CancellationToken ct)
    {
        using var conn = new OracleConnection(_connStr);
        await conn.OpenAsync();

        int offset = 0;
        bool hasMore = true;

        while (hasMore && !ct.IsCancellationRequested)
        {
            var rows = (await conn.QueryAsync<OracleRecord>(@"
                SELECT
                  s.STUDYID                          AS GlobalPatientId,
                  p.PATIENTNAME                       AS Name,
                  p.PatientSpellName                  AS SpellName,
                  CASE WHEN p.SEX='男' THEN 'M'      -- M=男/F=女/U=未知
                       WHEN p.SEX='女' THEN 'F'
                       ELSE 'U' END                   AS Gender,
                  p.BIRTHDAY                           AS DateOfBirth,
                  p.Address,
                  p.Phonenumber                       AS Telephone,
                  s.CHECKSERIALNUM                    AS AccessionNumber,
                  s.SICKROOM                          AS BedNumber,
                  DECODE(p.hispatienttype,'1','OP','2','IH','OP') AS PatientType,  -- OP=门诊/IH=住院
                  CASE WHEN s.IFEMERGENCY=1 THEN '1' ELSE '3' END AS EmergencyDegree,  -- 1=急诊/3=普通
                  p.clinicpatientid                   AS ClinicalNumber,
                  p.infeepatientid                    AS InpatientNumber,
                  s.DIAGID                            AS HisOrderCode,
                  s.FEETOTAL                          AS TotalFee,
                  s.AGE,
                  s.AGEUNIT,
                  s.DSTUDYUID                         AS StudyInstanceUID,
                  s.STUDYTIME                          AS StudyDate,
                  s.CHKDEPTID                         AS ExecDepartmentCode,
                  dt.Modality                         AS ModalityCode,
                  s.DEVICEID                          AS DeviceCode,
                  s.DEPARTMENTID                      AS ApplyDepartmentCode,
                  s.SEPERATETIME                       AS ArriveTime,
                  s.SEPERATETIME                       AS RegisterTime,
                  s.OPERATORID                        AS CheckInDoctorCode,
                  (SELECT USERNAME FROM PACS31.PACSUSER WHERE USERID=s.OPERATORID) AS CheckInDoctorName,
                  r.REPORTDESCRIBE                    AS Findings,
                  r.REPORTDIAGNOSE                    AS Impression,
                  r.OPERATETIME                        AS SubmitDateTime,
                  r.DOCID1                            AS SubmitDoctorCode,
                  (SELECT USERNAME FROM PACS31.PACSUSER WHERE USERID=r.DOCID1) AS SubmitDoctorName,
                  r.OPERATORID                        AS ApproveDoctorCode,
                  (SELECT USERNAME FROM PACS31.PACSUSER WHERE USERID=r.OPERATORID) AS ApproveDoctorName,
                  psi.IMAGECOUNT                      AS ImageCount,
                  s.STUDYSCRIPTION                     AS StudyScription,
                  s.BESPEAKTIME                        AS ScheduledDate,
                  s.QUEUENO                            AS QueueNo
                FROM PACS31.STUDYINFO s
                LEFT JOIN PACS31.PATIENTINFO p           ON s.CHECKSERIALNUM = p.CHECKSERIALNUM
                LEFT JOIN PACS31.PATIENTDIAGRPTINFO r    ON s.DIAGRPTID = r.DIAGRPTID
                LEFT JOIN PACS31.DEVICETYPEINFO dt       ON s.DEVICETYPEID = dt.DevicetypeId
                LEFT JOIN PACS31.PACS_STUDYINFO psi      ON s.CHECKSERIALNUM = psi.ACCESSIONNUMBER
                WHERE s.SEPERATETIME BETWEEN :start AND :end
                  AND s.ISAVAILABLE = 1
                ORDER BY s.SEPERATETIME
                OFFSET :offset ROWS FETCH NEXT :limit ROWS ONLY",
                new { start = range.Start, end = range.End, offset, limit = pageSize })).AsList();

            hasMore = rows.Count == pageSize;
            offset += pageSize;

            foreach (var row in rows)
            {
                if (string.IsNullOrWhiteSpace(row.AccessionNumber))
                    continue;
                yield return BuildArchive(row);
            }
        }
    }

    // ─────── OracleRecord → MigrateArchive 映射 ───────

    /// <summary>将 Oracle 查询行转换为迁移用的 Archive 对象</summary>
    private MigrateArchive BuildArchive(OracleRecord r)
    {
        var (age, ageUnit, ageDisplay) = ParseAge(r.Age, r.AgeUnit);

        var patient = new PatientArchive
        {
            Name = r.Name ?? "",
            SpellName = r.SpellName,
            Gender = r.Gender ?? "U",   // M=男/F=女/U=未知
            Age = age,
            AgeUnit = ageUnit,          // year/month/day/hour
            AgeDisplay = ageDisplay,    // 如 "35岁"
            GlobalPatientId = r.GlobalPatientId,
            HisPatientId = r.GlobalPatientId,
            PatientIndex = r.GlobalPatientId != null ? $"HIS/{r.GlobalPatientId}" : null,
            DateOfBirth = r.DateOfBirth,
            IDCardType = "01",          // 01=身份证
            Address = r.Address,
            Telephone = r.Telephone
        };

        var visit = new VisitArchive
        {
            ClinicalNumber = r.ClinicalNumber,
            InpatientNumber = r.InpatientNumber,
            PatientType = r.PatientType ?? "OP",   // OP=门诊/IH=住院
            BedNumber = r.BedNumber,
            EmergencyDegree = r.EmergencyDegree     // 1=急诊/3=普通
        };
        // 根据 ModalityCode 查找对应的执行科室（如 CT → 放射科）
        string modalityCode = r.ModalityCode ?? "";
        string modalityName = modalityCode;
        string? execDeptCode = null;
        string? execDeptName = null;

        if (!string.IsNullOrWhiteSpace(modalityCode) && _modalityMap.TryGetValue(modalityCode, out var dept))
        {
            execDeptCode = dept.ExecDepartmentCode;
            execDeptName = dept.ExecDepartmentName;
        }

        var order = new OrderArchive
        {
            HisOrderCodes = !string.IsNullOrWhiteSpace(r.HisOrderCode) ? new[] { r.HisOrderCode } : Array.Empty<string>(),
            AccessionNumber = r.AccessionNumber ?? "",
            RegisterTime = r.RegisterTime,
            ModalityCode = modalityCode,
            ModalityName = modalityName,
            ExecDepartmentCode = execDeptCode,
            ExecDepartmentName = execDeptName,
            ApplyDepartmentCode = r.ApplyDepartmentCode,
            ApplyDepartmentName = null,
            ApplyDoctorCode = null,
            ApplyDoctorName = null,
            CheckInDoctorCode = r.CheckInDoctorCode,
            CheckInDoctorName = r.CheckInDoctorName,
            CheckInTime = r.ArriveTime,
            TotalFee = ParseFee(r.TotalFee) ?? 0,
            Status = DetermineStatus(r),   // Arrived=已到检/Studyed=有影像/Reported=有报告
            IsMatch = !string.IsNullOrWhiteSpace(r.StudyInstanceUID),
            IsFromRIS = !string.IsNullOrWhiteSpace(r.HisOrderCode),
            DeviceCode = r.DeviceCode,
            DeviceName = r.DeviceCode,
            QueueNumber = r.QueueNo?.ToString(),
            Procedures = new[]
            {
                new ProcedureInfo
                {
                    BodyPartName = null,
                    BodyPartCode = null,
                    CheckItemCode = null,
                    CheckItemName = null,
                    CheckFee = ParseFee(r.TotalFee) ?? 0
                }
            }
        };

        var study = new StudyArchive
        {
            StudyDate = r.StudyDate,
            InsertionTime = r.StudyDate,
            StudyInstanceUIDs = !string.IsNullOrWhiteSpace(r.StudyInstanceUID)
                ? new[] { r.StudyInstanceUID }
                : Array.Empty<string>(),
            StorageNode = _identity.StorageNode,
            ImageCount = r.ImageCount != null && int.TryParse(r.ImageCount, out var imgCount) ? imgCount : null,
            StudyDescription = r.StudyScription
        };

        var report = new ReportArchive
        {
            SubmitTime = r.SubmitDateTime,
            SubmitDoctorCode = r.SubmitDoctorCode,
            SubmitDoctorName = r.SubmitDoctorName,
            ApproveTime = r.SubmitDateTime,
            ApproveDoctorCode = r.ApproveDoctorCode,
            ApproveDoctorName = r.ApproveDoctorName,
            Findings = r.Findings,
            Impression = r.Impression,
            PdfLink = "report://auto-rendering",
            Status = "Verify",
            FullStatus = "Verify"
        };

        // Id = MD5(AccessionNumber) 保证同一检查号生成相同 GUID，用于 MongoDB _id
        var schedule = r.ScheduledDate.HasValue
            ? new ScheduleArchive { StartTime = r.ScheduledDate }
            : null;

        return new MigrateArchive
        {
            Id = IdGenerator.FromString(r.AccessionNumber ?? Guid.NewGuid().ToString()),
            ArchiveType = "Migrate",
            ServerNode = _identity.ServerNode,
            HospitalCode = _identity.HospitalCode,
            HospitalName = _identity.HospitalName,
            Patient = patient,
            Visit = visit,
            Order = order,
            Study = study,
            Report = report,
            Schedule = schedule
        };
    }

    // ─────── 辅助方法 ───────

    /// <summary>
    /// 解析 Oracle 年龄字段（数字+单位）→ (数值, 单位代码, 显示字符串)。
    /// 单位映射：年→year / 月→month / 天→day / 小时→hour
    /// </summary>
    private static (int?, string?, string?) ParseAge(string? age, string? ageUnit)
    {
        if (string.IsNullOrWhiteSpace(age) || !int.TryParse(age, out var ageVal))
            return (null, null, null);

        string unitCode = ageUnit switch
        {
            "年" => "year",
            "月" => "month",
            "天" => "day",
            "小时" => "hour",
            _ => "year"
        };
        string display = $"{ageVal}{ageUnit ?? "岁"}";
        return (ageVal, unitCode, display);
    }

    /// <summary>
    /// 根据报告/影像状态确定检查状态：
    ///   Arrived = 已到检但尚未做检查
    ///   Studyed = 已有影像（StudyInstanceUID 非空）
    ///   Reported = 已出报告（SubmitDoctorCode 非空）
    /// </summary>
    private static string DetermineStatus(OracleRecord r)
    {
        if (!string.IsNullOrWhiteSpace(r.SubmitDoctorCode))
            return "Reported";
        if (!string.IsNullOrWhiteSpace(r.StudyInstanceUID))
            return "Studyed";
        return "Arrived";
    }

    private static double? ParseFee(string? fee)
    {
        if (string.IsNullOrWhiteSpace(fee))
            return null;
        if (double.TryParse(fee, out var val))
            return val;
        return null;
    }
}
