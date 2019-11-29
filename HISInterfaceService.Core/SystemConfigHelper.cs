using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Management;

namespace HISInterfaceService.Core
{
    public class SystemConfigHelper
    {
        public static string GetSystemConfig(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}
