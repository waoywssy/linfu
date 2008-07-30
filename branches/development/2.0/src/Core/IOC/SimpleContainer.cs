using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.IOC
{
    public class SimpleContainer : IContainer
    {
        private readonly Dictionary<Type, IFactory> _factories = new Dictionary<Type, IFactory>();
        private readonly Dictionary<string, Dictionary<Type, IFactory>> _namedFactories =
            new Dictionary<string, Dictionary<Type, IFactory>>();

        public bool SuppressErrors
        {
            get; set;
        }

        public void AddFactory(Type serviceType, IFactory factory)
        {
            _factories[serviceType] = factory;
        }

        public bool Contains(Type serviceType)
        {
            return _factories.ContainsKey(serviceType);
        }

        public object GetService(Type serviceType)
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

        public void AddFactory(string serviceName, Type serviceType, IFactory factory)
        {
            // Create the entry, if necessary
            if (!_namedFactories.ContainsKey(serviceName))
                _namedFactories[serviceName] = new Dictionary<Type, IFactory>();

            _namedFactories[serviceName][serviceType] = factory;            
        }

        public bool Contains(string serviceName, Type serviceType)
        {   
            return _namedFactories.ContainsKey(serviceName) && 
                _namedFactories[serviceName].ContainsKey(serviceType);
        }

        public object GetService(string serviceName, Type serviceType)
        {
            // Determine if the service exists, and
            // suppress the errors if necessary
            bool exists = Contains(serviceName, serviceType);
            if (!exists && SuppressErrors)
                return null;

            if (!exists && SuppressErrors != true)
                throw new NamedServiceNotFoundException(serviceName, serviceType);

            var factory = _namedFactories[serviceName][serviceType];

            // Make sure that the factory exists
            if (factory == null)
                return null;

            return factory.CreateInstance(this);
        }
    }
}
