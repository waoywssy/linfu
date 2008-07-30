using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.IOC
{
    public class ServiceNotFoundException : Exception
    {
        private readonly Type _serviceType;
        public ServiceNotFoundException(Type serviceType)
        {
            _serviceType = serviceType;
        }
        public override string Message
        {
            get
            {
                return string.Format("Service type '{0}' not found", _serviceType.AssemblyQualifiedName);
            }
        }
    }
}
