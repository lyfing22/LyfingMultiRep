using System.Data;
using System.Runtime.CompilerServices;
using Dapper;
using DataMigrate.Core;
using DataMigrate.Infrastructure;
using DataMigrate.Models;
using Oracle.ManagedDataAccess.Client;

namespace DataMigrate.Sources.Neusoft;

/// <summary>
/// 东软PACS数据源适配器：连接Oracle数据库，查询STUDYINFO等表构建迁移档案对象
/// </summary>
public class NeusoftSource : IMigrationSource
{
    private readonly string _connStr;
    private readonly IdentityConfig _identity;
    private readonly Dictionary<string, DeviceDptInfo> _modalityMap;

    public string SourceName => "Neusoft PACS";

    public NeusoftSource(SourceDependencies deps)
    {
        _connStr = deps.ConnectionString;
        _identity = deps.Identity;
        _modalityMap = deps.ModalityDepartment;
    }

    public async Task ValidateAsync()
    {
        using var conn = new OracleConnection(_connStr);
        await conn.OpenAsync();
        await conn.QuerySingleAsync<int>("SELECT 1 FROM DUAL");
    }

    public async Task<MigrateArchive?> GetByAccessionNumberAsync(string accessionNumber)
    {
        using var conn = new OracleConnection(_connStr);
        await conn.OpenAsync();

        var row = await conn.QuerySingleOrDefaultAsync<NeusoftRecord>(@"
            SELECT
              p.ENETPATIENTID                          AS GlobalPatientId,
              p.PATIENTNAME                       AS Name,
              p.PatientSpellName                  AS SpellName,
              CASE WHEN p.SEX='男' THEN 'M'
                   WHEN p.SEX='女' THEN 'F'
                   ELSE 'U' END                   AS Gender,
              p.BIRTHDAY                           AS DateOfBirth,
              s.AGE,
              s.AGEUNIT,
              p.Address,
              p.Phonenumber                       AS Telephone,
              p.IDNUMBER 						  AS IDCard,
              p.SOCIETYID 							AS SocietyNumber,
              s.CHECKSERIALNUM                    AS AccessionNumber,
              s.SICKROOM                          AS BedNumber,
              s.SICKBED                          AS BedNumber,
             CASE WHEN p.hispatienttype='1' THEN 'OP'      
               WHEN p.hispatienttype='2' THEN 'IH'
               WHEN p.hispatienttype='3' THEN 'PE'
               WHEN p.hispatienttype='5' THEN 'OP' --旧门诊
               WHEN p.hispatienttype='6' THEN 'PE' --旧住院
               WHEN p.hispatienttype='10' THEN 'PE' --体检/随访
               ELSE 'OP' END                   AS PatientType,
              CASE WHEN s.IFEMERGENCY=1 THEN '1' ELSE '3' END AS EmergencyDegree,
              p.clinicpatientid                   AS ClinicalNumber,
              p.infeepatientid                    AS InpatientNumber,
              s.DIAGID                            AS HisOrderCode,
             s.PREDIAGNOSE                       AS ClinicalDiagnosis,
             s.ABSTRACTHISTORY					  AS    DiseaseHistory,
             s.DOCTORCODE 						  AS  ApplyDoctorName,
            dpt.DEPARTMENTCODE				  AS ApplyDepartmentCode,
            dpt.DEPARTMENTNAME				  AS ApplyDepartmentName,
            s.ABSTRACTHISTORY					  AS [Sign],
            s.HISCHECKITEM   					AS HisExamName,
              s.SEPERATETIME                       AS ArriveTime,
              s.SEPERATETIME                       AS RegisterTime,
              s.OPERATORID                        AS CheckInDoctorCode,    
              s.FEETOTAL                          AS TotalFee,             
              s.DSTUDYUID                         AS StudyInstanceUID,
              s.STUDYTIME                          AS StudyDate,
             s.PHOTOMAKERID                      AS  ExecDoctorCode,--检查技师ID
              s.CHKDEPTID                         AS ExecDepartmentCode,
              s.FILENUM    						  AS  ImageCount,  --todo
              dt.Modality                         AS ModalityCode,
              s.DEVICEID                          AS DeviceCode,        
              (SELECT USERNAME FROM PACS31.PACSUSER WHERE USERID=s.OPERATORID) AS CheckInDoctorName,
         CASE WHEN s.IFMASCULINE='1' THEN '0'      -- 是否阳性，0：未确定，1：阴性，2：阳性 ----->/// -1:未知0:阴性 1:阳性 
               WHEN s.IFMASCULINE='2' THEN '1'
               ELSE '-1' END                   AS PositiveStatus,
              r.REPORTDESCRIBE                    AS Findings,
              r.REPORTDIAGNOSE                    AS Impression,
              r.OPERATETIME                        AS SubmitDateTime,
              r.DOCID1                            AS SubmitDoctorCode,
              (SELECT USERNAME FROM PACS31.PACSUSER WHERE USERID=r.DOCID1) AS SubmitDoctorName,
              r.OPERATORID                        AS ApproveDoctorCode,
              (SELECT USERNAME FROM PACS31.PACSUSER WHERE USERID=r.OPERATORID) AS ApproveDoctorName,
              psi.IMAGECOUNT                      AS ImageCount, --todo
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

    public async Task<DateRange> GetTimeRangeAsync()
    {
        using var conn = new OracleConnection(_connStr);
        await conn.OpenAsync();

        var row = await conn.QuerySingleAsync(@"
            SELECT MIN(s.SEPERATETIME) AS MinDate, MAX(s.SEPERATETIME) AS MaxDate
            FROM PACS31.STUDYINFO s
            WHERE s.ISAVAILABLE = 1");

        DateTime? min = row.MinDate;
        DateTime? max = row.MaxDate;

        return min.HasValue && max.HasValue
            ? new DateRange(min.Value, max.Value)
            : throw new InvalidOperationException("源数据库 PACS31.STUDYINFO 中无有效数据");
    }

    public async Task<SourceMetadata> GetMetadataAsync(DateRange range)
    {
        using var conn = new OracleConnection(_connStr);
        await conn.OpenAsync();

        var count = await conn.QuerySingleAsync<int>(@"
            SELECT COUNT(*)
            FROM PACS31.STUDYINFO s
            WHERE s.SEPERATETIME >= :start AND s.SEPERATETIME < :end
              AND s.ISAVAILABLE = 1", new { start = range.Start, end = range.End });

        return new SourceMetadata(count, range.Start, range.End);
    }

    public async IAsyncEnumerable<MigrateArchive> EnumerateArchivesAsync(
        DateRange range, [EnumeratorCancellation] CancellationToken ct)
    {
        using var conn = new OracleConnection(_connStr);
        await conn.OpenAsync();

        var rows = (await conn.QueryAsync<NeusoftRecord>(@"
            SELECT
              s.STUDYID                          AS GlobalPatientId,
              p.PATIENTNAME                       AS Name,
              p.PatientSpellName                  AS SpellName,
              CASE WHEN p.SEX='男' THEN 'M'
                   WHEN p.SEX='女' THEN 'F'
                   ELSE 'U' END                   AS Gender,
              p.BIRTHDAY                           AS DateOfBirth,
              p.Address,
              p.Phonenumber                       AS Telephone,
              s.CHECKSERIALNUM                    AS AccessionNumber,
              s.SICKROOM                          AS BedNumber,
              DECODE(p.hispatienttype,'1','OP','2','IH','OP') AS PatientType,
              CASE WHEN s.IFEMERGENCY=1 THEN '1' ELSE '3' END AS EmergencyDegree,
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
            WHERE s.SEPERATETIME >= :start AND s.SEPERATETIME < :end
              AND s.ISAVAILABLE = 1
            ORDER BY s.SEPERATETIME",
            new { start = range.Start, end = range.End })).AsList();

        foreach (var row in rows)
        {
            if (string.IsNullOrWhiteSpace(row.AccessionNumber))
                continue;
            yield return BuildArchive(row);
        }
    }

    private MigrateArchive BuildArchive(NeusoftRecord r)
    {
        bool IsFromRIS = !string.IsNullOrWhiteSpace(r.HisOrderCode);
        var (age, ageUnit, ageDisplay) = ParseAge(r.Age, r.AgeUnit);
        var patient = new PatientArchive
        {
            Name = r.Name,
            SpellName = r.SpellName,
            Gender = r.Gender ?? "U",
            Age = age,
            AgeUnit = ageUnit,
            AgeDisplay = ageDisplay,
            GlobalPatientId = r.GlobalPatientId,
            HisPatientId = r.GlobalPatientId,
            PatientIndex = IsFromRIS ? $"HIS/{r.GlobalPatientId}" : $"Migrate/{r.GlobalPatientId}",
            DateOfBirth = r.DateOfBirth,
            IDCardType = "01",
            IDCard = r.IDCard,
            Address = r.Address,
            Telephone = r.Telephone
        };
        var visit = new VisitArchive
        {
            ClinicalNumber = r.ClinicalNumber,
            InpatientNumber = r.InpatientNumber,
            VisitSerialNumber = r.AccessionNumber,
            PatientType = r.PatientType,
            BedNumber = r.BedNumber,
            RoomNumber = r.RoomNumber,
            EmergencyDegree = r.EmergencyDegree,
            Critical = "",
            ClinicalDiagnosis = r.ClinicalDiagnosis,
            DiseaseHistory = r.DiseaseHistory,
            Sign = r.Sign,
        };

        string modalityCode = r.ModalityCode;
        string? execDeptCode = null;
        string? execDeptName = null;

        if (!string.IsNullOrWhiteSpace(modalityCode) && _modalityMap.TryGetValue(modalityCode, out var dept))
        {
            execDeptCode = dept.ExecDepartmentCode;
            execDeptName = dept.ExecDepartmentName;
        }

        var order = new OrderArchive
        {
            HisExamName = r.HisExamName,
            HisOrderCodes = !string.IsNullOrWhiteSpace(r.HisOrderCode) ? new[] { r.HisOrderCode } : Array.Empty<string>(),
            AccessionNumber = r.AccessionNumber ?? "",
            RegisterTime = r.RegisterTime,
            ModalityCode = modalityCode,
            ModalityName = modalityCode,
            ExecDepartmentCode = execDeptCode,
            ExecDepartmentName = execDeptName,
            ExecDoctorCode = r.ExecDoctorCode,
            ExecDoctorName = r.ExecDoctorName,
            ApplyDepartmentCode = r.ApplyDepartmentCode,
            ApplyDepartmentName = r.ApproveDoctorName,
            ApplyDoctorCode = r.ApplyDoctorCode,
            ApplyDoctorName = r.ApplyDoctorName,
            CheckInDoctorCode = r.CheckInDoctorCode,
            CheckInDoctorName = r.CheckInDoctorName,
            CheckInTime = r.ArriveTime,
            TotalFee = ParseFee(r.TotalFee) ?? 0,
            Status = DetermineStatus(r),
            IsMatch = !string.IsNullOrWhiteSpace(r.StudyInstanceUID),
            IsFromRIS = IsFromRIS,
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
            PositiveStatus = r.PositiveStatus,
            PdfLink = "report://auto-rendering",
            Status = "Verify",
            FullStatus = "Verify"
        };

        var schedule = r.ScheduledDate.HasValue
            ? new ScheduleArchive { StartTime = r.ScheduledDate }
            : null;

        return new MigrateArchive
        {
            Id = IdGenerator.FromString(order.AccessionNumber),
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

    private static string DetermineStatus(NeusoftRecord r)
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
