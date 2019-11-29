using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HISInterfaceService.Core.HisRequestModel.EXAMMASTER
{

    /// <summary>
    ///检查报告主表
    /// </summary>
    public class EXAMMASTER
    {
        /// <summary>
        /// 医疗机构代码
        /// </summary>
        public string ORG_CODE { get; set; }
        /// <summary>
        /// 检查报告单号
        /// </summary>
        public string REPORT_FORM_NO { get; set; }
        /// <summary>
        /// 病人ID
        /// </summary>
        public string PATIENT_ID { get; set; }
        /// <summary>
        /// 医疗机构代码
        /// </summary>
        public string EVENT_TYPE { get; set; }
        /// <summary>
        /// 本地事件号（门诊号或者住院号）
        /// </summary>
        public string EVENT_NO { get; set; }
        /// <summary>
        /// 归档检索日期
        /// </summary>
        public DateTime? RETRIEVE_DATE { get; set; }
        /// <summary>
        /// 检查分类的标准编码
        /// </summary>
        public string CLASS_TYPE_CODE { get; set; }
        /// <summary>
        /// 检查分类的标准名称
        /// </summary>
        public string CLASS_TYPE_NAME { get; set; }
        /// <summary>
        /// 检查部位的标准编码
        /// </summary>
        public string EXAM_SITE_CODE { get; set; }
        /// <summary>
        /// 检查项目的标准编码
        /// </summary>
        public string EXAM_ITEM_CODE { get; set; }
        /// <summary>
        /// 检查项目名称
        /// </summary>
        public string EXAM_ITEM_NAME { get; set; }
        /// <summary>
        /// 报告的标题
        /// </summary>
        public string REPORT_TITLE { get; set; }
        /// <summary>
        /// 文档创建时间
        /// </summary>
        public DateTime EFFECTIVE_DTIME { get; set; }
        /// <summary>
        /// 病人的姓名
        /// </summary>
        public string NAME { get; set; }
        /// <summary>
        /// 病人性别代码
        /// </summary>
        public string SEX_CODE { get; set; }
        /// <summary>
        /// 病人出生日期
        /// </summary>
        public string BIRTH_DATE { get; set; }
        /// <summary>
        /// 填报者：编号
        /// </summary>
        public string AUTHOR_ID { get; set; }
        /// <summary>
        /// 报告生成时间
        /// </summary>
        public string REPORT_CREATE_DTIME { get; set; }
        /// <summary>
        /// 填报者：姓名
        /// </summary>
        public string AUTHOR_NAME { get; set; }
        /// <summary>
        /// 审核者：编号
        /// </summary>
        public string AUTHENTICATOR_ID { get; set; }
        /// <summary>
        /// 审核者：审核时间
        /// </summary>
        public string AUTHENTICATOR_DTIME { get; set; }
        /// <summary>
        /// 审核者：姓名
        /// </summary>
        public string AUTHENTICATOR_NAME { get; set; }
        /// <summary>
        /// 申请科室
        /// </summary>
        public string PARTICIPANT_DEPT { get; set; }
        /// <summary>
        /// 申请医生：编号
        /// </summary>
        public string PARTICIPANT_ID { get; set; }
        /// <summary>
        /// 申请医生：申请时间
        /// </summary>
        public string PARTICIPANT_DTIME { get; set; }
        /// <summary>
        /// 申请医生：姓名
        /// </summary>
        public string PARTICIPANT_NAME { get; set; }
        /// <summary>
        /// 对应的医嘱：医嘱号
        /// </summary>
        public string ORDER_ID { get; set; }
        /// <summary>
        /// 对应的医嘱：优先级  对应医嘱类型的描述
        /// </summary>
        public string ORDER_PRIORITY { get; set; }


        /// <summary>
        /// 检查的执行科室名称
        /// </summary>
        public string PERFORMER_DEPT_NAME { get; set; }
        /// <summary>
        /// 检查的执行医生
        /// </summary>
        public string PERFORMER_DOCTOR { get; set; }

        /// <summary>
        /// 检查的执行时间
        /// </summary>
        public string EXAM_PERFORMER_DTIME { get; set; }
        /// <summary>
        /// 临床诊断
        /// </summary>
        public string OUTPAT_DIAG_NAME { get; set; }
        /// <summary>
        ///病情描述
        /// </summary>
        public string PATIENT_CONDITION_DESCR { get; set; }
        /// <summary>
        /// 检查目的
        /// </summary>
        public string EXAM_PURPOSE { get; set; }
        /// <summary>
        /// 影像所见
        /// </summary>
        public string IMAGE_DESCR { get; set; }
        /// <summary>
        /// 是否异常检验/检查结果是否异常的标志
        /// </summary>
        public string IS_ABNORMAL { get; set; }
        /// <summary>
        ///影像结论 对影像结果的结论描述
        /// </summary>
        public string CONCLUSION { get; set; }
        /// <summary>
        ///检查结果代码
        /// </summary>
        public string Exam_Result_Code { get; set; }


        /// <summary>
        ///检查定量结果
        /// </summary>
        public string Exam_Quantitive_Result { get; set; }
        /// <summary>
        ///检查定量结果计量单位
        /// </summary>
        public string Quantitive_Unit { get; set; }
        /// <summary>
        ///特殊检查标志
        /// </summary>
        public string Special_Flag { get; set; }
        /// <summary>
        ///介入物名称
        /// </summary>
        public string Intervention { get; set; }
        /// <summary>
        ///操作方法描述
        /// </summary>
        public string Operation_Way_Descr { get; set; }
        /// <summary>
        ///操作次数
        /// </summary>
        public string Operation_Count { get; set; }
        /// <summary>
        ///麻醉方法代码
        /// </summary>
        public string Anesthesia_Code { get; set; }
        /// <summary>
        ///麻醉观察结果
        /// </summary>
        public string Anesthetic_Observation { get; set; }
        /// <summary>
        ///麻醉医师签名
        /// </summary>
        public string Anesthetic_Doctor { get; set; }

        /// <summary>
        ///标本类别描述
        /// </summary>
        public string Specimen_Class { get; set; }
        /// <summary>
        ///检查标本号
        /// </summary>
        public string Specimen_No { get; set; }
        /// <summary>
        ///标本状态
        /// </summary>
        public string Specimen_Status { get; set; }
        /// <summary>
        ///标本固定液名称
        /// </summary>
        public string Specimen_Fixative { get; set; }
        /// <summary>
        ///标本采样日期时间
        /// </summary>
        public string Specimen_Sample_Time { get; set; }
        /// <summary>
        ///接收标本日期时间
        /// </summary>
        public string pecimen_Receive_Time { get; set; }
        /// <summary>
        ///检查技师签名
        /// </summary>
        public string Exam_Technician { get; set; }
        /// <summary>
        ///诊断日期
        /// </summary>
        public string Diag_Date { get; set; }
        /// <summary>
        ///主诉
        /// </summary>
        public string Chief_Complaints { get; set; }
        /// <summary>
        ///症状开始日期时间
        /// </summary>
        public DateTime Symptom_Start_Time { get; set; }
        /// <summary>
        ///症状停止日期时间
        /// </summary>
        public DateTime Symptom_End_Time { get; set; }
        /// <summary>
        ///登记时间
        /// </summary>
        public DateTime REGISTER_TIME { get; set; }

        /// <summary>
        /// 登记人
        /// </summary>
        public string REGISTER_OPERATOR { get; set; }

        /// <summary>
        /// 诊断代码
        /// </summary>
        public string DIAGNOSE_CODE { get; set; }
        /// <summary>
        /// 打印时间
        /// </summary>
        public DateTime PRINT_TIME { get; set; }
        /// <summary>
        /// 删除标识
        /// </summary>
        public string DELETE_MARK { get; set; }

    }
}
