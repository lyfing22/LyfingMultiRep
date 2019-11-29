using System;
using System.IO;
using System.Text;
using System.Xml;
using HISInterfaceService.Core.DataMapper;
using HISInterfaceService.Core.HisRequestModel;
using HISInterfaceService.Core.Log;
using HISInterfaceService.Service;
using HISInterfaceService.Service.DbService;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ServiceUnitTest
{
    [TestClass]
    public class XmlImportTest
    {
        public XmlImportTest()
        {
            LoggerFactory.SetCurrent(new EnterpriseLogFactory());
            MapperConfigReg.Register();
        }

        [TestMethod]
        public void TestPatient()
        {
            string xmlPath = "d:\\importData\\patient.txt";
            string content = File.ReadAllText(xmlPath,Encoding.UTF8);
            Response res = new HisDataPushService().PatientRegistry(content);
        }
        [TestMethod]
        public void TestOrder()
        {
            string xmlPath = "d:\\importData\\order.txt";
            string content = File.ReadAllText(xmlPath, Encoding.UTF8);
            Response res = new HisDataPushService().AddRisAppBill(content);
        }

        [TestMethod]
        public void TestReport()
        {
            string xmlPath = "d:\\importData\\report.txt";
            string content = File.ReadAllText(xmlPath, Encoding.UTF8);
            Response res = new HisDataPushService().RegisterDocument(content);
        }




    }
}
