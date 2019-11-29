using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HISInterfaceService.Core.Enum
{
    public enum ReportStatus
    {
        Save = 10,
        Submit = 20,
        Verify = 30,
        Refuse = 40,
        Print = 50,
        Publish = 60,

        /// <summary>
        /// 未写报告
        /// </summary>
        UnSave = 70,

        /// <summary>
        /// 已检查未关联
        /// </summary>
        UnMatch = 80,

        /// <summary>
        /// 审核中
        /// </summary>
        Verifing = 90
    }
}
