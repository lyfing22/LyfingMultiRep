using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using HISInterfaceService.Core.Encrypt;
using HISInterfaceService.Core.EntityModel;
using HISInterfaceService.Core.Enum;
using HISInterfaceService.Core.HisRequestModel;
using HISInterfaceService.Core.HttpReqestHelper;
using HISInterfaceService.Core.Log;
using HISInterfaceService.Service.DbService;
using Microsoft.International.Converters.PinYinConverter;
using Patient = HISInterfaceService.Core.EntityModel.Patient;

namespace HISInterfaceService.Service
{
    public class HisDataParseHelper
    {

        public static HISInterfaceService.Core.EntityModel.Patient RequestToPatient(Request request)
        {
            try
            {
                PatientRegistryRt rt = request.Body.PatientRegistryRt;
                HISInterfaceService.Core.EntityModel.Patient patient = new HISInterfaceService.Core.EntityModel.Patient();
                patient.Address = rt.PATAddressList != null && rt.PATAddressList.PATAddress != null ? rt.PATAddressList.PATAddress.PATAddressDesc : "";
                patient.DateOfBirth = ConvertToDateTime(rt.PATDob);
                patient.GlobalPatientId = rt.PATPatientID;
                patient.HISPatientId = rt.PATPatientID;
                //patient.PatientMasterIndex
                patient.Name = rt.PATName;
                patient.SpellName = ToPinYinWithoutBlank(patient.Name);
                patient.Gender = rt.PATSexCode == "1" ? "M" : rt.PATSexCode == "2" ? "F" : "U";
                patient.Marriage = rt.PATMaritalStatusDesc;
                patient.NationName = rt.PATNationDesc;
                patient.SocietyNumber = rt.PATHealthCardID;
                patient.IDCard = rt.PATIdentityList != null && rt.PATIdentityList.PATIdentity != null ? rt.PATIdentityList.PATIdentity.PATIdentityNum : "";
                patient.Telephone = rt.PATTelephone;
                patient.UpdateUserCode = rt.UpdateUserCode;
                patient.UpdateUserDesc = rt.UpdateUserDesc;
                patient.UpdateDate = ConvertToDateTime(rt.UpdateDate);
                patient.UpdateTime = ConvertToTimeSpan(rt.UpdateTime);
                patient.LastUpdateDateTime = DateTime.Now;
                patient.Id = Guid.NewGuid();
                patient.Note = rt.PATRemarks;
                patient.CreateDateTime = DateTime.Now;
                //HospitalId,serviceNode,ClinicalNumber,InpatientNumber,Eamil
                return patient;
            }
            catch (Exception ex)
            {
                //LoggerFactory.CreateLog().LogError("转换病人未处理异常" + ex.Message, ex);
                throw ex;
            }
        }


        public static List<Order> RequestToOrder(Request request, PatientService patService)
        {
            List<Order> list = new List<Order>();
            foreach (var rt in request.Body.AddRisAppBillRt)
            {
                var order = ConvertOrder(rt, patService);
                if (order != null)
                    list.Add(order);
            }
            return list;
        }

