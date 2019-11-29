using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HISInterfaceService.Core.HttpReqestHelper
{
   public  class FtpRequestHelper
    {
        /// <summary>
        /// ftp下载文件
        /// </summary>
        /// <param name="ftpPath"></param>
        /// <param name="savePath"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string FtpDownloadFile(string ftpPath, string savePath, string username, string password)
        {
            FtpWebRequest request =
                (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpPath));
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.ReadWriteTimeout =1000*60;
            request.KeepAlive = true;
            request.Credentials = new NetworkCredential(username, password);
            using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (FileStream fs = new FileStream(savePath, FileMode.Create))
                    {
                        byte[] buffer = new byte[2048];
                        int read = 0;
                        while ((read = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            fs.Write(buffer, 0, read);
                        }
                    }
                }
            }
            return savePath;
        }
    }
}
