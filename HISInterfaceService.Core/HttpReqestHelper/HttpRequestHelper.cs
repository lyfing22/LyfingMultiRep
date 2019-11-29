using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using HISInterfaceService.Core.HisRequestModel;
using HISInterfaceService.Core.Log;
using Newtonsoft.Json;

namespace HISInterfaceService.Core.HttpReqest
{
    public class HttpRequestHelper
    {
        public static string GetHttpResponse<T>(string strUrl, T t)
        {
            var client = new HttpClient();
            client.Timeout = new TimeSpan(0, 0, 10);
            //client.TransportSettings.ConnectionTimeout = new TimeSpan(0, 0, 10);
            string strJoin = JsonConvert.SerializeObject(t);

            LoggerFactory.CreateLog().LogRestInfo("Request Param: " + strJoin);
            try
            {
                //var response = client.Post(strUrl, GetContent(t));
                var httpContent = new StringContent(strJoin, Encoding.UTF8);
                var response = client.PostAsync(strUrl, httpContent).Result;
                //response.EnsureStatusIsSuccessful();
                response.EnsureSuccessStatusCode();
                var readResult = response.Content.ReadAsStringAsync().Result;
                LoggerFactory.CreateLog().LogRestInfo("Response Result: " + readResult);
                return readResult;
            }
            catch (Exception ex)
            {
                LoggerFactory.CreateLog().LogError("Notify His failed! " + ex.Message);
                throw new Exception("Notify His failed! " + ex.Message);
            }
        }

        #region  orderReport中间数据推到Ris数据库

        public static ThirdResponseModel PostDataToHina(OrderViewDTOForApi orderView)
        {
            LoggerFactory.CreateLog().LogInfo("推送中间表数据提交", orderView);
            try
            {
                HttpClient httpClient = new HttpClient();
                string url = "http://localhost:8765/Rest/ThirdPart/UploadOrderView";// string.Empty;
                string appSecret = string.Empty;
                string orderJson = JsonConvert.SerializeObject(orderView);
                //httpClient.DefaultRequestHeaders.Add("HiAuthAppKey", SystemConfigHelper.GetSystemConfig("HiAuthAppKey"));
                //httpClient.DefaultRequestHeaders.Add("HiAuthAppVersion", SystemConfigHelper.GetSystemConfig("HiAuthAppVersion"));
                //httpClient.DefaultRequestHeaders.Add("HiAuthAppReferer", SystemConfigHelper.GetSystemConfig("HiAuthAppReferer"));

                httpClient.DefaultRequestHeaders.Add("HiAuthAppKey", "6dda099e295c469e9c3033287f632a83");
                httpClient.DefaultRequestHeaders.Add("HiAuthAppVersion", "V1.0");
                httpClient.DefaultRequestHeaders.Add("HiAuthAppReferer", "http://172.19.100.244:8765");


                string sign = GetRequestSign(url, "post", orderJson, appSecret);
                httpClient.DefaultRequestHeaders.Add("HiAuthAppSignature", sign);
                HttpContent httpContent = new StringContent(orderJson);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                httpContent.Headers.ContentType.CharSet = "utf-8";
                HttpResponseMessage response = httpClient.PostAsync(url, httpContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    String resResult = response.Content.ReadAsStringAsync().Result;
                    LoggerFactory.CreateLog().LogInfo("推送中间表数据返回", resResult);
                    if (!string.IsNullOrEmpty(resResult)) return JsonConvert.DeserializeObject<ThirdResponseModel>(resResult);
                }
            }
            catch (Exception ex)
            {
                LoggerFactory.CreateLog().LogInfo("推送中间表数据出现未处理异常" + ex.Message, ex);
            }
            return null;
        }

        public static string GetRequestSign(string url, string method, string requestBody, string appSecret,
            bool containsAbsoluteUri = true, StringBuilder log = null)
        {
            List<string> combined = new List<string>();

            // request method
            combined.Add(method.ToUpper());

            Uri uri = new Uri(url);
            if (containsAbsoluteUri)
            {
                // scheme
                combined.Add(uri.Scheme.ToLower());
                // host
                combined.Add(uri.Host.ToLower());
                // port
                combined.Add(uri.Port.ToString());
            }
            // path    
            string path = uri.AbsolutePath.ToLower();
            path = path.Replace("\\", "/");
            if (path.EndsWith("/"))
                path = path.Substring(0, path.Length - 1);
            combined.Add(Encode(path));
            // query string
            string q = (uri.Query ?? "").Trim();
            if (q.Length > 0)
            {
                if (q.StartsWith("?"))
                    q = q.Substring(1);
                string[] itemStrs = q.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
                List<KeyValuePair<string, string>> items = new List<KeyValuePair<string, string>>();
                foreach (string itemStr in itemStrs)
                {
                    if (itemStr.Trim().Length == 0) continue;
                    string key = "", value = "";
                    int index = itemStr.IndexOf("=");
                    if (index <= 0) // = is missing or key is missing, ignore
                    {
                        continue;
                    }
                    else
                    {
                        key = HttpUtility.UrlDecode(itemStr.Substring(0, index)).Trim().ToLower();
                        value = HttpUtility.UrlDecode(itemStr.Substring(index + 1)).Trim();
                        items.Add(new KeyValuePair<string, string>(key, value));
                    }
                }
                // query
                combined.Add(String.Join("&",
                    items.OrderBy(t => t.Key).Select(t => String.Format("{0}={1}", Encode(t.Key), Encode(t.Value))).ToArray()));
            }
            else
                combined.Add("");
            // body
            combined.Add(Encode(requestBody ?? ""));
            // salt
            combined.Add(appSecret);
            string baseString = String.Join("|", combined.ToArray());
            if (log != null)
                log.AppendLine("Base String: " + baseString);
            System.Security.Cryptography.SHA256Managed s256 = new System.Security.Cryptography.SHA256Managed();
            byte[] buff;
            buff = s256.ComputeHash(Encoding.UTF8.GetBytes(baseString));
            s256.Clear();
            return Convert.ToBase64String(buff);
        }

        private static string Encode(string s)
        {
            string t = HttpUtility.UrlEncode(s);
            t = t.Replace("+", "%20");
            t = t.Replace("!", "%21");
            t = t.Replace("(", "%28");
            t = t.Replace(")", "%29");
            t = t.Replace("*", "%2a");
            return t;
        }
        private static string Decode(string s)
        {
            return HttpUtility.UrlDecode(s);
        }

        #endregion

    }
}
