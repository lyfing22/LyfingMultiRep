using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace HISInterfaceService.Core.HisRequestModel
{
    public class Header
    {
        public Header()
        {
            SourceSystem = "PACS";
            MessageID = string.Empty;
        }
        /// <summary>
        /// 
        /// </summary>
        public string SourceSystem { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MessageID { get; set; }
    }

    public partial class Body
    {
        /// <summary>
        /// 
        /// </summary>
        public string ResultCode { get; set; }
        /// <summary>
        /// 成功
        /// </summary>
        public string ResultContent { get; set; }

        public PatientRegistryRt PatientRegistryRt { get; set; }

        [XmlElement("AddRisAppBillRt")]
        public AddRisAppBillRtItem[] AddRisAppBillRt { get; set; }

        public RegisterDocumentRt RegisterDocumentRt { get; set; }
    }

    public class Response
    {
        /// <summary>
        /// 
        /// </summary>
        public Header Header { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Body Body { get; set; }
    }
}