using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using HISInterfaceService.Core.DataMapper;
using HISInterfaceService.Core.EntityModel;
using HISInterfaceService.Core.HisRequestModel;
using HISInterfaceService.Core.HttpReqest;
using HISInterfaceService.Core.Log;
using HISInterfaceService.Dapper.Repository;
using HISInterfaceService.Service.DbService;
using Newtonsoft.Json;
using Patient = HISInterfaceService.Core.EntityModel.Patient;

namespace HISInterfaceService.Service
{
    public class HisDataPushService
    {

        private PatientService patientService = new PatientService();
        private OrderService orderService = new OrderService();
        private ReportService reportService = new ReportService();

        /// <summary>
        /// 接收推送的病人信息
        /// </summary>
        /// <param name="action"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Response PatientRegistry(string message)
        {
            Response response = new Response() { Header = new Header { }, Body = new Body { } };
            try
            {
                LoggerFactory.CreateLog().LogRestInfo("PatientRegistry Param: " + message);
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Request));
                Request requst = xmlSerializer.Deserialize(new StringReader(message)) as Request;
                if (requst is null || requst.Body == null || requst.Body.PatientRegistryRt == null)
                {
                    response.Body.ResultCode = "-1";
                    response.Body.ResultContent = "反序列信息异常";
                    return response;
                }
                HISInterfaceService.Core.EntityModel.Patient patient = HisDataParseHelper.RequestToPatient(requst);
                if (!patient.DateOfBirth.HasValue)
                {
                    response.Body.ResultCode = "-1";
                    response.Body.ResultContent = $"病人生日不允许为空！PatientId:{patient.GlobalPatientId}";
                    return response;
                }
                var result = patientService.AddOrUpdate(patient);
                response.Body.ResultCode = result != null ? "0" : "1";
                response.Body.ResultContent = result != null ? "创建病人成功" : "创建病人异常";
            }
            catch (Exception ex)
            {
                response.Body.ResultCode = "-1";
                response.Body.ResultContent = "反序列信息异常:" + ex.ToString();
                LoggerFactory.CreateLog().LogError("PatientRegistry Exception" + ex.Message, ex);
            }
            return response;
        }

        /// <summary>
        /// 申请单
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public Response AddRisAppBill(string message)
        {
            Response response = new Response() { Header = new Header { }, Body = new Body { } };
            try
            {
                LoggerFactory.CreateLog().LogRestInfo("AddRisAppBill Param: " + message);

                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Request));

                Request requst = xmlSerializer.Deserialize(new StringReader(message)) as Request;
                if (requst is null || requst.Body == null || requst.Body.AddRisAppBillRt == null)
                {
                    response.Body.ResultCode = "-1";
                    response.Body.ResultContent = "反序列信息异常";
                    return response;
                }
                List<Order> list = HisDataParseHelper.RequestToOrder(requst, patientService);
                foreach (var order in list)
                {
                    try
                    {
                        //如果预约时间不为空，则需要写入到正式库一份
                        if (order.ScheduledDateTime.HasValue && !string.IsNullOrWhiteSpace(order.DeviceName))
                        {
                            Patient patient = patientService.GetPatientByGlobalPatientId(order.GloablePatientId);
                            if (patient == null || string.IsNullOrWhiteSpace(patient.GlobalPatientId))
                            {
                                response.Body.ResultCode = "-1";
                                response.Body.ResultContent += "未根据PatientId查询到病人信息，不进行预约写入；PatientId:" + order.GloablePatientId;
                                continue;
                            }
                            var caseForThirdPart = HisDataParseHelper.OrderToCaseForThirdPart(order, patient);
                            string resultStr = HttpRequestHelper.GetHttpResponse<CaseForThirdPartDTO>("http://localhost/REST/ThirdPart/CreateOrModifyOrder", caseForThirdPart);
                            CaseUploadResult caseUploadResult = JsonConvert.DeserializeObject<CaseUploadResult>(resultStr);
                            if (caseUploadResult.ResultCode != 1000)
                            {
                                response.Body.ResultCode = "-1";
                                response.Body.ResultContent += $"创建预约申请单异常：HisOrderCode：{order.HisOrderCode},ErrorInfo: {caseUploadResult.ResultDescription}；";
                                LoggerFactory.CreateLog().LogError($"创建预约申请单异常：HisOrderCode：{order.HisOrderCode},ErrorInfo: {caseUploadResult.ResultDescription}；");
                            }
                        }
                        orderService.DeleteOrder(order, false);
                        orderService.CreateOder(order);
                    }
                    catch (Exception ex)
                    {
                        response.Body.ResultCode = "-1";
                        response.Body.ResultContent += ex.ToString() + "；";
                        LoggerFactory.CreateLog().LogError("AddRisAppBill Exception：" + ex.ToString(), ex);
                    }
                }
                response.Body.ResultCode = response.Body.ResultCode == "-1" ? "-1" : "0";
                response.Body.ResultContent = response.Body.ResultCode == "-1" ? response.Body.ResultContent : "success";
            }
            catch (Exception ex)
            {
                response.Body.ResultCode = "-1";
                response.Body.ResultContent = "反序列信息异常:" + ex.ToString();
                LoggerFactory.CreateLog().LogError("PatientRegistry Exception" + ex.Message, ex);
            }
            return response;
        }

        /// <summary>
        /// 报告
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public Response RegisterDocument(string message)
        {
            Response response = new Response() { Header = new Header { }, Body = new Body { } };
            try
            {
                LoggerFactory.CreateLog().LogRestInfo("AddRisAppBill Param: " + message);

                string sourceSystem = string.Empty;
                string messageID = string.Empty;
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Request));
                Request requst = xmlSerializer.Deserialize(new StringReader(message)) as Request;
                if (requst is null || requst.Body == null || requst.Body.RegisterDocumentRt == null)
                {
                    response.Body.ResultCode = "-1";
                    response.Body.ResultContent = "反序列信息异常";
                    return response;
                }
                Report report = HisDataParseHelper.RequestToReport(requst, orderService);
                if (report == null)
                {
                    response.Header.SourceSystem = sourceSystem;
                    response.Header.MessageID = messageID;
                    response.Body.ResultCode = "-1";
                    response.Body.ResultContent = "未找到该报告对应的申请单";
                    return response;
                }
                reportService.Insert(report);
                response.Body.ResultCode = "0";
                response.Body.ResultContent = "success";
                //推送数据
                PostDataToHina(report.Order_Id);
                return response;
            }
            catch (Exception ex)
            {
                response.Body.ResultCode = "-1";
                response.Body.ResultContent = "服务异常:" + ex.ToString();
                LoggerFactory.CreateLog().LogError("RegisterDocument Exception" + ex.Message, ex);
                return response;
            }
        }



        /// <summary>
        /// 构造推送接口post类对象
        /// </summary>
        /// <returns></returns>
        private OrderViewDTOForApi GetOrderViewForApi(Guid orderId)
        {
            OrderViewDTOForApi model = new OrderViewDTOForApi
            {
                ServerNode = "HINAMIIS",
                HospitalCode = "Hospital201700001",
                RuningFeature = ""
            };
            //return model;
            Order order = orderService.GetOrderById(orderId);
            if (order == null) return null;
            Patient patient = patientService.GetPatientByGlobalPatientId(order.GloablePatientId);
            if (patient == null) return null;
            PatientForUploadDTOForApi patientInfo = patient.MapTo<PatientForUploadDTOForApi>();
            if (patientInfo == null) return null;
            VisitForUploadDTOForApi visitInfo = new VisitForUploadDTOForApi
            {
                ClinicalNumber = order.ClinicNumber,
                InpatientNumber = order.InhospitalNumber,
                MedicalRecordNumber = "",
                VisitSerialNumber = order.VisitNumber,
                PatientType = order.PatientType,
                BedNumber = order.BedNo,
                RoomNumber = order.RoomNo,
                ClinicalDiagnosis = order.ClinicalDiagnosis,
                AllergyHistory = order.DiseaseHistory
            };
            OrderForUploadDTOForApi orderInfo = order.MapTo<OrderForUploadDTOForApi>();
            Report report = reportService.GetReportByOrderId(orderId);
            ReportForUploadDTOForApi reportInfo = report.MapTo<ReportForUploadDTOForApi>();
            orderInfo.ReportDoctorCode = report.SubmitDoctorCode;
            orderInfo.ReportDoctorName = report.SubmitDoctorName;
            orderInfo.DeviceCode = order.ModalityCode;
            orderInfo.DeviceName = order.DeviceName;
            orderInfo.ModalityCode = order.ModalityCode;
            orderInfo.ModalityName = order.DeviceName;
            orderInfo.StudyId = orderInfo.AccessionNumber;
            orderInfo.Status = "Reported";
            model.PatientInfo = patientInfo;
            model.VisitInfo = visitInfo;
            model.OrderInfo = orderInfo;
            model.ReportInfo = reportInfo;
            return model;
        }

        /// <summary>
        /// 推送数据
        /// </summary>
        /// <returns></returns>
        public bool PostDataToHina(Guid orderId)
        {
            OrderViewDTOForApi postData = GetOrderViewForApi(orderId);
            ThirdResponseModel res = HttpRequestHelper.PostDataToHina(postData);
            if (res == null) return false;
            if (res.Success) return true;
            else
            {
                LoggerFactory.CreateLog().LogError("推送中间表数据到Ris失败" + res.Message);
                return false;
            }
        }

    }
}
