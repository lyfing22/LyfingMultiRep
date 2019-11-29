using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace HISInterfaceService.Core.HisRequestModel
{
    [XmlRoot("Order")]
    public class OrderItem
    {
        /// <summary>
        /// 
        /// </summary>
        [XmlElement("OEORIOrderItemID")]
        public string OEORIOrderItemID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [XmlElement("RISRPositionCode")]
        public string RISRPositionCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [XmlElement("RISRPostureCode")]
        public string RISRPostureCode { get; set; }
        /// <summary>
        /// 心悸
        /// </summary>
        [XmlElement("RISRCode")]
        public string RISRCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [XmlElement("RISRDesc")]
        public string RISRDesc { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [XmlElement("RISRPrice")]
        public string RISRPrice { get; set; }
    }

    [XmlRoot("OrderList")]
    public class OrderList
    {
        /// <summary>
        /// 
        /// </summary>
        [XmlElement("Order")]
        public OrderItem[] Order { get; set; }
    }

    public class AddRisAppBillRtItem
    {

        public string DocumentID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BusinessFieldCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string HospitalCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string RISRAppNum { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string RISRExamID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PATPatientID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PAADMVisitNumber { get; set; }
        /// <summary>
        /// 就诊类型代码 用已区分O:门诊、H:体检、E:急诊病人就诊信息
        /// </summary>
        public string PAADMEncounterTypeCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PAADMAdmWardCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PAADMAdmWardDesc { get; set; }
        /// <summary>
        /// 002床
        /// </summary>
        public string PAADMCurBedNo { get; set; }
        /// <summary>
        /// 检查目的
        /// </summary>
        public string RISRMattersAttention { get; set; }
        /// <summary>
        /// 现病史
        /// </summary>
        public string RISRSpecalMedicalRecord { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string RISRSubmitDocCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string RISRSubmitDocDesc { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string RISRSubmitTime { get; set; }
        /// <summary>
        /// 医学影像科
        /// </summary>
        public string RISRAcceptDeptCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string RISRDeptLocation { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string RISRISEmergency { get; set; }
        /// <summary>
        /// 体征
        /// </summary>
        public string RISRClinicalSymptoms { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [XmlElement("OrderList")]
        public OrderList OrderList { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string UpdateUserCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string UpdateDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string UpdateTime { get; set; }
        /// <summary>
        /// PWBEK-脾胃病二科
        /// </summary>
        public string AppDeptDesc { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string OEORIOrderItemID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string RISRPositionCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string RISRPostureCode { get; set; }
        /// <summary>
        /// 心悸
        /// </summary>
        public string RISRCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string RISRDesc { get; set; }

        public string OEORIAppDeptCode { get; set; }

        public string SCHEDULED_DATE_TIME { get; set; }
        public string clinic { get; set; }
    }
}
