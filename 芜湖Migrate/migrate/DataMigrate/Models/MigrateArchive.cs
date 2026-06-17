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
    /// <summary>患者唯一标识（主键）</summary>
    public Guid Id { get; set; }

    /// <summary>只有RIS使用</summary>
    public Guid? ReportId { get; set; }

    /// <summary>对于归档数据，内部使用此Id</summary>
    public string ArchiveId { get; set; }

    /// <summary>归档数据类型，不能为空，不同的归档数据类型不一样</summary>
    public string ArchiveType { get; set; } = "Migrate";

    /// <summary>患者信息</summary>
    public PatientArchive Patient { get; set; }

    /// <summary>就诊信息</summary>
    public VisitArchive Visit { get; set; }

    /// <summary>申请单信息</summary>
    public OrderArchive Order { get; set; }

    /// <summary>报告信息</summary>
    public ReportArchive Report { get; set; }

    /// <summary>检查信息</summary>
    public StudyArchive Study { get; set; }

    /// <summary>预约信息（非必须，非预约段此字段为null）</summary>
    public ScheduleArchive Schedule { get; set; }

    /// <summary>请求协助信息</summary>
    public AssistInfo DiagAssist { get; set; }

    /// <summary>是否发送了请求协助</summary>
    public bool HasReqAssist { get; set; }

    /// <summary>扩展的归档信息，一般用于索引</summary>
    public ExArchiveData ExData { get; set; }

    /// <summary>是在区域中代表一个节点的标识</summary>
    public string ServerNode { get; set; }

    /// <summary>是在区域中代表一个医疗机构的标识</summary>
    public string HospitalCode { get; set; }

    /// <summary>医院名称</summary>
    public string HospitalName { get; set; }

    /// <summary>最后更新时间</summary>
    public DateTime? LastUpdateTime { get; set; }
}

/// <summary>患者归档模型</summary>
public class PatientArchive
{
    /// <summary>患者姓名</summary>
    public string Name { get; set; }

    /// <summary>拼音名</summary>
    public string SpellName { get; set; }

    /// <summary>简拼（拼音首字母）</summary>
    public string BriefSpellName { get; set; }

    /// <summary>患者性别，男为M，女为F</summary>
    public string Gender { get; set; }

    /// <summary>患者年龄（注：Age如果为空，请不传即不带此参数）</summary>
    public int? Age { get; set; }

    /// <summary>年龄单位，如岁为year，月为month，周为week，天为day</summary>
    public string AgeUnit { get; set; }

    /// <summary>显示年龄。第三方上传时有时是"三岁八个月"无法与Age和AgeUnit适配，当有显示年龄时优先使用</summary>
    public string AgeDisplay { get; set; }

    /// <summary>患者唯一编号（主要是展示用）</summary>
    public string GlobalPatientId { get; set; }

    /// <summary>His患者编号</summary>
    public string HisPatientId { get; set; }

    /// <summary>患者索引</summary>
    public string PatientIndex { get; set; }

    /// <summary>患者出生日期，格式为yyyy-MM-dd</summary>
    public DateTime? DateOfBirth { get; set; }

    /// <summary>民族</summary>
    public string Nation { get; set; }

    /// <summary>婚姻状况</summary>
    public string Marriage { get; set; }

    /// <summary>
    /// 身份证件类型
    /// 01：居民身份证  02：居民户口簿  03：护照  04：军官证
    /// 05：驾驶证  06：港澳居民来往内地通行证  07：台湾居民来往内地通行证  99：其他法定有效证件
    /// </summary>
    public string IDCardType { get; set; } = "01";

    /// <summary>身份证件号</summary>
    public string IDCard { get; set; }

    /// <summary>地址</summary>
    public string Address { get; set; }

    /// <summary>联系电话</summary>
    public string Telephone { get; set; }

    /// <summary>社保号</summary>
    public string SocietyNumber { get; set; }

    /// <summary>备注</summary>
    public string Note { get; set; }

    /// <summary>电子邮箱</summary>
    public string Email { get; set; }
}

/// <summary>就诊信息归档模型</summary>
public class VisitArchive
{
    /// <summary>是否是VIP报告</summary>
    public bool IsVIP { get; set; }

    /// <summary>VIP报告医生</summary>
    public Guid? VipReportDoctorId { get; set; }

    /// <summary>门诊号</summary>
    public string ClinicalNumber { get; set; }

    /// <summary>住院号</summary>
    public string InpatientNumber { get; set; }

