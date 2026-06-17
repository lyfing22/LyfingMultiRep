namespace DataMigrate.Sources.Neusoft;

public class NeusoftRecord
{
    public string? GlobalPatientId { get; init; }
    /// <summary>社保号</summary>
    public string SocietyNumber { get; set; }
    public string? Name { get; init; }
    public string? SpellName { get; init; }
    public string? Gender { get; init; }
    public DateTime? DateOfBirth { get; init; }
    public string? Address { get; init; }
    public string? Telephone { get; init; }
    public string Email { get; set; }
    public string IDCard { get; set; }
    public string AccessionNumber { get; init; }
    public string? BedNumber { get; init; }
    public string RoomNumber { get; set; }
    public string? PatientType { get; init; }
    public string? EmergencyDegree { get; init; }
    public string? ClinicalNumber { get; init; }
    public string? InpatientNumber { get; init; }
    public string ClinicalDiagnosis { get; set; }
    public string DiseaseHistory { get; set; }
    public string Sign { get; set; }
    public string HisExamName { get; set; }

    /// <summary>HIS 申请单申请日期</summary>
    public DateTime? HisRequestDate { get; set; }
    public string? HisOrderCode { get; init; }
    public string? TotalFee { get; init; }
    public string? Age { get; init; }
    public string? AgeUnit { get; init; }
    public string? StudyInstanceUID { get; init; }
    public DateTime? StudyDate { get; init; }
    public string? ExecDepartmentCode { get; init; }
    public string ExecDoctorCode { get; set; }
    public string ExecDoctorName { get; set; }
    public string? ModalityCode { get; init; }
    public string? DeviceCode { get; init; }
    public string? ApplyDepartmentCode { get; init; }
    public string ApplyDoctorCode { get; set; }
    public string ApplyDoctorName { get; set; }
    public DateTime? ArriveTime { get; init; }
    public DateTime? RegisterTime { get; init; }
    public string? CheckInDoctorCode { get; init; }
    public string? CheckInDoctorName { get; init; }
    public string? Findings { get; init; }
    public string? Impression { get; init; }
    public int? PositiveStatus { get; set; }
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
