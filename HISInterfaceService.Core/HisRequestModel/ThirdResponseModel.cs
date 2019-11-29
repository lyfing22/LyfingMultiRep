using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HISInterfaceService.Core.HisRequestModel
{
    public class ThirdResponseModel
    {
        /// <summary>
        /// 错误消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 错误编码
        /// </summary>
        /// <see cref="ThirdPartErrorCode"/>
        public string ErrorCode { get; set; }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }

    }
}
