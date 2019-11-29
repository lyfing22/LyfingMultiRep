using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using HISInterfaceService.Core.EntityModel;

namespace HISInterfaceService.Core.HisRequestModel
{
    [XmlRoot(ElementName= "clinicalDocument")]
    public   class ClinicalDocument
    {
        public DocumentHeader documentHeader { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public StructuredBody structuredBody { get; set; }
    }
    public class Certificate
    {
        /// <summary>
        /// 证件名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 证件号码
        /// </summary>
        public string value { get; set; }
    }

    public class Addr
    {
        /// <summary>
        /// 
        /// </summary>
        public string text { get; set; }
        /// <summary>
        /// xx号xx小区xx栋xx单元
        /// </summary>
        public string houseNumber { get; set; }
        /// <summary>
        /// xx大道
        /// </summary>
        public string streetName { get; set; }
        /// <summary>
        /// xx乡镇
        /// </summary>
        public string township { get; set; }
        /// <summary>
        /// xx区
        /// </summary>
        public string county { get; set; }
        /// <summary>
        /// xx市
        /// </summary>
        public string city { get; set; }
        /// <summary>
        /// xx省
        /// </summary>
        public string state { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string postalCode { get; set; }
        /// <summary>
        /// 现住址
        /// </summary>
        [XmlAttribute("desc")]
        public string @desc { get; set; }
    }

    public class Age
    {
        /// <summary>
        /// 岁
        /// </summary>
        [XmlAttribute("unit")]
        public string @unit { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute("value")]
        public string @value { get; set; }
    }

    public class Patient
    {
        /// <summary>
        /// 
        /// </summary>
        public string medicareNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string admvisitNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string medRecordNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string healthCardNo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Certificate certificate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Addr addr { get; set; }
        /// <summary>
        /// 胡传发
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string telecom { get; set; }
        /// <summary>
        /// 男性
        /// </summary>
        public string administrativeGender { get; set; }
        /// <summary>
        /// 已婚
        /// </summary>
        public string maritalStatus { get; set; }
        /// <summary>
        /// 汉族
        /// </summary>
        public string ethnicGroup { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Age age { get; set; }
    }

    public class Participant
    {
        /// <summary>
        /// 家庭关系描述
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Addr addr { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string telecom { get; set; }
        /// <summary>
        /// 联系人姓名
        /// </summary>
        public string name { get; set; }
    }

    public class DocumentHeader
    {
        /// <summary>
        /// 
        /// </summary>
        public string realmCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string typeId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string template { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 检查报告
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string effectiveTime { get; set; }
        /// <summary>
        /// 级别名称
        /// </summary>
        public string confidentiality { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string versionNumber { get; set; }
        /// <summary>
        /// 医生姓名
        /// </summary>
        public Author author { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string custodian { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Patient patient { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Participant participant { get; set; }
    }

    public class Author
    {
        [XmlAttribute("id")]
        public string AuthorCode{ get;set;}
        [XmlText]
        public string author { get; set; }
    }

    public class SectionItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string text { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string E07 { get; set; }
        /// <summary>
        /// 诊断名称
        /// </summary>
        public string E01 { get; set; }
        /// <summary>
        /// 诊断代码
        /// </summary>
        public string E02 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute("code")]
        public string @code { get; set; }
        /// <summary>
        /// 诊断
        /// </summary>
        [XmlAttribute("desc")]
        public string @desc { get; set; }

        /// <sustringry>
        ///    string
        /// </sstringary>
        public string E03 { get; set; }
        /// <sustringry>
        ///    string
        /// </sstringary>
        public string E04 { get; set; }
        /// <sustringry>
        ///    string
        /// </sstringary>
        public string E05 { get; set; }
        /// <sustringry>
        ///    string
        /// </sstringary>
        public string E06 { get; set; }
        /// <sustringry>
        ///    string
        /// </sstringary>
        /// <sustringry>
        ///    string
        /// </sstringary>
        public string E08 { get; set; }
        /// <sustringry>
        ///    string
        /// </sstringary>
        public string E09 { get; set; }
        /// <sustringry>
        ///    string
        /// </sstringary>
        public string E10 { get; set; }
        /// <sustringry>
        ///    string
        /// </sstringary>
        public string E11 { get; set; }
        /// <sustringry>
        ///    string
        /// </sstringary>
        public string E12 { get; set; }
        /// <sustringry>
        ///    string
        /// </sstringary>
        public string E13 { get; set; }
        /// <sustringry>
        ///    string
        /// </sstringary>
        public string E14 { get; set; }
        /// <sustringry>
        ///    string
        /// </sstringary>
        public string E15 { get; set; }
        /// <sustringry>
        ///    string
        /// </sstringary>
        public string E16 { get; set; }
        /// <sustringry>
        ///    string
        /// </sstringary>
        public string E17 { get; set; }
        /// <sustringry>
        ///    string
        /// </sstringary>
        public string E18 { get; set; }
        /// <sustringry>
        ///    string
        /// </sstringary>
        public string E19 { get; set; }
        /// <sustringry>
        ///    string
        /// </sstringary>
        public string E20 { get; set; }
        /// <sustringry>
        ///    string
        /// </sstringary>
        public string E21 { get; set; }
        /// <sustringry>
        ///    string
        /// </sstringary>
        public string E22 { get; set; }
        /// <sustringry>
        ///    string
        /// </sstringary>
        public string E23 { get; set; }
        /// <sustringry>
        ///    string
        /// </sstringary>
        public string E24 { get; set; }
        /// <sustringry>
        ///    string
        /// </sstringary>
        public string E25 { get; set; }
    }

    public class StructuredBody
    {
        /// <summary>
        /// 
        /// </summary>
        public string E0004 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string E0005 { get; set; }
        /// <sustring>
        ///    string
        /// </sstringy>
        public string E0006 { get; set; }
        /// <sustring>
        ///    string
        /// </sstringy>
        public string E0008 { get; set; }
        /// <sustring>
        ///    string
        /// </sstringy>
        public string E0148 { get; set; }
        /// <sustring>
        ///    string
        /// </sstringy>
        public string E0149 { get; set; }
        /// <sustring>
        ///    string
        /// </sstringy>
        public string E0156 { get; set; }
        /// <sustring>
        ///    string
        /// </sstringy>
        public string E0077 { get; set; }
        /// <sustring>
        ///    string
        /// </sstringy>
        public string E0002 { get; set; }
        /// <sustring>
        ///    string
        /// </sstringy>
        public string E0000 { get; set; }
        /// <sustring>
        ///    string
        /// </sstringy>
        public string E0154 { get; set; }
        /// <sustring>
        ///    string
        /// </sstringy>
        public string E0075 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        //[XmlElement("section")]
        //public SectionItem sectionS0048 { get; set; }

        [XmlElement("section")]
        public  SectionItem [] sectionS0076 { get; set; }
    }
}
