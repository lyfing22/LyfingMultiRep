using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HISInterfaceService.Core.HisRequestModel
{
    public class CaseUploadResult
    {
        public int ResultCode { get; set; }

        public string ResultDescription { get; set; }
    }
    public class CaseForThirdPartDTO
    {
        public string ServerNode { get; set; }

        public PatientForThirdPartDTO PatientInfo { get; set; }

        public OrderForThirdPartDTO OrderInfo { get; set; }

        public VisitForThirdPartDTO VisitInfo { get; set; }

        public ReportForThirdPartDTO ReportInfo { get; set; }

    }

    public class OrderForThirdPartDTO
    {
        /// <summary>
        /// 检查号
        /// </summary>
        public string AccessionNumber { get; set; }
        /// <summary>
        /// 登记时间
        /// </summary>
        public string RegisterTime { get; set; }
        /// <summary>
        /// 总费用
        /// </summary>
        public double TotalFee { get; set; }
        /// <summary>
        /// 到检时间
        /// </summary>
        public string ArriveTime { get; set; }
        /// <summary>
        /// 是否为HIS申请单
        /// </summary>
        public bool IsFromHIS { get; set; }
        /// <summary>
        /// HIS申请单编码
        /// </summary>
        public string HisOrderCode { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// 建议
        /// </summary>
        public string Suggestion { get; set; }
        /// <summary>
        /// 拍片日期
        /// </summary>
        public string StudyDate { get; set; }
        /// <summary>
        /// 影像号
        /// </summary>
        public string StudyInstanceUID { get; set; }
        /// <summary>
        /// 到检医生编码
        /// </summary>
        public string CheckInDoctorCode { get; set; }
        /// <summary>
        /// 到检医生姓名
        /// </summary>
        public string CheckInDoctorName { get; set; }
        /// <summary>
        /// 申请医生编码
        /// </summary>
        public string ApplyDoctorCode { get; set; }
        /// <summary>
        /// 申请医生姓名
        /// </summary>
        public string ApplyDoctorName { get; set; }
        /// <summary>
        /// 检查医生编码
        /// </summary>
        public string ExecDoctorCode { get; set; }
        /// <summary>
        /// 检查医生姓名
        /// </summary>
        public string ExecDoctorName { get; set; }
        /// <summary>
        /// 报告医生编码
        /// </summary>
        public string ReportDoctorCode { get; set; }
        /// <summary>
        /// 报告医生姓名
        /// </summary>
        public string ReportDoctorName { get; set; }
        /// <summary>
        /// 申请科室编码
        /// </summary>
        public string ApplyDepartmentCode { get; set; }
        /// <summary>
        /// 申请医生姓名
        /// </summary>
        public string ApplyDepartmentName { get; set; }
        /// <summary>
        /// 检查科室编码
        /// </summary>
        public string ExecDepartmentCode { get; set; }
        /// <summary>
        /// 检查科室姓名
        /// </summary>
        public string ExecDepartmentName { get; set; }
        /// <summary>
        /// 检查设备编码
        /// </summary>
        public string DeviceCode { get; set; }
        /// <summary>
        /// 检查设备名称
        /// </summary>
        public string DeviceName { get; set; }
        /// <summary>
        /// 检查室名称
        /// </summary>
        public string ExamRoom { get; set; }
        /// <summary>
        /// 检查设备AETitle
        /// </summary>
        public string DeviceAETitle { get; set; }
        /// <summary>
        /// 设备类型编码
        /// </summary>
        public string ModalityCode { get; set; }
        /// <summary>
        /// 设备类型名称
        /// </summary>
        public string ModalityName { get; set; }
        /// <summary>
        /// 检查项目，单个格式为 检查部位编码,检查部位名称，检查项目编码，检查项目名称，价格 ，多个项目则以|分隔
        /// </summary>
        public string CheckItems { get; set; }
        /// <summary>
        /// 年龄单位，如岁为year,月为month,周为week,天为day
        /// </summary>
        public string AgeUnitCode { get; set; }
        /// <summary>
        /// 年龄单位，岁、月、周、天、小时
        /// </summary>
        public string AgeUnit { get; set; }
        /// <summary>
        /// 年龄值
        /// </summary>
        public int Age { get; set; }
        public string ScheduledStartTime { get; set; }
        public string ScheduledEndTime { get; set; }
        /// <summary>
        /// HIS 申请单申请日期
        /// </summary>
        public string HisOrderRequestDate { get; set; }

        /// <summary>
        /// 集成数据源Code
        /// </summary>
        public string IntegrationSourceCode { get; set; }
    }

    public class PatientForThirdPartDTO
    {
        /// <summary>
        /// 患者编号
        /// </summary>
        public string GlobalPatientId { get; set; }
        /// <summary>
        /// 出生日期,格式如1990-04-12
        /// </summary>
        public string DateOfBirth { get; set; }
        /// <summary>
        /// 电子邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 性别，男-M,女-F
        /// </summary>
        public string Gender { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// 拼音名（可不填）
        /// </summary>
        public string SpellName { get; set; }
        /// <summary>
        /// 医保号
        /// </summary>
        public string SocietyNumber { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Telephone { get; set; }
        /// <summary>
        /// 身份证号
        /// </summary>
        public string IDCard { get; set; }
        /// <summary>
        /// 住址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 姓氏
        /// </summary>
        public string FamilyName { get; set; }
        /// <summary>
        /// 名字
        /// </summary>
        public string GivenName { get; set; }
        /// <summary>
        /// 中名
        /// </summary>
        public string MiddleName { get; set; }
    }

    public class ReportForThirdPartDTO
    {

        /// <summary>
        /// 阳性率，必须字段 {true： 阳性}
        /// </summary>
        public bool? IsPositive { get; set; }

        /// <summary>
        /// 关键字
        /// </summary>
        public string Keywords { get; set; }

        /// <summary>
        /// 报告url
        /// </summary>
        public string ReportUrl { get; set; }

        /// <summary>
        /// 审核医生姓名
        /// </summary>
        public string ApproveDoctorName { get; set; }

        /// <summary>
        /// 审核医生Code
        /// </summary>
        public string ApproveDoctorCode { get; set; }

        /// <summary>
        /// 审核时间
        /// </summary>
        public string ApproveDateTime { get; set; }

        /// <summary>
        /// 提交医生
        /// </summary>
        public string SubmitDoctorName { get; set; }

        /// <summary>
        /// 提交报告医生id
        /// </summary>
        public string SubmitDoctorCode { get; set; }

        /// <summary>
        /// 提交时间
        /// </summary>
        public string SubmitDateTime { get; set; }

        /// <summary>
        /// 检查所见
        /// </summary>
        public string Findings { get; set; }

        /// <summary>
        /// 检查印象
        /// </summary>
        public string Impression { get; set; }

        /// <summary>
        /// 符合/不符合
        /// </summary>
        public string Accord { get; set; }

        /// <summary>
        /// 摄片评级
        /// </summary>
        public string FilmingRank { get; set; }

        /// <summary>
        /// 讨论区
        /// </summary>
        public string Forum { get; set; }

        /// <summary>
        /// 是否打印胶片  
        /// </summary>
        public bool? IsPrintFilm { get; set; }

        /// <summary>
        /// 结论
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// ICD Code
        /// </summary>
        public string ICDCode { get; set; }

        /// <summary>
        /// ACR Code
        /// </summary>
        public string ACRCode { get; set; }
    }

    public class VisitForThirdPartDTO
    {
        /// <summary>
        /// 患者来源，
        /// </summary>
        public string PatientType { get; set; }
        /// <summary>
        /// HIS 患者编号，（可能不唯一）
        /// </summary>
        public string HisPatientId { get; set; }
        /// <summary>
        /// 门诊号
        /// </summary>
        public string ClinicalNumber { get; set; }
        /// <summary>
        /// 病房号
        /// </summary>
        public string RoomNumber { get; set; }
        /// <summary>
        /// 病床号
        /// </summary>
        public string BedNumber { get; set; }
        /// <summary>
        /// 住院号
        /// </summary>
        public string InpatientNumber { get; set; }
        /// <summary>
        /// 临床诊断
        /// </summary>
        public string ClinicalDiagnosis { get; set; }
        /// <summary>
        /// 过敏史
        /// </summary>
        public string AllergyHistory { get; set; }
        /// <summary>
        /// 病史
        /// </summary>
        public string DiseaseHistory { get; set; }

        /// <summary>
        /// 紧急程度
        /// </summary>
        public int? EmergencyDegree { get; set; }

        /// <summary>
        /// 危急值
        /// </summary>
        public string CriticalCode { get; set; }
        /// <summary>
        /// 症状
        /// </summary>
        public string Symptom { get; set; }

        /// <summary>
        /// 体征
        /// </summary>
        public string Sign { get; set; }


        /// <summary>
        /// 就诊号
        /// </summary>
        public string VisitNumber { get; set; }
    }

    public class ProcedureForThirdPartDTO
    {
        public Guid? CheckItemId { get; set; }

        public string CheckItemName { get; set; }

        public Guid? BodyPartId { get; set; }

        public string BodyPartName { get; set; }

        public double Fee { get; set; }
    }

    public class CaseAdaptiveDTO
    {
        public CaseForThirdPartDTO CaseForThirdPart { get; set; }

        #region Complemental Members
        /// <summary>
        /// 登记医生ID
        /// </summary>
        public Guid? RegisterDoctorId { get; set; }
        /// <summary>
        /// 申请医生Id
        /// </summary>
        public Guid? ApplyDoctorId { get; set; }
        /// <summary>
        /// 检查医生Id
        /// </summary>
        public Guid? ExecDoctorId { get; set; }
        /// <summary>
        /// 报告医生Id
        /// </summary>
        public Guid? ReportDoctorId { get; set; }
        /// <summary>
        /// 申请科室Id
        /// </summary>
        public Guid? ApplyDepartmentId { get; set; }
        /// <summary>
        /// 申请科室Id
        /// </summary>
        public Guid? ExecDepartmentId { get; set; }
        /// <summary>
        /// 检查设备Id
        /// </summary>
        public Guid? DeviceId { get; set; }
        /// <summary>
        /// 检查设备
        /// </summary>
        //public Device Device { get; set; }
        /// <summary>
        /// 设备类型Id
        /// </summary>
        public Guid? ModalityId { get; set; }
        /// <summary>
        /// 设备类型
        /// </summary>
        //public Modality Modality { get; set; }
        /// <summary>
        /// 检查项目列表
        /// </summary>
        public IList<ProcedureForThirdPartDTO> Procedures { get; set; }
        /// <summary>
        /// 报告提交医生Id
        /// </summary>
        public Guid? SubmitDoctorId { get; set; }
        /// <summary>
        /// 报告提交医生
        /// </summary>
       // public Staff SubmitDoctor { get; set; }
        /// <summary>
        /// 报告审核医生Id
        /// </summary>
        public Guid? ApproveDoctorId { get; set; }
        /// <summary>
        /// 报告审核医生
        /// </summary>
        //public Staff ApproveDoctor { get; set; }
        /// <summary>
        ///报告内容
        /// </summary>
        public string ReportContent { get; set; }
        /// <summary>
        /// 患者来源描述
        /// </summary>
        public string PatientTypeDescription { get; set; }
        /// <summary>
        /// 性别描述
        /// </summary>
        public string GenderDescription { get; set; }
        /// <summary>
        /// 检查部位，以|分隔
        /// </summary>
        public string BodyPartNames { get; set; }
        /// <summary>
        /// 检查项目
        /// </summary>
        public string CheckItemNames { get; set; }

        #endregion

    }
}
