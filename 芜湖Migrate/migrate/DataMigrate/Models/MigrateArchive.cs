using System.Text.Json;
using System.Text.Json.Serialization;

namespace DataMigrate.Models;

/// <summary>共享的 JsonSerializerOptions 配置：camelCase + 忽略 null</summary>
public static class ArchiveJson
{
    /// <summary>默认序列化配置（null 属性不输出、驼峰命名）</summary>
    public static readonly JsonSerializerOptions Serialize = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>带缩进的格式化版本，用于 debug 模式打印 JSON</summary>
    public static readonly JsonSerializerOptions PrettyPrint = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };
}

/// <summary>上传到 MIIS 的归档数据根对象。Id = MD5(AccessionNumber) 实现幂等写入</summary>
public class MigrateArchive
{
    public Guid Id { get; set; }
    public Guid? ReportId { get; set; }
    public string ArchiveId { get; set; }
    public string ArchiveType { get; set; } = "Migrate";
    public PatientArchive Patient { get; set; }
    public VisitArchive Visit { get; set; }
    public OrderArchive Order { get; set; }
    public ReportArchive Report { get; set; }
    public StudyArchive Study { get; set; }
    public ScheduleArchive Schedule { get; set; }
    public AssistInfo DiagAssist { get; set; }
    public bool HasReqAssist { get; set; }
    public ExArchiveData ExData { get; set; }
    public string ServerNode { get; set; }
    public string HospitalCode { get; set; }
    public string HospitalName { get; set; }
    public DateTime? LastUpdateTime { get; set; }
}

