using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.IoC
{
    /// <summary>
    /// The exception thrown when a service type is
    /// requested from a container and that named container
    /// is unable to find or create that particular service instance.
    /// </summary>
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
