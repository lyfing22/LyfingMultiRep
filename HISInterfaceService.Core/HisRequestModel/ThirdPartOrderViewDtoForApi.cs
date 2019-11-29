using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HISInterfaceService.Core.HisRequestModel
{
    public class OrderViewDTOForApi 
    {

        /// <summary>
        /// 患者信息
        /// </summary>
        public PatientForUploadDTOForApi PatientInfo { get; set; }
        /// <summary>
        /// 就诊信息
        /// </summary>
        public VisitForUploadDTOForApi VisitInfo { get; set; }
        /// <summary>
        /// 申请单信息
        /// </summary>
        public OrderForUploadDTOForApi OrderInfo { get; set; }
        /// <summary>
        /// 报告信息
        /// </summary>
        public ReportForUploadDTOForApi ReportInfo { get; set; }
        /// <summary>
        /// 检查信息
        /// </summary>
        public StudyInfoDTOForApi StudyInfo { get; set; }
        /// <summary>
        /// 是在区域中代表一个节点的标识
        /// </summary>
        public string ServerNode { get; set; }
        /// <summary>
        /// 是在区域中代表一个医疗机构的标识
        /// </summary>
        public string HospitalCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string RuningFeature { get; set; }
    }

    public class PatientForUploadDTOForApi
    {
        /// <summary>
        /// 患者姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 患者唯一编号
        /// </summary>
        public string GlobalPatientId { get; set; }
        /// <summary>
        /// GlobalPatientId类型
        /// 1:his,2:其他系统 0:默认
        /// </summary>
        public int PidType { get; set; }//

        /// <summary>
        /// His患者编号
        /// </summary>
        public string HISPatientId { get; set; }

        /// <summary>
        /// His患者编号来源
        /// </summary>
        public string PidSourceCode { get; set; }

        /// <summary>
        /// 患者信息版本
        /// </summary>
        public long PatientUpdVersion { get; set; }

        /// <summary>
        /// 患者出生日期，格式为yyyy-MM-dd，如1990-03-12
        /// </summary>
        public string DateOfBirth { get; set; }
        /// <summary>
        /// 患者性别，男为M，女为F
        /// </summary>
        public string Gender { get; set; }
        /// <summary>
        /// 身份证号
        /// </summary>
        public string IDCard { get; set; }
        /// <summary>
        /// 医保号
        /// </summary>
        public string SocietyNumber { get; set; }
        /// <summary>
        /// 拼音名
        /// </summary>
        public string SpellName { get; set; }
        /// <summary>
        /// 姓氏
        /// </summary>
        public string FamilyName { get; set; }
        /// <summary>
        /// 名字
        /// </summary>
        public string GivenName { get; set; }
        /// <summary>
        /// 中间名
        /// </summary>
        public string MiddleName { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// 电子邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Telephone { get; set; }
    }

    public class VisitForUploadDTOForApi
    {
        /// <summary>
        /// 门诊号
        /// </summary>
        public string ClinicalNumber { get; set; }
        /// <summary>
        /// 住院号
        /// </summary>
        public string InpatientNumber { get; set; }

        /// <summary>
        /// 病案号
        /// </summary>
        public string MedicalRecordNumber { get; set; }

        /// <summary>
        /// 就诊流水号
        /// </summary>
        public string VisitSerialNumber { get; set; }
        /// <summary>
        /// 患者来源，住院为IH，门诊为OP，急诊为EM
        /// </summary>
        public string PatientType { get; set; }
        /// <summary>
        /// 病床号
        /// </summary>
        public string BedNumber { get; set; }
        /// <summary>
        /// 病房号
        /// </summary>
        public string RoomNumber { get; set; }
        /// <summary>
        /// 临床诊断
        /// </summary>
        public string ClinicalDiagnosis { get; set; }
        /// <summary>
        /// 过敏史
        /// </summary>
        public string AllergyHistory { get; set; }
    }

    public class OrderForUploadDTOForApi
    {
        /// <summary>
        /// 检查号
        /// </summary>
        public string AccessionNumber { get; set; }
        /// <summary>
        /// 请求影像平台生成的默认给:1,其它给2,RIS:0
        /// </summary>
        public int AccNumType { get; set; }
        /// <summary>
        /// 申请科室编号
        /// </summary>
        public string ApplyDepartmentCode { get; set; }
        /// <summary>
        /// 申请科室名称
        /// </summary>
        public string ApplyDepartmentName { get; set; }
        /// <summary>
        /// 申请医生编号
        /// </summary>
        public string ApplyDoctorCode { get; set; }
        /// <summary>
        /// 申请医生姓名
        /// </summary>
        public string ApplyDoctorName { get; set; }
        /// <summary>
        /// 到检时间，格式如1992-06-16 10:10:12 
        /// </summary>
        public string ArriveTime { get; set; }
        /// <summary>
        /// 到检操作医生编号
        /// </summary>
        public string CheckInDoctorCode { get; set; }
        /// <summary>
        /// 到检医生姓名
        /// </summary>
        public string CheckInDoctorName { get; set; }
        /// <summary>
        /// 设备的AETitle
        /// </summary>
        public string DeviceAETitle { get; set; }
        /// <summary>
        /// 设备编号
        /// </summary>
        public string DeviceCode { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; }
        /// <summary>
        /// 检查室
        /// </summary>
        public string ExamRoom { get; set; }
        /// <summary>
        /// 执行科室编号
        /// </summary>
        public string ExecDepartmentCode { get; set; }
        /// <summary>
        /// 执行科室名称
        /// </summary>
        public string ExecDepartmentName { get; set; }
        /// <summary>
        /// 检查医生编码
        /// </summary>
        public string ExecDoctorCode { get; set; }
        /// <summary>
        /// 检查医生姓名
        /// </summary>
        public string ExecDoctorName { get; set; }
        /// <summary>
        /// 是否来源于His
        /// </summary>
        public bool? IsFromHIS { get; set; }
        /// <summary>
        /// His申请单号
        /// </summary>
        public string HisOrderCode { get; set; }
        /// <summary>
        /// 设备类型编号
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
        /// 申请单状态：已检查（拍完片）是Studyed,报告中（即报告提交了但未经过审核）是Reporting，已报告（报告完成并通过了审核）是Reported
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// 登记时间，格式如1992-06-16 10:10:12
        /// </summary>
        public string RegisterTime { get; set; }
        /// <summary>
        /// 报告医师编码
        /// </summary>
        public string ReportDoctorCode { get; set; }
        /// <summary>
        /// 报告医师姓名
        /// </summary>
        public string ReportDoctorName { get; set; }
        /// <summary>
        /// 影像拍片时间，格式如1992-06-16 10:10:12
        /// </summary>
        public string StudyDate { get; set; }
        /// <summary>
        /// 影像号
        /// </summary>
        public string StudyId { get; set; }
        /// <summary>
        /// 建议
        /// </summary>
        public string Suggestion { get; set; }
        /// <summary>
        /// 患者年龄（注：Age如果为空，请传0或不传（即不带此参数））
        /// </summary>
        public int? Age { get; set; }
        /// <summary>
        /// 年龄单位，如岁为year,月为month,周为week,天为day
        /// </summary>
        public string AgeUnit { get; set; }
        /// <summary>
        /// 总费用（如为空，请传0）
        /// </summary>
        public double TotalFee { get; set; }
        /// <summary>
        /// 摄片等级
        /// </summary>
        public string FilmingRank { get; set; }
        /// <summary>
        /// 是否请求区域平台协助写报告
        /// </summary>
        public bool? RegionAssist { get; set; }
        /// <summary>
        /// 请求写报告类别，0：自己写报告，1：请求写报告，2：请求审核报告，3：请求再审核报告
        /// </summary>
        public int? RegionAssistType { get; set; }
        /// <summary>
        /// 请求写报告的描述
        /// </summary>
        public string RegionAssistComments { get; set; }
    }

    public class ReportForUploadDTOForApi
    {
        /// <summary>
        /// 报告提交时间，格式如1992-06-16 10:10:12
        /// </summary>
        public string SubmitDateTime { get; set; }
        /// <summary>
        /// 报告提交医生编号
        /// </summary>
        public string SubmitDoctorCode { get; set; }
        /// <summary>
        /// 报告提交医生姓名
        /// </summary>
        public string SubmitDoctorName { get; set; }
        /// <summary>
        /// 报告审核时间，格式如1992-06-16 10:10:12
        /// </summary>
        public string ApproveDatetime { get; set; }
        /// <summary>
        /// 报告审核医生编号
        /// </summary>
        public string ApproveDoctorCode { get; set; }
        /// <summary>
        /// 报告审核医生姓名
        /// </summary>
        public string ApproveDoctorName { get; set; }
        /// <summary>
        /// 检查缩减
        /// </summary>
        public string Findings { get; set; }
        /// <summary>
        /// 报告印象
        /// </summary>
        public string Impression { get; set; }
        /// <summary>
        /// 是否为阳性
        /// </summary>
        public bool? IsPositive { get; set; }
        //IsPrintFilm
        /// <summary>
        /// 关键词
        /// </summary>
        public string KeyWords { get; set; }
        /// <summary>
        /// 报告pdf文件路径
        /// </summary>
        public string SnapshotFilePath { get; set; }
        //ReportUrl
        /// <summary>
        /// 讨论区
        /// </summary>
        public string Forum { get; set; }
        /// <summary>
        /// 符合\不符合
        /// </summary>
        public string Accord { get; set; }
        /// <summary>
        /// 危急值
        /// </summary>
        public bool? IsCritical { get; set; }
    }

    public class StudyInfoDTOForApi
    {
        /// <summary>
        /// 影像数量
        /// </summary>
        public int? ImageCount { get; set; }
        /// <summary>
        /// 影像插入时间
        /// </summary>
        public string InsertionTime { get; set; }
        /// <summary>
        /// 最后Build时间
        /// </summary>
        public string LastBuildDateTime { get; set; }
        /// <summary>
        /// 是否是三维
        /// </summary>
        public int? Is3D { get; set; }
        /// <summary>
        /// 检查描述
        /// </summary>
        public string StudyDescription { get; set; }
        /// <summary>
        /// 机构编码
        /// </summary>
        public string SourceInstitutionCode { get; set; }

        /// <summary>
        /// 合并Study信息
        /// </summary>
        public List<MergeStudyItem> MergeStudyInstanceIds { get; set; }
    }

    /// <summary>
    /// Merge Study项
    /// </summary>
    public class MergeStudyItem
    {
        public string MergeStudyInstanceId { get; set; }
    }

}
