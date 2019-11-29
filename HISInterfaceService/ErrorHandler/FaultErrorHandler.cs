using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Web;
using HISInterfaceService.Core.Log;

namespace HISInterfaceService.ErrorHandler
{
    public class FaultErrorHandler : IErrorHandler
    {
        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            fault = null;
        }

        public bool HandleError(Exception error)
        {
            //  TO DO 在这里可以做日志记录等。
            LoggerFactory.CreateLog().LogError("error", error);
            Exception e = error;
            while (e.InnerException != null)
            {
                e = e.InnerException;
            }
            LoggerFactory.CreateLog().LogError("error", e);
            Console.WriteLine("Message:{0},StackTrace:{1}", error.Message, error.StackTrace);
            return true;
        }
    }
}