namespace DataMigrate.Sources.Oracle;

/// <summary>
/// Oracle 联表查询的 DTO。属性名必须与 SQL 中 AS 别名完全一致（大小写不敏感），
/// 否则 Dapper 无法映射到字段。
/// </summary>
public class OracleRecord
{
    public string? GlobalPatientId { get; init; }
    public string? Name { get; init; }
    public string? SpellName { get; init; }
    public string? Gender { get; init; }
    public DateTime? DateOfBirth { get; init; }
    public string? Address { get; init; }
    public string? Telephone { get; init; }
    public string? AccessionNumber { get; init; }
    public string? BedNumber { get; init; }
    public string? PatientType { get; init; }
    public string? EmergencyDegree { get; init; }
    public string? ClinicalNumber { get; init; }
    public string? InpatientNumber { get; init; }
    public string? HisOrderCode { get; init; }
    public string? TotalFee { get; init; }
    public string? Age { get; init; }
    public string? AgeUnit { get; init; }
    public string? StudyInstanceUID { get; init; }
    public DateTime? StudyDate { get; init; }
    public string? ExecDepartmentCode { get; init; }
    public string? ModalityCode { get; init; }
    public string? DeviceCode { get; init; }
    public string? ApplyDepartmentCode { get; init; }
    public DateTime? ArriveTime { get; init; }
    public DateTime? RegisterTime { get; init; }
    public string? CheckInDoctorCode { get; init; }
    public string? CheckInDoctorName { get; init; }
    public string? Findings { get; init; }
    public string? Impression { get; init; }
    public DateTime? SubmitDateTime { get; init; }
    public string? SubmitDoctorCode { get; init; }
    public string? SubmitDoctorName { get; init; }
    public string? ApproveDoctorCode { get; init; }
    public string? ApproveDoctorName { get; init; }
    public string? ImageCount { get; init; }
    public string? StudyScription { get; init; }
    public DateTime? ScheduledDate { get; init; }
    public int? QueueNo { get; init; }
}
