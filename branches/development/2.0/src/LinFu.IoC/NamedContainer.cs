using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC;

namespace LinFu.IoC
{
    public class NamedContainer : SimpleContainer, INamedContainer
    {
        protected readonly Dictionary<string, Dictionary<Type, IFactory>> _namedFactories =
            new Dictionary<string, Dictionary<Type, IFactory>>();

        public virtual void AddFactory(string serviceName, Type serviceType, IFactory factory)
        {
            if (serviceName == string.Empty)
            {
                AddFactory(serviceType, factory);
                return;
            }
            // Create the entry, if necessary
            if (!_namedFactories.ContainsKey(serviceName))
                _namedFactories[serviceName] = new Dictionary<Type, IFactory>();

            _namedFactories[serviceName][serviceType] = factory;            
        }

        public virtual bool Contains(string serviceName, Type serviceType)
        {
            // Use the standard IContainer.Contains(Type)
            // if the service name is blank
            if (serviceName == string.Empty)
                return Contains(serviceType);

            return _namedFactories.ContainsKey(serviceName) && 
                   _namedFactories[serviceName].ContainsKey(serviceType);
        }

        public virtual object GetService(string serviceName, Type serviceType)
        {
            // Used the other GetService method if
            // the name is blank
            if (serviceName == string.Empty)
                return GetService(serviceType);

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