/// <summary>患者基本信息</summary>
public class PatientArchive
{
    public string Name { get; set; }
    public string SpellName { get; set; }
    public string BriefSpellName { get; set; }
    public string Gender { get; set; }
    public int? Age { get; set; }
    public string AgeUnit { get; set; }
    public string AgeDisplay { get; set; }
    public string GlobalPatientId { get; set; }
    public string HisPatientId { get; set; }
    public string PatientIndex { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string IDCardType { get; set; } = "01";
    public string IDCard { get; set; }
    public string Address { get; set; }
    public string Telephone { get; set; }
    public string Nation { get; set; }
    public string Marriage { get; set; }
    public string SocietyNumber { get; set; }
    public string Note { get; set; }
    public string Email { get; set; }
}

/// <summary>就诊信息</summary>
public class VisitArchive
{
    public bool IsVIP { get; set; }
    public Guid? VipReportDoctorId { get; set; }
    public string ClinicalNumber { get; set; }
    public string InpatientNumber { get; set; }
    public string MedicalRecordNumber { get; set; }
    public string VisitSerialNumber { get; set; }
    public string PatientType { get; set; }
    public string Critical { get; set; }
    public string EmergencyDegree { get; set; }
    public string DoctorAdviceType { get; set; }
    public string BedNumber { get; set; }
    public string RoomNumber { get; set; }
    public string ClinicalDiagnosis { get; set; }
    public string AllergyHistory { get; set; }
    public string VisitNumber { get; set; }
    public string PastHistory { get; set; }
    public string Symptom { get; set; }
    public string Sign { get; set; }
    public string LaboratoryReport { get; set; }
    public string OtherDiagnosis { get; set; }
    public string DiseaseHistory { get; set; }
    public string PatientIdentity { get; set; }
    public string ChargeType { get; set; }
    public bool IsSpecialPatient { get; set; }
    public string InfectionResult { get; set; }
    public string HpTestResult { get; set; }
    public string AdditionalInfo { get; set; }
    public Dictionary<string, string> ExData { get; set; }
}

/// <summary>检查医嘱信息（核心业务对象）</summary>
public class OrderArchive
{
    public string[] HisOrderCodes { get; set; }
    public DateTime? HisRequestDate { get; set; }
    public string HisExamName { get; set; }
    public string InHospitalCount { get; set; }
    public string AccessionNumber { get; set; }
    public string DeviceCode { get; set; }
    public string DeviceName { get; set; }
    public string CallerStatus { get; set; }
    public string DeviceAETitle { get; set; }
    public string ExamRoom { get; set; }
    public string DeviceAddress { get; set; }
    public string ExecDepartmentCode { get; set; }
    public string ExecDepartmentName { get; set; }
    public string ExecDoctorCode { get; set; }
    public string ExecDoctorName { get; set; }
    public string ModalityCode { get; set; }
    public string ModalityName { get; set; }
    public string[] CheckTypes { get; set; }
    public ChargeItem[] ChargeItems { get; set; } = [];
    public ProcedureInfo[] Procedures { get; set; } = [];
    public CannulateInfo Cannulate { get; set; } = new CannulateInfo();
    public bool IsUseMedicine { get; set; }
    public bool IsAdverseReactions { get; set; }
    public double TotalFee { get; set; }
    public string Note { get; set; }
    public string Suggestion { get; set; }
    public string SpecialRequire { get; set; }
    public bool OrderSpecialClass { get; set; }
    public Guid? DiagnoseGroupId { get; set; }
    public string DiagnoseGroupName { get; set; }
    public DateTime? RegisterTime { get; set; }
    public string RegisterDoctorCode { get; set; }
    public string RegisterDoctorName { get; set; }
    public string ApplyDoctorCode { get; set; }
    public string ApplyDoctorName { get; set; }
    public string ApplyDepartmentCode { get; set; }
    public string ApplyDepartmentName { get; set; }
    public DateTime? CheckInTime { get; set; }
    public string CheckInDoctorCode { get; set; }
    public string CheckInDoctorName { get; set; }
    public bool ExamReadyConfirmed { get; set; }
    public bool ExamFinishConfirmed { get; set; }
    public DateTime? CancelTime { get; set; }
    public string CancelDoctorCode { get; set; }
    public string CancelDoctorName { get; set; }
    public string FilmingRank { get; set; }
    public string FilmingFailedReason { get; set; }
    public bool IsMatch { get; set; }
    public bool IsFromRIS { get; set; }
    public string QueueNumber { get; set; }
    public string[] PaperOrderLinks { get; set; }
    public Dictionary<string, string> ExData { get; set; }
    public string Status { get; set; }
    public string Comments { get; set; }
    public bool NeedPrintFilm { get; set; }
}

/// <summary>检查项目明细</summary>
public class ProcedureInfo
{
    public string CheckItemCode { get; set; }
    public string CheckItemName { get; set; }
    public string Group { get; set; }
    public Guid? BodyPartId { get; set; }
    public string BodyPartCode { get; set; }
    public string BodyPartName { get; set; }
    public double CheckFee { get; set; }
    public string CheckType { get; set; }
    public double? Weights { get; set; }
    public double MaterialFee { get; set; }
    public int FilmAmount { get; set; }
    public string FilmSpecName { get; set; }
    public double FilmFee { get; set; }
    public int ImageAmount { get; set; }
    public int ExposalAmount { get; set; }
    public int TimeCount { get; set; }
    public string ExamTechnique { get; set; }
}

/// <summary>收费项目信息</summary>
public class ChargeItem
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string Specs { get; set; }
    public int Quantity { get; set; }
    public string Unit { get; set; }
    public double Amount { get; set; }
    public string Category { get; set; }
}

/// <summary>置管信息</summary>
public class CannulateInfo
{
    public Guid? NurseId { get; set; }
    public DateTime? CannulateTime { get; set; }
}

/// <summary>影像检查信息</summary>
public class StudyArchive
{
    public int? ImageCount { get; set; }
    public bool IsPrinted { get; set; }
    public DateTime? InsertionTime { get; set; }
    public DateTime? LastImageDateTime { get; set; }
    public DateTime? LastBuildDateTime { get; set; }
    public int? Is3D { get; set; }
    public string StudyDescription { get; set; }
    public string SourceInstitutionCode { get; set; }
    public DateTime? StudyDate { get; set; }
    public string[] StudyInstanceUIDs { get; set; }
    public string StorageNode { get; set; }
    public int? PrintStatus { get; set; }
    public int? FilmCount { get; set; }
}

