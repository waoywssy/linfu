using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC
{
    /// <summary>
    /// Represents a basic inversion of control container that supports
    /// named services.
    /// </summary>
    /// <seealso name="IContainer/>
    /// <seealso name="INamedContainer"/>
    public abstract class ServiceContainerBase : BaseContainer, IServiceContainer
    {
        private readonly Dictionary<string, Dictionary<Type, IFactory>> _namedFactories =
            new Dictionary<string, Dictionary<Type, IFactory>>();

        private readonly List<IPostProcessor> _postProcessors = new List<IPostProcessor>();
        /// <summary>
        /// Adds an <see cref="IFactory"/> instance and associates it
        /// with the given <paramref name="serviceType">service type</paramref> and
        /// <paramref name="serviceName">service name</paramref>.
        /// </summary>
        /// <param name="serviceName">The name of the service to associate with the given <see cref="IFactory"/> instance.</param>
        /// <param name="serviceType">The type of service that the factory will be able to create.</param>
        /// <param name="factory">The <see cref="IFactory"/> instance that will create the object instance.</param>
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

        /// <summary>
        /// Determines whether or not a service can be created using
        /// the given <paramref name="serviceName">service name</paramref>
        /// and <paramref name="serviceType">service type</paramref>.
        /// </summary>
        /// <param name="serviceName">The name of the service to associate with the given <see cref="IFactory"/> instance.</param>
        /// <param name="serviceType">The type of service that the factory will be able to create.</param>
        /// <returns>Returns <c>true</c> if the service exists; otherwise, it will return <c>false</c>.</returns>
        public virtual bool Contains(string serviceName, Type serviceType)
        {
            // Use the standard IContainer.Contains(Type)
            // if the service name is blank
            if (serviceName == string.Empty)
                return Contains(serviceType);

            return _namedFactories.ContainsKey(serviceName) && 
                   _namedFactories[serviceName].ContainsKey(serviceType);
        }

        /// <summary>
        /// Causes the container to instantiate the service with the given
        /// <paramref name="serviceType">service type</paramref>. If the service type cannot be created, then an
        /// exception will be thrown if the <see cref="IContainer.SuppressErrors"/> property
        /// is set to false. Otherwise, it will simply return null.
        /// </summary>
        /// <param name="serviceName">The name of the service to instantiate.</param>
        /// <param name="serviceType">The service type to instantiate.</param>        
        /// <returns>If successful, it will return a service instance that is compatible with the given type;
        /// otherwise, it will just return a <c>null</c> value.</returns>
        public virtual object GetService(string serviceName, Type serviceType)
        {
            // Used the other GetService method if
            // the name is blank
            if (serviceName == string.Empty)
                return GetService(serviceType);

            // Determine if the service exists, and
            // suppress the errors if necessary
            var exists = Contains(serviceName, serviceType);
            if (!exists && SuppressErrors)
                return null;

            if (!exists && SuppressErrors != true)
                throw new NamedServiceNotFoundException(serviceName, serviceType);

            var factory = _namedFactories[serviceName][serviceType];

            object result = null;
            // Make sure that the factory exists
            if (factory != null)
                result = factory.CreateInstance(this);

            return result;
        }
        /// <summary>
        /// The list of postprocessors that will handle every
        /// service request result.
        /// </summary>
        public IList<IPostProcessor> PostProcessors
        {
            get { return _postProcessors; }
        }
    }
}
