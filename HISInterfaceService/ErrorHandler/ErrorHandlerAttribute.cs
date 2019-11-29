using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Web;

namespace HISInterfaceService.ErrorHandler
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class ErrorHandlerAttribute : Attribute, IServiceBehavior
    {
        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            //throw new NotImplementedException();
        }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints,
            BindingParameterCollection bindingParameters)
        {
            //throw new NotImplementedException();
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            IErrorHandler errorHandler = new FaultErrorHandler();
            foreach (var channelDispatcherBase in serviceHostBase.ChannelDispatchers)
            {
                var channelDispatcher = channelDispatcherBase as ChannelDispatcher;
                channelDispatcher.ErrorHandlers.Add(errorHandler);
            }
        }
    }
}