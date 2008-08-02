using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.IOC
{
    public class NamedServiceNotFoundException : ServiceNotFoundException
    {
        private readonly string _serviceName;
        private readonly Type _serviceType;
        public NamedServiceNotFoundException(string serviceName, Type serviceType) : base(serviceType)
        {
            _serviceName = serviceName;
            _serviceType = serviceType;
        }
        public override string Message
        {
            get
            {
                return string.Format("Unable to find a service named '{0}' with type '{1}'", _serviceName,
                                     _serviceType.AssemblyQualifiedName);
            }
        }
    }
}
