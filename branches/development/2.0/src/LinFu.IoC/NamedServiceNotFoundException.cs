using System;

namespace LinFu.IoC
{
    /// <summary>
    /// The exception thrown when a service name and a service type is
    /// requested from a named container and that named container
    /// is unable to find or create that particular service instance.
    /// </summary>
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