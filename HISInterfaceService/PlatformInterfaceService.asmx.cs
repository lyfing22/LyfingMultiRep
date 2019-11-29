using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using HISInterfaceService.Core.HisRequestModel;
using HISInterfaceService.Core.Log;
using HISInterfaceService.ErrorHandler;
using HISInterfaceService.Service;

namespace HISInterfaceService
{
    /// <summary>
    /// PlatformInterfaceService 的摘要说明
    /// </summary>
  
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class PlatformInterfaceService : System.Web.Services.WebService
    {
        private HisDataPushService dataPushService = new HisDataPushService();
        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        /// <summary>
        /// 接收推送的病人信息
        /// </summary>
        /// <param name="action"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [WebMethod]
        public Response PatientRegistry(string message)
        {
            return dataPushService.PatientRegistry(message);
        }
        /// <summary>
        /// 接收推送的病人信息
        /// </summary>
        /// <param name="action"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [WebMethod]
        public Response AddRisAppBill(string message)
        {
            return dataPushService.AddRisAppBill(message);
        }
        /// <summary>
        /// 接收推送的病人信息
        /// </summary>
        /// <param name="action"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [WebMethod]
        public Response RegisterDocument(string message)
        {
            return dataPushService.RegisterDocument(message);
        }

    }

}
