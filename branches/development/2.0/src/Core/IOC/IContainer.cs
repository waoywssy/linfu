using System;

namespace LinFu.IOC
{
    public interface IContainer
    {
        bool SuppressErrors { get; set; }

        void AddFactory(Type serviceType, IFactory factory);
        void AddFactory(string serviceName, Type serviceType, IFactory factory);

        bool Contains(Type serviceType);
        bool Contains(string serviceName, Type serviceType);

        object GetService(Type serviceType);
        object GetService(string serviceName, Type serviceType);
    }
}