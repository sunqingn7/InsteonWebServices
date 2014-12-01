using System.Collections.Generic;
using ServiceStack.ServiceInterface.ServiceModel;

namespace Insteon.Daemon.Common.Response
{
    public class GetDevicesResponse : IHasResponseStatus
    {		
        public IList<DeviceInfo> Devices { get; set; }		
        public ResponseStatus ResponseStatus { get; set; }
    }
}