    /// <summary>病案号</summary>
    public string MedicalRecordNumber { get; set; }

    /// <summary>就诊流水号</summary>
    public string VisitSerialNumber { get; set; }

    /// <summary>患者来源，住院为IH，门诊为OP，急诊为EM</summary>
    public string PatientType { get; set; }

    /// <summary>危重值Code</summary>
    public string Critical { get; set; }

    /// <summary>紧急程度（编码）</summary>
    public string EmergencyDegree { get; set; }

    /// <summary>医嘱类型（编码）</summary>
    public string DoctorAdviceType { get; set; }

    /// <summary>病床号</summary>
    public string BedNumber { get; set; }

    /// <summary>病房号</summary>
    public string RoomNumber { get; set; }

    /// <summary>临床诊断</summary>
    public string ClinicalDiagnosis { get; set; }

    /// <summary>过敏史</summary>
    public string AllergyHistory { get; set; }

    /// <summary>就诊号</summary>
    public string VisitNumber { get; set; }

    /// <summary>既往史</summary>
    public string PastHistory { get; set; }

    /// <summary>症状</summary>
    public string Symptom { get; set; }

    /// <summary>体征</summary>
    public string Sign { get; set; }

    /// <summary>化验结果</summary>
    public string LaboratoryReport { get; set; }

    /// <summary>其它诊断结果</summary>
    public string OtherDiagnosis { get; set; }

    /// <summary>病史</summary>
    public string DiseaseHistory { get; set; }

    /// <summary>患者身份，因为跟申请单相关，放在Visit里</summary>
    public string PatientIdentity { get; set; }

    /// <summary>费别类型（编码）</summary>
    public string ChargeType { get; set; }

    /// <summary>特殊患者标记</summary>
    public bool IsSpecialPatient { get; set; }

    /// <summary>传染病检查（编码）</summary>
    public string InfectionResult { get; set; }

    /// <summary>UBT尿素呼气试验结果（编码）</summary>
    public string HpTestResult { get; set; }

    /// <summary>附加信息（Json格式），可以为空，需要遵循规范</summary>
    public string AdditionalInfo { get; set; }

    /// <summary>扩展信息</summary>
    public Dictionary<string, string> ExData { get; set; }
}

/// <summary>申请单归档模型</summary>
public class OrderArchive
{
    /// <summary>HIS 申请单号</summary>
    public string[] HisOrderCodes { get; set; }

    /// <summary>HIS 申请单申请日期</summary>
    public DateTime? HisRequestDate { get; set; }

    /// <summary>HIS检查项目</summary>
    public string HisExamName { get; set; }

    /// <summary>住院次数（从HIS获取）</summary>
    public string InHospitalCount { get; set; }

    /// <summary>检查号</summary>
    public string AccessionNumber { get; set; }

    /// <summary>设备编号</summary>
    public string DeviceCode { get; set; }

    /// <summary>设备名称</summary>
    public string DeviceName { get; set; }

    /// <summary>叫号状态</summary>
    public string CallerStatus { get; set; }

    /// <summary>设备的AETitle</summary>
    public string DeviceAETitle { get; set; }

    /// <summary>检查室</summary>
    public string ExamRoom { get; set; }

    /// <summary>设备地址</summary>
    public string DeviceAddress { get; set; }

    /// <summary>执行科室编号</summary>
    public string ExecDepartmentCode { get; set; }

    /// <summary>执行科室名称</summary>
    public string ExecDepartmentName { get; set; }

    /// <summary>检查医生编码</summary>
    public string ExecDoctorCode { get; set; }

    /// <summary>检查医生姓名</summary>
    public string ExecDoctorName { get; set; }

    /// <summary>设备类型编号</summary>
    public string ModalityCode { get; set; }

    /// <summary>设备类型名称（study存在多设备类型，需要支持模糊检索）</summary>
    public string ModalityName { get; set; }

    /// <summary>检查类型</summary>
    public string[] CheckTypes { get; set; }

    /// <summary>收费项目详情</summary>
    public ChargeItem[] ChargeItems { get; set; } = [];

    /// <summary>检查项目详情</summary>
    public ProcedureInfo[] Procedures { get; set; } = [];

    /// <summary>置管信息</summary>
    public CannulateInfo Cannulate { get; set; } = new CannulateInfo();

    /// <summary>是否已用药</summary>
    public bool IsUseMedicine { get; set; }