        public static Report RequestToReport1(string xmlContent, OrderService orderService, out string sourceSystem, out string messageID)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlContent);
            XmlNode rootNode = doc.SelectSingleNode("Request");
            XmlElement header = (XmlElement)rootNode.SelectSingleNode("Header");
            XmlElement body = (XmlElement)rootNode.SelectSingleNode("Body/AddRisAppBillRt");
            sourceSystem = header.GetElementsByTagName("SourceSystem")[0].InnerText;
            messageID = header.GetElementsByTagName("MessageID")[0].InnerText;
            string RISRExamID = body.GetElementsByTagName("RISRExamID")[0].InnerText;//检查号RISRExamID
            string OEORIOrderItemID = body.GetElementsByTagName("OEORIOrderItemID")[0].InnerText;//医嘱号OEORIOrderItemID
            string DocumentPath = body.GetElementsByTagName("DocumentPath")[0].InnerText;
            string DocumentContent = body.GetElementsByTagName("DocumentContent")[0].InnerText;
            string accessionNum = RISRExamID + OEORIOrderItemID;
            Order order = orderService.GetOrderByAccessionNum(accessionNum);
            if (order == null) throw new ArgumentException("未找到该报告对应的申请单" + accessionNum);
            Report report = new Report()
            {
                Id = Guid.NewGuid(),
                ReportUrl = "",
                Order_Id = order.Id,
                ReportStatus = ReportStatus.Submit.ToString()
            };
            return report;
        }


        public static Report RequestToReport(Request request, OrderService orderService)
        {
            string pdfFilePath = "d:\\importData\\" + new Random().Next() + ".pdf";//存储报告路径
            string ftpUserName = "reportViewer";
            string ftpUserPwd = "hinacom";
            RegisterDocumentRt rt = request.Body.RegisterDocumentRt;
            string RISRExamID = rt.RISRExamID;//检查号RISRExamID
            string DocumentPath = rt.DocumentPath;
            pdfFilePath = FtpRequestHelper.FtpDownloadFile(DocumentPath, pdfFilePath, ftpUserName, ftpUserPwd);//下载pdf到服务器本机
            string documentContent = Base64Helper.Base64Decoede(rt.DocumentContent);//Base64机密后的报告内容
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ClinicalDocument));
            ClinicalDocument document = xmlSerializer.Deserialize(new StringReader(documentContent)) as ClinicalDocument;
            string submitDoctorCode = string.Empty;
            string submitDoctorName = string.Empty;
            if (document.documentHeader != null && document.documentHeader.author != null)
            {
                submitDoctorName = document.documentHeader.author.author;
                submitDoctorCode = document.documentHeader.author.AuthorCode;
            }
            //if (document != null && document.documentHeader != null)
            //{
            //    submitDoctorCode = document.documentHeader.id;
            //}
            string accessionNum = RISRExamID;
            Order order = orderService.GetOrderByAccessionNum(accessionNum);
            if (order == null) throw new ArgumentException("未找到该报告对应的申请单" + accessionNum);
            Report report = new Report()
            {
                Id = Guid.NewGuid(),
                ReportUrl = pdfFilePath,
                Order_Id = order.Id,
                SubmitDoctorCode = submitDoctorCode,
                SubmitDoctorName = submitDoctorName,
                ReportCreaterCode = submitDoctorCode,
                ReportCreaterName = submitDoctorName,
                ReportStatus = ReportStatus.Submit.ToString()
            };
            if (document != null && document.structuredBody != null && document.structuredBody.sectionS0076 != null
                && document.structuredBody.sectionS0076.Any(item => item.code.Equals("S0076")))
            {
                SectionItem section = document.structuredBody.sectionS0076.Where(item => item.code.Equals("S0076")).FirstOrDefault();
                report.Findings = section.E08;
                report.Impression = section.E09;
                report.ApproveDoctorCode = section.E12;
                report.ApproveDoctorName = section.E13;
                report.SubmitDateTime = GetCombineTime(section.E23, section.E24);
                report.ApproveDatetime = GetCombineTime(section.E19, section.E20);
                orderService.Update(order.Id, new
                {
                    InhospitalNumber = document.structuredBody.E0002,//住院号
                    ClinicNumber = document.structuredBody.E0000,
                    DeviceName = section.E03,
                    ModalityCode = section.E02,
                    BodyPartCodes = string.IsNullOrEmpty(section.E04) ? order.BodyPartCodes : section.E04,
                    BodyPartNames = string.IsNullOrEmpty(section.E05) ? order.BodyPartNames : section.E05,
                    ExamDoctorCode = section.E10,
                    ExamDoctorName = section.E11,
                    StudyDateTime = GetCombineTime(section.E16, section.E17)
                });
            }
            return report;
        }

        private static DateTime? GetCombineTime(string date, string time)
        {
            DateTime dateTime;
            if (string.IsNullOrEmpty(date)) return null;
            if (string.IsNullOrEmpty(time)) DateTime.TryParse(date, out dateTime);
            else DateTime.TryParse(date + " " + time, out dateTime);
            return dateTime;
        }


        private static Order ConvertOrder(AddRisAppBillRtItem rt, PatientService patService)
        {
            try
            {
                Order order = new Order();
                order.Id = Guid.NewGuid();
                order.OrderStatus = "Reported";
                //order.HisOrderCode = rt.OEORIOrderItemID.Replace("||", "@@");//将提取过来的|替换给@@
                order.GloablePatientId = rt.PATPatientID;
                order.HisPatientId = rt.PATPatientID;
                order.AccessionNumber = rt.RISRExamID;
                Patient patient = patService.GetPatientByGlobalPatientId(order.GloablePatientId);
                if (patient != null)
                {
                    order.PatientName = patient.Name;
                    order.NameInPy = patient.SpellName;
                    order.Birthday = patient.DateOfBirth.ToString();
                    order.Gender = patient.Gender;
                    order.Address = patient.Address;
                    order.Telephone = patient.Telephone;
                    order.IdCard = patient.IDCard;
                    order.SocietyNumber = patient.SocietyNumber;
                }
                order.VisitNumber = rt.PAADMVisitNumber;
                order.PatientType = rt.PAADMEncounterTypeCode == "O" ? "OP" : rt.PAADMEncounterTypeCode == "H" ? "PE" : rt.PAADMEncounterTypeCode == "E" ? "EM" : "IH";
                if (order.PatientType == "IH")
                {
                    order.InhospitalNumber = rt.DocumentID;//？？病区PAADMAdmWardCode
                }
                //else
                //{
                //    order.ClinicNumber = rt.DocumentID;
                //}
                order.OrderDepartmentCode = rt.AppDeptDesc;
                order.OrderDepartmentName = rt.AppDeptDesc;
                //order.ExamDepartmentName = rt.RISRAcceptDeptCode;
                order.ExamDepartmentCode = rt.RISRAcceptDeptCode;
                //使用TestResults 储存紧急程度
                order.TestResults = rt.RISRISEmergency;

                order.OrderDateTime = ConvertToDateTime(rt.RISRSubmitTime);
                order.BedNo = rt.PAADMCurBedNo;
                //order.Notes = rt.RISRMattersAttention;
                //order.OrderExamCode = rt.RISRCode;
                //order.OrderExamName = rt.RISRDesc;
                order.ClinicalDiagnosis = rt.RISRClinicalSymptoms;
                order.DiseaseHistory = rt.RISRSpecalMedicalRecord;
                order.Symptom = rt.RISRClinicalSymptoms;
                order.ApplyDoctorName = rt.RISRSubmitDocDesc;
                order.ApplyDoctorCode = rt.RISRSubmitDocCode;
                order.CheckInDoctorCode = rt.RISRSubmitDocCode;
                order.CheckInDoctorName = rt.RISRSubmitDocDesc;
                order.OrderDoctorName = rt.RISRSubmitDocDesc;
                order.ScheduledDateTime = ConvertToDateTime(rt.SCHEDULED_DATE_TIME);
                order.clinic = rt.clinic;
                order.DeviceName = rt.clinic;
                //order.ModalityCode = rt.clinic;
                order.UpdateUserCode = rt.UpdateUserCode;
                order.UpdateDate = ConvertToDateTime(rt.UpdateDate);
                order.UpdateTime = ConvertToTimeSpan(rt.UpdateTime);
                double totalCosts = 0.00;
                foreach (var item in rt.OrderList.Order)
                {
                    if (!string.IsNullOrWhiteSpace(order.OrderExamName))
                    {
                        order.OrderExamName += "|";
                        order.OrderExamCode += "|";
                        order.BodyPartCodes += "|";
                        order.BodyPartNames += "|";
                    }
                    order.HisOrderCode = item.OEORIOrderItemID.Replace("||", "__");//将提取过来的|替换给@@
                    order.OrderExamName += item.RISRDesc;
                    order.OrderExamCode += item.RISRCode;
                    order.BodyPartCodes += item.RISRPositionCode;
                    order.BodyPartNames += item.RISRPostureCode;
                    order.OrderExamCosts += item.RISRPrice;
                    double price = 0;
                    double.TryParse(item.RISRPrice, out price);
                    totalCosts += price;
                }
                //order.AccessionNumber = string.Format("{0}_{1}", rt.RISRAppNum, order.HisOrderCode);

                order.TotalCosts = totalCosts;
                return order;
            }
            catch (Exception ex)
            {
                LoggerFactory.CreateLog().LogError("Order对象转换异常:" + ex.ToString());
            }
            return null;
        }

        public static CaseForThirdPartDTO OrderToCaseForThirdPart(Order order, HISInterfaceService.Core.EntityModel.Patient patient)
        {
            CaseForThirdPartDTO dto = new CaseForThirdPartDTO();
            dto.ServerNode = "HINAMIIS";

            dto.VisitInfo = new VisitForThirdPartDTO();
            dto.PatientInfo = new PatientForThirdPartDTO();
            dto.PatientInfo.Address = patient.Address;
            dto.PatientInfo.DateOfBirth = patient.DateOfBirth.ToString();
            dto.PatientInfo.GlobalPatientId = patient.GlobalPatientId;
            dto.PatientInfo.Name = patient.Name;
            dto.PatientInfo.Gender = patient.Gender;
            dto.PatientInfo.SocietyNumber = patient.SocietyNumber;
            dto.PatientInfo.IDCard = patient.IDCard;
            dto.PatientInfo.Telephone = patient.Telephone;
            dto.PatientInfo.SpellName = patient.SpellName;
            if (string.IsNullOrWhiteSpace(dto.PatientInfo.SpellName))
            {
                dto.PatientInfo.SpellName = ToPinYinWithoutBlank(dto.PatientInfo.Name);
            }
            dto.OrderInfo = new OrderForThirdPartDTO();
            dto.OrderInfo.HisOrderCode = order.HisOrderCode;
            dto.OrderInfo.IntegrationSourceCode = "Platform";
            //dto.OrderInfo.AccessionNumber = order.AccessionNumber;
            dto.OrderInfo.ApplyDepartmentCode = order.OrderDepartmentCode;
            dto.OrderInfo.ApplyDoctorName = order.ApplyDoctorName;
            dto.OrderInfo.ApplyDoctorCode = order.ApplyDoctorCode;
            dto.OrderInfo.ExecDepartmentCode = order.ExamDepartmentName;
            //dto.OrderInfo.ExecDepartmentName = order.ExamDepartmentName;
            dto.OrderInfo.ExamRoom = order.ExamDepartmentName;
            dto.OrderInfo.DeviceCode = order.DeviceName;
            dto.OrderInfo.HisOrderRequestDate = order.OrderDateTime.HasValue ? order.OrderDateTime.Value.ToString() : DateTime.Now.ToString();
            dto.OrderInfo.Note = order.Notes;
            dto.OrderInfo.ApplyDoctorCode = order.OrderDoctorName;
            if (order.ScheduledDateTime.Value.Minute > 30)
            {
                dto.OrderInfo.ScheduledStartTime = order.ScheduledDateTime.Value.ToString("yyyy-MM-dd HH:31:00");
            }
            else
            {
                dto.OrderInfo.ScheduledStartTime = order.ScheduledDateTime.Value.ToString("yyyy-MM-dd HH:01:00");
            }
            dto.OrderInfo.ScheduledEndTime = order.ScheduledDateTime.Value.ToString("yyyy-MM-dd HH:mm:00");
            dto.OrderInfo.ModalityCode = order.ModalityCode;
            dto.OrderInfo.IsFromHIS = true;
            dto.VisitInfo.InpatientNumber = order.InhospitalNumber;
            int emergencyDegree = 3;
            if (int.TryParse(order.TestResults, out emergencyDegree))
            {
                dto.VisitInfo.EmergencyDegree = emergencyDegree;
            }
            string checkItems = string.Empty;

            string[] examNames = order.OrderExamName.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            string[] examCodes = order.OrderExamCode.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            string[] bodyPartCodes = order.BodyPartCodes.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            string[] bodyPartNames = order.BodyPartNames.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            string[] orderExamCosts = order.OrderExamCosts.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            if (examNames.Length == examCodes.Length)
            {
                for (int i = 0; i < examNames.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(checkItems))
                    {
                        checkItems += "|";
                    }
                    checkItems = $"{(bodyPartCodes.Length <= i ? "" : bodyPartCodes[i])},{(bodyPartCodes.Length <= i ? "" : bodyPartCodes[i])},{examCodes[i]},{examNames[i]},{(orderExamCosts.Length <= i ? "0" : orderExamCosts[i])}";
                }
            }
            dto.OrderInfo.CheckItems = checkItems;
            dto.OrderInfo.TotalFee = order.TotalCosts.HasValue ? order.TotalCosts.Value : 0.00;

            dto.VisitInfo.PatientType = order.PatientType;
            dto.VisitInfo.BedNumber = order.BedNo;
            dto.VisitInfo.ClinicalDiagnosis = order.ClinicalDiagnosis;
            dto.VisitInfo.DiseaseHistory = order.DiseaseHistory;
            dto.VisitInfo.Sign = order.Symptom;//体征
            dto.VisitInfo.VisitNumber = order.VisitNumber;

            return dto;
        }


        private static DateTime? ConvertToDateTime(string str)
        {
            DateTime dateTime;
            if (!DateTime.TryParse(str, out dateTime))
            {
                return null;
            }
            return dateTime;
        }


        private static TimeSpan? ConvertToTimeSpan(string str)
        {
            TimeSpan dateTime;
            if (!TimeSpan.TryParse(str, out dateTime))
            {
                return null;
            }
            return dateTime;
        }


        public static string ToPinYinWithoutBlank(string str)
        {
            var pinyin = string.Empty;
            foreach (var item in str.ToCharArray())
            {
                if (ChineseChar.IsValidChar(item))
                {
                    var chars = new ChineseChar(item);
                    var pinYins = chars.Pinyins[0].Substring(0, chars.Pinyins[0].Length - 1);
                    //Note:remove the last number of sound.
                    pinyin += string.Format("{0}", ToCamlFormat(pinYins));
                }
                else
                {
                    pinyin += item;
                }
            }
            return pinyin;
        }

        /// <summary>
        /// 首字母大写
        /// </summary>
        /// <param name="pinyin"></param>
        /// <returns></returns>
        public static string ToCamlFormat(string pinyin)
        {
            if (string.IsNullOrEmpty(pinyin)) return pinyin;
            var firstLetter = pinyin.Substring(0, 1).ToUpper();
            return string.Format("{0}{1}", firstLetter, pinyin.Substring(1, pinyin.Length - 1).ToLower());
        }

    }
}
