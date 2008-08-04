using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC;

namespace LinFu.IoC
{
    public class SimpleContainer : IContainer
    {
        private readonly Dictionary<Type, IFactory> _factories = new Dictionary<Type, IFactory>();        

        public virtual bool SuppressErrors
        {
            get; set;
        }

        public virtual void AddFactory(Type serviceType, IFactory factory)
        {
            _factories[serviceType] = factory;
        }

        public virtual bool Contains(Type serviceType)
        {
            return _factories.ContainsKey(serviceType);
        }

        public virtual object GetService(Type serviceType)
        {
            object result = null;
            if (!_factories.ContainsKey(serviceType) && !SuppressErrors)
                throw new ServiceNotFoundException(serviceType);

            if (!_factories.ContainsKey(serviceType) && SuppressErrors)
                return null;
            
            // Use the corresponding factory 
            // and create the service instance
            var factory = _factories[serviceType];
            if (factory != null)
                result = factory.CreateInstance(this);

            return result;
        }
    }
}