    /// <summary>是否不良反应</summary>
    public bool IsAdverseReactions { get; set; }

    /// <summary>总费用（如为空，请传0）</summary>
    public double TotalFee { get; set; }

    /// <summary>备注</summary>
    public string Note { get; set; }

    /// <summary>建议</summary>
    public string Suggestion { get; set; }

    /// <summary>申请单检查前特殊要求</summary>
    public string SpecialRequire { get; set; }

    /// <summary>专项检查</summary>
    public bool OrderSpecialClass { get; set; }

    /// <summary>诊断组</summary>
    public Guid? DiagnoseGroupId { get; set; }

    /// <summary>诊断组名称</summary>
    public string DiagnoseGroupName { get; set; }

    /// <summary>登记时间</summary>
    public DateTime? RegisterTime { get; set; }

    /// <summary>登记医生编号</summary>
    public string RegisterDoctorCode { get; set; }

    /// <summary>登记医生姓名</summary>
    public string RegisterDoctorName { get; set; }

    /// <summary>申请科室编号</summary>
    public string ApplyDepartmentCode { get; set; }

    /// <summary>申请科室名称</summary>
    public string ApplyDepartmentName { get; set; }

    /// <summary>申请医生编号</summary>
    public string ApplyDoctorCode { get; set; }

    /// <summary>申请医生姓名</summary>
    public string ApplyDoctorName { get; set; }

    /// <summary>到检时间</summary>
    public DateTime? CheckInTime { get; set; }

    /// <summary>到检操作医生编号</summary>
    public string CheckInDoctorCode { get; set; }

    /// <summary>到检医生姓名</summary>
    public string CheckInDoctorName { get; set; }

    /// <summary>是否确认检查就绪</summary>
    public bool ExamReadyConfirmed { get; set; }

    /// <summary>是否确认检查完成</summary>
    public bool ExamFinishConfirmed { get; set; }

    /// <summary>取消时间</summary>
    public DateTime? CancelTime { get; set; }

    /// <summary>取消医生编码</summary>
    public string CancelDoctorCode { get; set; }

    /// <summary>取消医生姓名</summary>
    public string CancelDoctorName { get; set; }

    /// <summary>摄片等级</summary>
    public string FilmingRank { get; set; }

    /// <summary>摄片失败原因</summary>
    public string FilmingFailedReason { get; set; }

    /// <summary>是否已关联</summary>
    public bool IsMatch { get; set; }

    /// <summary>是否来自于ris的标记，指示是否可以在ris中编辑。对于迁移的数据此处也是填false</summary>
    public bool IsFromRIS { get; set; }

    /// <summary>排队号</summary>
    public string QueueNumber { get; set; }

    /// <summary>纸质申请单</summary>
    public string[] PaperOrderLinks { get; set; }

    /// <summary>扩展信息</summary>
    public Dictionary<string, string> ExData { get; set; }

    /// <summary>申请单状态</summary>
    public string Status { get; set; }

    /// <summary>留言信息</summary>
    public string Comments { get; set; }

    /// <summary>是否打印胶片</summary>
    public bool NeedPrintFilm { get; set; }
}

/// <summary>检查项目明细</summary>
public class ProcedureInfo
{
    /// <summary>检查项目编码</summary>
    public string CheckItemCode { get; set; }

    /// <summary>检查项目名称</summary>
    public string CheckItemName { get; set; }

    /// <summary>检查项目分组</summary>
    public string Group { get; set; }

    /// <summary>检查部位Id（Pacs查询WorkList使用）</summary>
    public Guid? BodyPartId { get; set; }

    /// <summary>检查部位编码</summary>
    public string BodyPartCode { get; set; }

    /// <summary>检查部位名称</summary>
    public string BodyPartName { get; set; }

    /// <summary>检查费用</summary>
    public double CheckFee { get; set; }

    /// <summary>检查类型</summary>
    public string CheckType { get; set; }

    /// <summary>权重</summary>
    public double? Weights { get; set; }

    /// <summary>材料费用</summary>
    public double MaterialFee { get; set; }

    /// <summary>胶片数量</summary>
    public int FilmAmount { get; set; }

    /// <summary>胶片规格（名称）</summary>
    public string FilmSpecName { get; set; }

    /// <summary>胶片费用</summary>
    public double FilmFee { get; set; }

    /// <summary>图像数量（统计使用）</summary>
    public int ImageAmount { get; set; }

    /// <summary>曝光次数（统计使用）</summary>
    public int ExposalAmount { get; set; }

