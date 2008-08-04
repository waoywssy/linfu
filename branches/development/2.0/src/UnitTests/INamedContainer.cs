using System;

namespace LinFu.IoC
{
    public interface INamedContainer
    {
        void AddFactory(string serviceName, Type serviceType, IFactory factory);
        bool Contains(string serviceName, Type serviceType);
        object GetService(string serviceName, Type serviceType);
    }
}