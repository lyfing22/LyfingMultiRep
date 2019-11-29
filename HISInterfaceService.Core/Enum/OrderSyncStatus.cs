using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HISInterfaceService.Core.Enum
{
    public enum OrderSyncStatus
    {
        /// <summary>
        /// 已经从HIS数据库同步到OrderSync中，RIS尚未回写
        /// </summary>
        OrderSynced = 0,

        /// <summary>
        /// 已经预约
        /// </summary>
        Booked = 1,

        /// <summary>
        /// 已经到检
        /// </summary>
        ArrivalChecked = 2,

        /// <summary>
        /// 已经取消申请单
        /// </summary>
        Canceled = 3,

        /// <summary>
        /// 检查确认
        /// </summary>
        CheckConfirmed = 4,

        /// <summary>
        /// 已经回写报告
        /// </summary>
        ReportSynced = 5
    }
}
