using System;
using System.Collections.Generic;
using System.Linq;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC
{
    /// <summary>
    /// Represents a basic inversion of control container that supports
    /// named services.
    /// </summary>
    public abstract class ServiceContainerBase : BaseContainer, IServiceContainer
    {
        private readonly Dictionary<string, Dictionary<Type, IFactory>> _namedFactories =
            new Dictionary<string, Dictionary<Type, IFactory>>();

        private readonly List<IPostProcessor> _postProcessors = new List<IPostProcessor>();
        private readonly List<IPreprocessor> _preprocessors = new List<IPreprocessor>();

        /// <summary>
        /// Initializes the container with the default services.
        /// </summary>
        protected ServiceContainerBase()
        {
            this.AddDefaultServices();
        }
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
            if (serviceName == null)
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
            if (serviceName == null)
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
        /// <param name="additionalArguments">The additional arguments that will be used to instantiate the service type.</param>
        /// <returns>If successful, it will return a service instance that is compatible with the given type;
        /// otherwise, it will just return a <c>null</c> value.</returns>
        public virtual object GetService(string serviceName, Type serviceType, params object[] additionalArguments)
        {
            // Used the other GetService method if
            // the name is blank
            if (serviceName == null)
                return GetService(serviceType, additionalArguments);

            // Determine if the service exists, and
            // suppress the errors if necessary
            bool exists = Contains(serviceName, serviceType);
            if (!exists && SuppressErrors)
                return null;

            if (!exists && SuppressErrors != true)
                throw new NamedServiceNotFoundException(serviceName, serviceType);

            // Use the named factory if it exists
            IFactory factory = null;
            if (_namedFactories.ContainsKey(serviceName) &&
                _namedFactories[serviceName].ContainsKey(serviceType))
                factory = GetFactory(serviceName, serviceType, additionalArguments);

            object result = null;

            // Make sure that the factory exists
            if (factory != null)
                result = factory.CreateInstance(serviceType, this, additionalArguments);

            return result;
        }

        /// <summary>
        /// Allows subclasses to determine which factories should be used
        /// for a particular service request.
        /// </summary>
        /// <param name="serviceName">The name of the service to instantiate.</param>
        /// <param name="serviceType">The type of service being requested.</param>
        /// <param name="additionalArguments">The additional arguments that will be used to instantiate the service type.</param>
        /// <returns>A factory instance.</returns>
        protected virtual IFactory GetFactory(string serviceName, Type serviceType, object[] additionalArguments)
        {
            if (serviceName == null)
                return GetFactory(serviceType, additionalArguments);

            if (_namedFactories.ContainsKey(serviceName) && _namedFactories[serviceName].ContainsKey(serviceType))
                return _namedFactories[serviceName][serviceType];

            return null;
        }
        /// <summary>
        /// The list of postprocessors that will handle every
        /// service request result.
        /// </summary>
        public IList<IPostProcessor> PostProcessors
        {
            get { return _postProcessors; }
        }

        /// <summary>
        /// The list of preprocessors that will handle
        /// every service request before each actual service is created.
        /// </summary>
        public IList<IPreprocessor> Preprocessors
        {
            get { return _preprocessors; }
        }

        /// <summary>
        /// Lists all the services available in the container,
        /// including the named services.
        /// </summary>
        public override IEnumerable<IServiceInfo> AvailableServices
        {
            get
            {
                var results = new List<IServiceInfo>(base.AvailableServices);

                // Append the named factory entries
                var additionalResults = from name in _namedFactories.Keys
                                        let dictionary = _namedFactories[name]
                                        from type in dictionary.Keys
                                        let info = new ServiceInfo(name, type) as IServiceInfo
                                        select info;


                results.AddRange(additionalResults);
                return results;
            }
        }
    }
}