    /// <summary>检查时长</summary>
    public int TimeCount { get; set; }

    /// <summary>检查技术</summary>
    public string ExamTechnique { get; set; }
}

/// <summary>收费项目信息</summary>
public class ChargeItem
{
    /// <summary>项目编码</summary>
    public string Code { get; set; }

    /// <summary>项目名称</summary>
    public string Name { get; set; }

    /// <summary>规格</summary>
    public string Specs { get; set; }

    /// <summary>数量</summary>
    public int Quantity { get; set; }

    /// <summary>单位</summary>
    public string Unit { get; set; }

    /// <summary>金额</summary>
    public double Amount { get; set; }

    /// <summary>类别</summary>
    public string Category { get; set; }
}

/// <summary>置管信息</summary>
public class CannulateInfo
{
    /// <summary>置管护士</summary>
    public Guid? NurseId { get; set; }

    /// <summary>置管时间</summary>
    public DateTime? CannulateTime { get; set; }
}

/// <summary>影像检查信息</summary>
public class StudyArchive
{
    /// <summary>影像数量</summary>
    public int? ImageCount { get; set; }

    /// <summary>是否已打印</summary>
    public bool IsPrinted { get; set; }

    /// <summary>影像插入时间</summary>
    public DateTime? InsertionTime { get; set; }

    /// <summary>最后影像时间</summary>
    public DateTime? LastImageDateTime { get; set; }

    /// <summary>最后Build时间</summary>
    public DateTime? LastBuildDateTime { get; set; }

    /// <summary>是否是三维</summary>
    public int? Is3D { get; set; }

    /// <summary>检查描述</summary>
    public string StudyDescription { get; set; }

    /// <summary>机构编码</summary>
    public string SourceInstitutionCode { get; set; }

    /// <summary>影像拍片时间</summary>
    public DateTime? StudyDate { get; set; }

    /// <summary>影像号</summary>
    public string[] StudyInstanceUIDs { get; set; }

    /// <summary>影像存储节点</summary>
    public string StorageNode { get; set; }

    /// <summary>打印状态：未排版(0)，已排版(1)，已打印(2)</summary>
    public int? PrintStatus { get; set; }

    /// <summary>胶片数量</summary>
    public int? FilmCount { get; set; }
}

/// <summary>报告归档模型</summary>
public class ReportArchive
{
    /// <summary>报告提交时间</summary>
    public DateTime? SubmitTime { get; set; }

    /// <summary>报告提交医生编号</summary>
    public string SubmitDoctorCode { get; set; }

    /// <summary>报告提交医生姓名</summary>
    public string SubmitDoctorName { get; set; }

    /// <summary>报告审核时间</summary>
    public DateTime? ApproveTime { get; set; }

    /// <summary>报告审核医生编号</summary>
    public string ApproveDoctorCode { get; set; }

    /// <summary>报告审核医生姓名</summary>
    public string ApproveDoctorName { get; set; }

    /// <summary>实习医生编号</summary>
    public string InternDoctorCode { get; set; }

    /// <summary>实习医生姓名</summary>
    public string InternDoctorName { get; set; }

    /// <summary>分配的报告医生</summary>
    public string AssignedReportDoctor { get; set; }

    /// <summary>分配的审核医生</summary>
    public string AssignedApproveDoctor { get; set; }

    /// <summary>报告转发人</summary>
    public string ReportForwarder { get; set; }

    /// <summary>打印医生（或打印地点）</summary>
    public string PrintDoctor { get; set; }

    /// <summary>打印时间</summary>
    public DateTime? PrintTime { get; set; }

    /// <summary>检查所见</summary>
    public string Findings { get; set; }

    /// <summary>报告印象</summary>
    public string Impression { get; set; }

    /// <summary>技术参数</summary>
    public string TechParams { get; set; }

    /// <summary>阴阳性：-1:未知，0:阴性，1:阳性</summary>
    public int? PositiveStatus { get; set; }

    /// <summary>报告评级</summary>
    public string ReportRank { get; set; }

    /// <summary>临床诊断符合情况</summary>
    public string Consistency { get; set; }

    /// <summary>
    /// 报告pdf文件路径。支持存储服务链接、http链接、ftp链接。
    /// 特殊值：实时渲染（PrintHelper.AutoRendPdfURL中定义）
    /// </summary>
    public string PdfLink { get; set; }

    /// <summary>符合\不符合</summary>
    public string Accord { get; set; }

