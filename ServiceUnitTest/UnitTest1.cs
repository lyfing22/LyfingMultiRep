using System;
using System.IO;
using System.Net;
using System.Net.Http;
using HISInterfaceService.Core.DataMapper;
using HISInterfaceService.Core.Encrypt;
using HISInterfaceService.Core.EntityModel;
using HISInterfaceService.Core.Log;
using HISInterfaceService.Dapper.Repository;
using HISInterfaceService.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ServiceUnitTest
{
    [TestClass]
    public class UnitTest1
    {

        public UnitTest1()
        {
            LoggerFactory.SetCurrent(new EnterpriseLogFactory());
            MapperConfigReg.Register();
        }
        [TestMethod]
        public void TestMethod1()
        {
            PatientRepository patientRep = new PatientRepository();
      bool falg=      patientRep.Update(Guid.Parse("4E258DDD-64AE-4DF9-9D8F-D83DDD6CBFC3"),
                new {Email = "123@qq.com", Note = "备注", Telephone = "13250947512"});
            string s;
            var entity = new Patient
            {
                Id = Guid.NewGuid(),
                DateOfBirth = DateTime.Now,
                Email = "1221@qq.com",
                Gender = "男",
                Name = "lyfing",
                Note = "note",
                GlobalPatientId = "2222",
                PatientMasterIndex = "index",
                SpellName = "chowlyfing",
                SocietyNumber = "211x",
                ClinicalNumber = "1213",
                InpatientNumber = "sdfsdfs",
                NationName = "china",
                Telephone = "142355545",
                MergeId = Guid.NewGuid(),
                HospitalId = Guid.NewGuid(),
                LastUpdateDateTime = DateTime.Today,
                CreateDateTime = DateTime.Today,
                IDCard = "21313",
                Address = "dsfsd",
                Marriage = "yes",
                FamilyName = "chow",
                GivenName = "lyifng",
                MiddleName = "lyfing",
                HISPatientId = "dsfdsfsdf",
                ServerNode = "hina",
                IsDelete = false,
                UpdateUserCode = "dfs",
                UpdateUserDesc = "dsfs",
                UpdateDate = DateTime.Now,
                UpdateTime = null
            };
            //patientRep.Insert(entity);
        }

        [TestMethod]
        public void TestOrderInsert()
        {
            OrderRepository orderRep = new OrderRepository();
            Order order = orderRep.Insert(new Order()
            {
                Id = Guid.NewGuid(),
                HisOrderCode = "sdfsf",
                SyncStatus = 1,
                IsSynced = true,
                IsActive = true,
                IsDeleted = true,
            });
            //Order order = orderRep.Get(Guid.Parse("A4442296-35D7-4C4E-8B1A-962DFC8920D3"));
            string s = "";

        }

        [TestMethod]
        public void TestOrderPush()
        {
            Guid orderId = Guid.Parse("64F2273C-5D63-46F5-AFEE-6C3CA2DC9FC5");
            bool flag = new HisDataPushService().PostDataToHina(orderId);
            string s = "";

        }

        [TestMethod]
        public void TestDownHttp()
        {
            string path = "http://192.168.88.39:8024/2.pdf";
            string filePath = "d:\\1.pdf";
            HttpClient client = new HttpClient();
            byte[] buffer = new byte[2048];
            int readLength = 0;
            using (Stream s = client.GetStreamAsync(path).Result)
            {
                using (Stream sw = File.Create(filePath))
                {
                    while (s.Read(buffer, 0, buffer.Length) > 0)
                    {
                        sw.Write(buffer, 0, buffer.Length);
                    }
                }
            }
        }

        [TestMethod]
        public void TestDownFtp()
        {
            string userName = "reportViewer";
            string psd = "hinacom";
            string path = "ftp://192.168.88.39/lyfing.pdf";
            string filePath = "d:\\2.pdf";
            Ftpdownloadfile( path, filePath, userName, psd);


        }

        [TestMethod]
        public void TestBase64()
        {
            string str = "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiPz4KPGNsaW5pY2FsRG9jdW1lbnQ+DQogIDxkb2N1bWVudEhlYWRlcj4NCiAgICA8cmVhbG1Db2RlPjwvcmVhbG1Db2RlPg0KICAgIDx0eXBlSWQ+PC90eXBlSWQ+DQogICAgPHRlbXBsYXRlPjwvdGVtcGxhdGU+DQogICAgPGlkPjwvaWQ+DQogICAgPHRpdGxlPuajgOafpeaKpeWRijwvdGl0bGU+DQogICAgPGVmZmVjdGl2ZVRpbWU+MjAxOTExMjcxNjAzMDY8L2VmZmVjdGl2ZVRpbWU+DQogICAgPGNvbmZpZGVudGlhbGl0eSBjb2RlPSLnuqfliKvku6PnoIEiPjwvY29uZmlkZW50aWFsaXR5Pg0KICAgIDx2ZXJzaW9uTnVtYmVyPjwvdmVyc2lvbk51bWJlcj4NCiAgICA8YXV0aG9yIGlkPSLljLvnlJ/nvJblj7ciPuWFs+iIkuWFgzwvYXV0aG9yPg0KICAgIDxjdXN0b2RpYW4+PC9jdXN0b2RpYW4+DQogICAgPHBhdGllbnQ+DQogICAgICA8bWVkaWNhcmVObz4wMDAyMTM4NjY3PC9tZWRpY2FyZU5vPg0KICAgICAgPGFkbXZpc2l0Tm8+MTIxNDAzODA8L2FkbXZpc2l0Tm8+DQogICAgICA8bWVkUmVjb3JkTm8+PC9tZWRSZWNvcmRObz4NCiAgICAgIDxoZWFsdGhDYXJkTm8+XjA4MDIwODkyNTAs5rOo5YaM5Yy755aX5Y2hLEQsXjA4MDk1NjY1OTgsPC9oZWFsdGhDYXJkTm8+DQogICAgICA8Y2VydGlmaWNhdGU+DQogICAgICAgIDxuYW1lIGNvZGU9IuivgeS7tuexu+Wei+S7o+eggSI+6Lqr5Lu96K+BPC9uYW1lPg0KICAgICAgICA8dmFsdWU+NDEwMTA1MTk1NjA0MTU4NDEyPC92YWx1ZT4NCiAgICAgIDwvY2VydGlmaWNhdGU+DQogICAgICA8YWRkciBkZXNjPSLnjrDkvY/lnYAiPg0KICAgICAgICA8dGV4dD7pg5Hlt57luILph5HmsLTljLrnuqzkuozot6/vvJjlj7fpmaLvvJHvvJXlj7fmpbzvvJblj7cgPC90ZXh0Pg0KICAgICAgICA8aG91c2VOdW1iZXI+PC9ob3VzZU51bWJlcj4NCiAgICAgICAgPHN0cmVldE5hbWU+PC9zdHJlZXROYW1lPg0KICAgICAgICA8dG93bnNoaXA+PC90b3duc2hpcD4NCiAgICAgICAgPGNvdW50eT48L2NvdW50eT4NCiAgICAgICAgPGNpdHk+PC9jaXR5Pg0KICAgICAgICA8c3RhdGU+PC9zdGF0ZT4NCiAgICAgICAgPHBvc3RhbENvZGU+PC9wb3N0YWxDb2RlPg0KICAgICAgPC9hZGRyPg0KICAgICAgPG5hbWU+5pyx5YWD6LaFPC9uYW1lPg0KICAgICAgPHRlbGVjb20+MTM2NTM3MTg3OTE8L3RlbGVjb20+DQogICAgICA8YWRtaW5pc3RyYXRpdmVHZW5kZXIgY29kZT0iMSIgLz4NCiAgICAgIDxtYXJpdGFsU3RhdHVzIGNvZGU9IjIwIj48L21hcml0YWxTdGF0dXM+DQogICAgICA8ZXRobmljR3JvdXAgY29kZT0iMDEiPjwvZXRobmljR3JvdXA+DQogICAgICA8YWdlIHVuaXQ9IuWygSI+NjM8L2FnZT4NCiAgICA8L3BhdGllbnQ+DQogICAgPHBhcnRpY2lwYW50Pg0KICAgICAgPGNvZGUgY29kZT0iMSI+PC9jb2RlPg0KICAgICAgPGFkZHIgZGVzYz0iIj4NCiAgICAgICAgPHRleHQ+6YOR5bee5biC6YeR5rC05Yy657qs5LqM6Lev77yY5Y+36Zmi77yR77yV5Y+35qW877yW5Y+3IDwvdGV4dD4NCiAgICAgICAgPGhvdXNlTnVtYmVyPjwvaG91c2VOdW1iZXI+DQogICAgICAgIDxzdHJlZXROYW1lPjwvc3RyZWV0TmFtZT4NCiAgICAgICAgPHRvd25zaGlwPjwvdG93bnNoaXA+DQogICAgICAgIDxjb3VudHk+PC9jb3VudHk+DQogICAgICAgIDxjaXR5PjwvY2l0eT4NCiAgICAgICAgPHN0YXRlPjwvc3RhdGU+DQogICAgICAgIDxwb3N0YWxDb2RlPjwvcG9zdGFsQ29kZT4NCiAgICAgIDwvYWRkcj4NCiAgICAgIDx0ZWxlY29tPjwvdGVsZWNvbT4NCiAgICAgIDxuYW1lPjwvbmFtZT4NCiAgICA8L3BhcnRpY2lwYW50Pg0KICA8L2RvY3VtZW50SGVhZGVyPg0KICA8c3RydWN0dXJlZEJvZHk+DQogICAgPEUwMDA0IGRlc2M9IuWnk+WQjSI+5pyx5YWD6LaFPC9FMDAwND4NCiAgICA8RTAwMDUgZGVzYz0i5oCn5Yir5Luj56CBIj48L0UwMDA1Pg0KICAgIDxFMDAwNiBkZXNjPSLmgKfliKvmj4/ov7AiPueUtzwvRTAwMDY+DQogICAgPEUwMDA4IGRlc2M9IuW5tOm+hCI+NjPlsoE8L0UwMDA4Pg0KICAgIDxFMDE0OCBkZXNjPSLnl4Xkurrnsbvlnovku6PnoIEiPklOUDwvRTAxNDg+DQogICAgPEUwMTQ5IGRlc2M9IueXheS6uuexu+Wei+aPj+i/sCI+SU5QPC9FMDE0OT4NCiAgICA8RTAxNTYgZGVzYz0i56eR5a6kaWQiPjwvRTAxNTY+DQogICAgPEUwMDc3IGRlc2M9IuenkeWupCI+5pS+5bCE56eRPC9FMDA3Nz4NCiAgICA8RTAwMDIgZGVzYz0i5L2P6Zmi5Y+3Ij4wMDAyMTM4NjY3PC9FMDAwMj4NCiAgICA8RTAwMDAgZGVzYz0i6Zeo6K+K5Y+3Ij48L0UwMDAwPg0KICAgIDxFMDE1NCBkZXNjPSLnl4XluoppZCI+PC9FMDE1ND4NCiAgICA8RTAwNzUgZGVzYz0i55eF5bqKIj4wMDA15bqKPC9FMDA3NT4NCiAgICA8c2VjdGlvbiBjb2RlPSJTMDA0OCIgZGVzYz0i6K+K5patIj4NCiAgICAgIDx0ZXh0PiAxOiDkuIDov4fmgKfmmZXljqXlvoXmn6XvvJrmgKXmgKfohJHooYDnrqHnl4XvvJ/lv4PmupDmgKfmmZXljqXvvJ8gMjog6Jub572R6Iac5LiL6IWU5Ye66KGAIDM6IOiEkeWupOezu+e7n+WwkemHj+enr+ihgCA0OiDpq5jooYDljosz57qn77yI5p6B6auY5Y2x77yJIDU6IOS6jOWei+ezluWwv+eXhTwvdGV4dD4NCiAgICAgIDxFMDcgZGVzYz0i5Yy755Sf5aGr5YaZ55qE6K+K5patIj48L0UwNz4NCiAgICAgIDxFMDEgZGVzYz0i6K+K5pat5ZCN56ewIj48L0UwMT4NCiAgICAgIDxFMDIgZGVzYz0i6K+K5pat5Luj56CBIj48L0UwMj4NCiAgICA8L3NlY3Rpb24+DQogICAgPHNlY3Rpb24gY29kZT0iUzAwNzYiIGRlc2M9IuajgOafpeiusOW9lSI+DQogICAgICA8dGV4dCAvPg0KICAgICAgPEUwMSBkZXNjPSLmo4Dmn6Xlj7ciPjExMzY1Njc5LTAyNDwvRTAxPg0KICAgICAgPEUwMiBkZXNjPSLmo4Dmn6Xorr7lpIfnvJbnoIEiPjEwMTQ8L0UwMj4NCiAgICAgIDxFMDMgZGVzYz0i5qOA5p+l6K6+5aSH5ZCN56ewIj5EUjfvvIjluorml4HvvIk8L0UwMz4NCiAgICAgIDxFMDQgZGVzYz0i5qOA5p+l6YOo5L2N5Luj56CBIj7luorovrnog7jpg6jmraPkvY08L0UwND4NCiAgICAgIDxFMDUgZGVzYz0i5qOA5p+l6YOo5L2N5ZCN56ewIj7luorovrnog7jpg6jmraPkvY08L0UwNT4NCiAgICAgIDxFMDYgZGVzYz0i5qOA5p+l6YOo5L2N5Yy755Sf5omL5YaZIj7luorovrnog7jpg6jmraPkvY08L0UwNj4NCiAgICAgIDxFMDcgZGVzYz0i5qOA5p+l6YOo5L2NIj7luorovrnog7jpg6jmraPkvY08L0UwNz4NCiAgICAgIDxFMDggZGVzYz0i5qOA5p+l5omA6KeBIj4gICAgICDlj4zogrrnurnnkIblop7lpJrvvIzlj4zogrrph47op4HlsJHorrjmqKHns4rlsI/niYfnirblr4bluqblop7pq5jlvbHvvIzmsJTnrqHlsYXkuK3vvIznurXpmpTlsYXkuK3vvIzlv4PlvbHnqI3lpKfvvIzlj4zkvqfohojpnaLlhYnmlbTvvIzlj4zkvqfogovohojop5LplJDliKnjgILlj7Pkvqfpoojpg6jop4HnlZnnva7nrqHlvbHvvIzmnKvnq6/op4Llr5/kuI3muIXjgIINCjwvRTA4Pg0KICAgICAgPEUwOSBkZXNjPSLmo4Dmn6Xnu5PmnpwiPjHjgIHlj4zogrrngo7nl4flj6/og73vvIzovoMyMDE5LTExLTIz5YmN54mH5aSn6Ie055u45Lu/77ybDQoy44CB5b+D5b2x56iN5aSn77ybDQotLS0tIOS7peS4iuivt+e7k+WQiOS4tOW6iuWPiuWFtuS7luebuOWFs+ajgOafpeOAgg0KPC9FMDk+DQogICAgICA8RTEwIGRlc2M9IuajgOafpeWMu+W4iOS7o+eggSI+WVhTWDIwMTkwMTg0PC9FMTA+DQogICAgICA8RTExIGRlc2M9IuajgOafpeWMu+W4iOWnk+WQjSI+5p2o5aSp6bmPPC9FMTE+DQogICAgICA8RTEyIGRlc2M9IuWuoeaguOWMu+W4iOS7o+eggSI+MDA4MTYxPC9FMTI+DQogICAgICA8RTEzIGRlc2M9IuWuoeaguOWMu+W4iOWnk+WQjSI+5YWz6IiS5YWDPC9FMTM+DQogICAgICA8RTE0IGRlc2M9IuiusOW9leiAheS7o+eggSI+WVhTWDIwMTkwMTg0PC9FMTQ+DQogICAgICA8RTE1IGRlc2M9IuiusOW9leiAheWnk+WQjSI+WVhTWDIwMTkwMTg0PC9FMTU+DQogICAgICA8RTE2IGRlc2M9IuajgOafpeaXpeacnyI+MjAxOS0xMS0yNzwvRTE2Pg0KICAgICAgPEUxNyBkZXNjPSLmo4Dmn6Xml7bpl7QiPjEzOjI2OjE0PC9FMTc+DQogICAgICA8RTE4IGRlc2M9IuaKpeWRiuWPtyI+Mzc5MDI5MTwvRTE4Pg0KICAgICAgPEUxOSBkZXNjPSLlrqHmoLjml6XmnJ8iPjIwMTktMTEtMjc8L0UxOT4NCiAgICAgIDxFMjAgZGVzYz0i5a6h5qC45pe26Ze0Ij4xNjowMjo1ODwvRTIwPg0KICAgICAgPEUyMSBkZXNjPSLmo4Dmn6Xpobnnm67mj4/ov7AiPuW6iuaXgeaVsOWtl+WMluaRhOW9se+8iERSKSjlhbbku5YpKOW6iui+ueiDuOmDqOato+S9jSk8L0UyMT4NCiAgICAgIDxFMjIgZGVzYz0i5qOA5p+l5pa55rOV5o+P6L+wIj48L0UyMj4NCiAgICAgIDxFMjMgZGVzYz0i5oql5ZGK5pel5pyfIj4yMDE5LTExLTI3PC9FMjM+DQogICAgICA8RTI0IGRlc2M9IuaKpeWRiuaXtumXtCI+MTQ6NTM6NDM8L0UyND4NCiAgICAgIDxFMjUgZGVzYz0i5Yy75Zix5Y+3Ij4xMTM2NTY3OXx8MTU0NjwvRTI1Pg0KICAgIDwvc2VjdGlvbj4NCiAgPC9zdHJ1Y3R1cmVkQm9keT4NCjwvY2xpbmljYWxEb2N1bWVudD4=";
            string result = Base64Helper.Base64Decoede(str);

        }


        private void Ftpdownloadfile(string to_uri, string path, string username, string password)
        {

            FtpWebRequest request =
            (FtpWebRequest)FtpWebRequest.Create(new Uri(to_uri));

            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.Credentials = new NetworkCredential(username, password);          
            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (FileStream fs = new FileStream(path, FileMode.Create))
                    {
                        byte[] buffer = new byte[2028];
                        int read = 0;
                        while ((read = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            fs.Write(buffer, 0, read);
                            fs.Flush();
                        }
                    }
                }
            }
        }

    }
}
