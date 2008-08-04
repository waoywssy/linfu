using System;

namespace LinFu.IoC
{
    public interface IContainer 
    {
        bool SuppressErrors { get; set; }
        void AddFactory(Type serviceType, IFactory factory);
        bool Contains(Type serviceType);
        object GetService(Type serviceType);
    }
}