    /// <summary>危急值</summary>
    public bool? IsCritical { get; set; }

    /// <summary>危急类型</summary>
    public string CriticalType { get; set; }

    /// <summary>
    /// 病例讨论标记（PACS搜索使用）。
    /// 从三个维度标记：是否需要讨论，是否完成，是否符合
    /// </summary>
    public string[] CaseDiscussFlags { get; set; }

    /// <summary>是否是重新审核的（用于查询）</summary>
    public bool? IsReApproved { get; set; }

    /// <summary>读片人Id</summary>
    public Guid? ImageReaderId { get; set; }

    /// <summary>读片人</summary>
    public string ImageReaderName { get; set; }

    /// <summary>危急值通知信息</summary>
    public CriticalNotify CriticalNotify { get; set; } = new CriticalNotify();

    /// <summary>
    /// 报告标记：NeedCaseDiscussion / NeedFollowUp / ReportFollowUped / ReportQuality
    /// </summary>
    public string[] Flags { get; set; }

    /// <summary>报告状态</summary>
    public string Status { get; set; }

    /// <summary>RIS报告完整状态</summary>
    public string FullStatus { get; set; }

    /// <summary>ICD编码</summary>
    public string ICDCode { get; set; }

    /// <summary>ACR编码</summary>
    public string ACRCode { get; set; }

    /// <summary>报告元数据</summary>
    public string ReportMetaData { get; set; }
}

/// <summary>危急值通知信息</summary>
public class CriticalNotify
{
    /// <summary>发送人</summary>
    public string SendDoctor { get; set; }

    /// <summary>发送时间</summary>
    public DateTime? SendTime { get; set; }

    /// <summary>接收信息</summary>
    public CriticalRecvInfo RecvInfo { get; set; } = new CriticalRecvInfo();
}

/// <summary>危急值接收信息</summary>
public class CriticalRecvInfo
{
    /// <summary>接收人</summary>
    public string RecvDoctor { get; set; }

    /// <summary>接收科室</summary>
    public string RecvDepartment { get; set; }

    /// <summary>接收时间</summary>
    public DateTime? RecvTime { get; set; }

    /// <summary>处理意见</summary>
    public string HandleAction { get; set; }
}

/// <summary>预约信息</summary>
public class ScheduleArchive
{
    /// <summary>预约医生编码</summary>
    public string ScheduleDoctorCode { get; set; }

    /// <summary>预约医生名称</summary>
    public string ScheduleDoctorName { get; set; }

    /// <summary>预约时间</summary>
    public DateTime? ScheduleTime { get; set; }

    /// <summary>预约开始时间</summary>
    public DateTime? StartTime { get; set; }

    /// <summary>预约结束时间</summary>
    public DateTime? EndTime { get; set; }
}

/// <summary>协助信息</summary>
public class AssistInfo
{
    /// <summary>协助描述</summary>
    public string Description { get; set; }

    /// <summary>创建时间</summary>
    public DateTime CreateTime { get; set; }

    /// <summary>指定协助的医院（编码）</summary>
    public string[] TargetHospitals { get; set; }

    /// <summary>已经被锁定（不允许其它人下载）</summary>
    public bool IsLocked { get; set; }

    /// <summary>锁定人/医院</summary>
    public string Locker { get; set; }

    /// <summary>打印模板Id</summary>
    public Guid? PrintTemplateId { get; set; }

    /// <summary>进行协助的医院（统计用）</summary>
    public string AssistHospitalCode { get; set; }

    /// <summary>进行协助的医院（统计用）</summary>
    public string AssistHospitalName { get; set; }

    /// <summary>请求方的打印模板</summary>
    public ReqPrintTemplate[] PrintTemplates { get; set; }
}

public class ReqPrintTemplate
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}

/// <summary>
/// 扩展的档案数据。主要用于索引，第三方不感知。
/// 框架自动填充排序字段和全流程状态
/// </summary>
public class ExArchiveData
{
    /// <summary>检查项目排序字段（框架填充）</summary>
    public string CheckItemSort { get; set; }

    /// <summary>检查部位排序字段（框架填充）</summary>
    public string BodyPartSort { get; set; }

    /// <summary>检查所见排序字段（框架填充）</summary>
    public string FindingSort { get; set; }

    /// <summary>印象排序字段（框架填充）</summary>
    public string ImpressionSort { get; set; }

    /// <summary>全流程状态</summary>
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