/// <summary>报告信息</summary>
public class ReportArchive
{
    public DateTime? SubmitTime { get; set; }
    public string SubmitDoctorCode { get; set; }
    public string SubmitDoctorName { get; set; }
    public DateTime? ApproveTime { get; set; }
    public string ApproveDoctorCode { get; set; }
    public string ApproveDoctorName { get; set; }
    public string InternDoctorCode { get; set; }
    public string InternDoctorName { get; set; }
    public string AssignedReportDoctor { get; set; }
    public string AssignedApproveDoctor { get; set; }
    public string ReportForwarder { get; set; }
    public string PrintDoctor { get; set; }
    public DateTime? PrintTime { get; set; }
    public string Findings { get; set; }
    public string Impression { get; set; }
    public string TechParams { get; set; }
    public int? PositiveStatus { get; set; }
    public string ReportRank { get; set; }
    public string Consistency { get; set; }
    public string PdfLink { get; set; }
    public string Accord { get; set; }
    public bool? IsCritical { get; set; }
    public string CriticalType { get; set; }
    public string[] CaseDiscussFlags { get; set; }
    public bool? IsReApproved { get; set; }
    public Guid? ImageReaderId { get; set; }
    public string ImageReaderName { get; set; }
    public CriticalNotify CriticalNotify { get; set; } = new CriticalNotify();
    public string[] Flags { get; set; }
    public string Status { get; set; }
    public string FullStatus { get; set; }
    public string ICDCode { get; set; }
    public string ACRCode { get; set; }
    public string ReportMetaData { get; set; }
}

/// <summary>危急值通知信息</summary>
public class CriticalNotify
{
    public string SendDoctor { get; set; }
    public DateTime? SendTime { get; set; }
    public CriticalRecvInfo RecvInfo { get; set; } = new CriticalRecvInfo();
}

/// <summary>危急值接收信息</summary>
public class CriticalRecvInfo
{
    public string RecvDoctor { get; set; }
    public string RecvDepartment { get; set; }
    public DateTime? RecvTime { get; set; }
    public string HandleAction { get; set; }
}

/// <summary>预约信息</summary>
public class ScheduleArchive
{
    public string ScheduleDoctorCode { get; set; }
    public string ScheduleDoctorName { get; set; }
    public DateTime? ScheduleTime { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
}

/// <summary>协助信息</summary>
public class AssistInfo
{
    public string Description { get; set; }
    public DateTime CreateTime { get; set; }
    public string[] TargetHospitals { get; set; }
    public bool IsLocked { get; set; }
    public string Locker { get; set; }
    public Guid? PrintTemplateId { get; set; }
    public string AssistHospitalCode { get; set; }
    public string AssistHospitalName { get; set; }
    public ReqPrintTemplate[] PrintTemplates { get; set; }
}

public class ReqPrintTemplate
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}

/// <summary>扩展的档案数据（仅定义模型供序列化用）</summary>
public class ExArchiveData
{
    public string CheckItemSort { get; set; }
    public string BodyPartSort { get; set; }
    public string FindingSort { get; set; }
    public string ImpressionSort { get; set; }
    public string FullStatus { get; set; }
}

/// <summary>设备类型→执行科室映射（从 appsettings.ModalityDepartment 反序列化）</summary>
public record DeviceDptInfo(string ExecDepartmentCode, string ExecDepartmentName, string? DeviceCode, string? DeviceName, string? ExamRoom);

/// <summary>ZTemp_MigratePlan 记录映射（仅用于分页查找）</summary>
public class MigratePlanRecord
{
    public Guid Id { get; set; }
    public DateTime TimeRangeStart { get; set; }
    public DateTime TimeRangeEnd { get; set; }
}

/// <summary>包含 PDF 报告的档案上传参数，对应 ArchiveController.SaveWithPdfReport</summary>
public record ArchiveUploadPara(MigrateArchive Data, string PdfReport);
