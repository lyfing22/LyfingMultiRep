using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HISInterfaceService.Core.HisRequestModel
{
    public class RegisterDocumentRt
    {
        /// <summary>
        /// 医疗机构编码
        /// </summary>
        public string OrganizationCode { get; set; }
        /// <summary>
        /// 患者主索引
        /// </summary>
        public string PATPatientID { get; set; }
        /// <summary>
        /// 就诊号码
        /// </summary>
        public string PAADMVisitNumber { get; set; }
        /// <summary>
        /// 检查号
        /// </summary>
        public string RISRExamID { get; set; }
        /// <summary>
        /// 样本号（条码号）
        /// </summary>
        public string SpecimenID { get; set; }
        /// <summary>
        /// 医嘱号
        /// </summary>
        public string OEORIOrderItemID { get; set; }
        /// <summary>
        /// 文档类别
        /// </summary>
        public string DocumentType { get; set; }
        /// <summary>
        /// 文档ID
        /// </summary>
        public string DocumentID { get; set; }
        /// <summary>
        /// 文档内容
        /// </summary>
        public string DocumentContent { get; set; }
        /// <summary>
        /// PDF文档路径
        /// </summary>
        public string DocumentPath { get; set; }
        /// <summary>
        /// 最后更新人编码
        /// </summary>
        public string UpdateUserCode { get; set; }
        /// <summary>
        /// 最后更新日期
        /// </summary>
        public string UpdateDate { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public string UpdateTime { get; set; }
    }
}
