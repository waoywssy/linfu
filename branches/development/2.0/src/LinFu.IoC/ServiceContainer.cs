using System;
using System.Collections.Generic;
using System.Linq;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC
{
    /// <summary>
    /// Represents a service container with additional
    /// extension points for customizing service instances
    /// </summary>
    public class ServiceContainer : ServiceContainerBase
    {
        private readonly Dictionary<Type, IFactory> _genericFactories = new Dictionary<Type, IFactory>();

        private readonly Dictionary<string, Dictionary<Type, IFactory>> _namedFactories =
            new Dictionary<string, Dictionary<Type, IFactory>>();

        /// <summary>
        /// Overridden. This method modifies the original
        /// <see cref="BaseContainer.GetService"/> method
        /// so that its results can be handled by the 
        /// postprocessors.
        /// </summary>
        /// <param name="serviceName">The name of the service to instantiate.</param>
        /// <param name="serviceType">The service type to instantiate.</param>        
        /// <returns>If successful, it will return a service instance that is compatible with the given type;
        /// otherwise, it will just return a <c>null</c> value.</returns>
        public override object GetService(string serviceName, Type serviceType)
        {
            // Attempt to create the service
            // without throwing an exception
            object instance = null;

            // Save the old state
            bool suppressErrors = SuppressErrors;
            lock (this)
            {
                SuppressErrors = true;
                instance = base.GetService(serviceName, serviceType);
                SuppressErrors = suppressErrors;
            }

            // Attempt to create the service type using
            // the generic factories, if possible
            if (instance == null && serviceType.IsGenericType && 
                !serviceType.IsGenericTypeDefinition)
            {
                // Check if any factories can create the service
                // with the given definition type
                var definitionType = serviceType.GetGenericTypeDefinition();
                if (_namedFactories.ContainsKey(serviceName) && 
                    _namedFactories[serviceName].ContainsKey(definitionType))
                {
                    var factory = _namedFactories[serviceName][definitionType];

                    // Generate the service instance
                    if (factory != null)
                        instance = factory.CreateInstance(serviceType, this);
                }
            }

            IServiceRequestResult result = PostProcess(serviceName, serviceType, instance);

            // Use the modified result, if possible; otherwise,
            // use the original result.
            instance = result.ActualResult ?? result.OriginalResult;

            if (suppressErrors == false && instance == null)
                throw new NamedServiceNotFoundException(serviceName, serviceType);

            return instance;
        }

        /// <summary>
        /// Overridden. Causes the container to instantiate the service with the given
        /// <paramref name="serviceType">service type</paramref>. If the service type cannot be created, then an
        /// exception will be thrown if the <see cref="IContainer.SuppressErrors"/> property
        /// is set to false. Otherwise, it will simply return null.
        /// </summary>
        /// <remarks>
        /// This overload of the <c>GetService</c> method has been overridden
        /// so that its results can be handled by the postprocessors.
        /// </remarks>
        /// <seealso cref="IPostProcessor"/>
        /// <param name="serviceType">The service type to instantiate.</param>
        /// <returns>If successful, it will return a service instance that is compatible with the given type;
        /// otherwise, it will just return a null value.</returns>
        public override object GetService(Type serviceType)
        {
            object instance = null;

            // Save the old state
            bool suppressErrors = SuppressErrors;

            // Attempt to create the service
            // without throwing an exception
            lock (this)
            {
                SuppressErrors = true;
                instance = base.GetService(serviceType);
                SuppressErrors = suppressErrors;
            }

            // Attempt to create the service type using
            // the generic factories, if possible
            if (instance == null && serviceType.IsGenericType &&
                !serviceType.IsGenericTypeDefinition)
            {
                // Check if any factories can create the service
                // with the given definition type
                var definitionType = serviceType.GetGenericTypeDefinition();
                if (_genericFactories.ContainsKey(definitionType))
                {
                    var factory = _genericFactories[definitionType];

                    // Generate the service instance
                    if (factory != null)
                        instance = factory.CreateInstance(serviceType, this);
                }
            }

            IServiceRequestResult result = PostProcess(string.Empty, serviceType, instance);

            // Use the modified result, if possible; otherwise,
            // use the original result.
            instance = result.ActualResult ?? result.OriginalResult;


            if (suppressErrors == false && instance == null)
                throw new ServiceNotFoundException(serviceType);

            return instance;
        }

        /// <summary>
        /// Overrides the <see cref="ServiceContainerBase.AddFactory(string,Type,IFactory)"/> method to support
        /// factories that create services based on open generic types.
        /// </summary>
        /// <param name="serviceName">The name of the service to associate with the given <see cref="IFactory"/> instance.</param>
        /// <param name="serviceType">The type of service that the factory will be able to create.</param>
        /// <param name="factory">The <see cref="IFactory"/> instance that will create the object instance.</param>
        public override void AddFactory(string serviceName, Type serviceType, IFactory factory)
        {
            // If the service type is not a generic type definition (such as IList<>)
            // let the base class handle the factory instance
            if (!serviceType.IsGenericTypeDefinition || !serviceType.ContainsGenericParameters)
            {
                base.AddFactory(serviceName, serviceType, factory);
                return;
            }

            // Create a new dictionary entry, if necessary
            if (!_namedFactories.ContainsKey(serviceName))
                _namedFactories[serviceName] = new Dictionary<Type, IFactory>();

            _namedFactories[serviceName][serviceType] = factory;
        }
        
        /// <summary>
        /// Overrides the <see cref="ServiceContainerBase.Contains(string,Type)"/> method to allow
        /// users to determine whether or not a specific generic service type can be created
        /// using the open generic factories that currently reside in the container itself.
        /// </summary>
        /// <param name="serviceName">The name of the service to associate with the given <see cref="IFactory"/> instance.</param>
        /// <param name="serviceType">The type of service that the factory will be able to create.</param>
        /// <returns>Returns <c>true</c> if the service exists; otherwise, it will return <c>false</c>.</returns>
        public override bool Contains(string serviceName, Type serviceType)
        {
            // Use the default implementation for
            // non-generic types
            if (!serviceType.IsGenericType && !serviceType.IsGenericTypeDefinition)
                return base.Contains(serviceName, serviceType);

            // If the service type is a generic type, determine
            // if the service type can be created by a 
            // standard factory that can create an instance
            // of that generic type (e.g., IFactory<IGeneric<T>>            
            var result = base.Contains(serviceName, serviceType);

            // Immediately return a positive match, if possible
            if (result)
                return true;

            if (serviceType.IsGenericType && !serviceType.IsGenericTypeDefinition)
            {
                // Determine the base type definition
                var baseDefinition = serviceType.GetGenericTypeDefinition();

                // Check if there are any generic factories that can create
                // the entire family of services whose type definitions
                // match the base type
                result = _namedFactories.ContainsKey(serviceName) &&
                         _namedFactories[serviceName].ContainsKey(baseDefinition);
            }

            return result;
        }

        /// <summary>
        /// Overrides the <see cref="ServiceContainerBase.AddFactory(string,Type,IFactory)"/> method to support
        /// factories that create services based on open generic types.
        /// </summary>
        /// <param name="serviceType">The type of service that the factory will be able to create.</param>
        /// <param name="factory">The <see cref="IFactory"/> instance that will create the object instance.</param>
        public override void AddFactory(Type serviceType, IFactory factory)
        {
            // If the service type is not a generic type definition (such as IList<>)
            // let the base class handle the factory instance
            if (!serviceType.IsGenericTypeDefinition || !serviceType.ContainsGenericParameters)
            {
                base.AddFactory(serviceType, factory);
                return;
            }

            _genericFactories.Add(serviceType, factory);
        }

        /// <summary>
        /// Overrides the <see cref="ServiceContainerBase.Contains(string,Type)"/> method to allow
        /// users to determine whether or not a specific generic service type can be created
        /// using the open generic factories that currently reside in the container itself.
        /// </summary>
        /// <param name="serviceType">The type of service that the factory will be able to create.</param>
        /// <returns>Returns <c>true</c> if the service exists; otherwise, it will return <c>false</c>.</returns>
        public override bool Contains(Type serviceType)
        {
            // Use the default implementation for
            // non-generic types
            if (!serviceType.IsGenericType && !serviceType.IsGenericTypeDefinition)
                return base.Contains(serviceType);

            // If the service type is a generic type, determine
            // if the service type can be created by a 
            // standard factory that can create an instance
            // of that generic type (e.g., IFactory<IGeneric<T>>            
            var result = base.Contains(serviceType);

            // Immediately return a positive match, if possible
            if (result)
                return true;

            if (serviceType.IsGenericType && !serviceType.IsGenericTypeDefinition)
            {
                // Determine the base type definition
                var baseDefinition = serviceType.GetGenericTypeDefinition();

                // Check if there are any generic factories that can create
                // the entire family of services whose type definitions
                // match the base type
                result = _genericFactories.ContainsKey(baseDefinition);
            }

            return result;
        }

        /// <summary>
        /// Lists all the services available inside the container. This includes
        /// unnamed and named service types, as well as open generic service types
        /// that can be created by the container.
        /// </summary>
        public override IEnumerable<IServiceInfo> AvailableServices
        {
            get
            {
                var result = new List<IServiceInfo>();

                // Append the unnamed services
                var unnamedServices = from type in _genericFactories.Keys
                                      let info = new ServiceInfo(string.Empty, type) as IServiceInfo
                                      select info;
                
                result.AddRange(unnamedServices);

                // Append the named services
                var namedServices = from name in _namedFactories.Keys
                                    from type in _namedFactories[name].Keys
                                    let info = new ServiceInfo(name, type) as IServiceInfo
                                    select info;

                result.AddRange(namedServices);

                // Reuse the results from the base class
                result.AddRange(base.AvailableServices);                

                return result;
            }
        }

        /// <summary>
        /// A method that searches the current container for
        /// postprocessors and passes every request result made
        /// to the list of <see cref="IServiceContainer.PostProcessors"/>.
        /// </summary>
        /// <param name="serviceName">The name of the service being requested. By default, this is usually blank.</param>
        /// <param name="serviceType">The type of service being requested.</param>
        /// <param name="instance">The original instance returned by container's service instantiation attempt.</param>
        /// <returns>A <see cref="IServiceRequestResult"/> representing the results returned as a result of the postprocessors.</returns>
        private IServiceRequestResult PostProcess(string serviceName, Type serviceType, object instance)
        {
            // Initialize the results
            var result = new ServiceRequestResult
                             {
                                 ServiceName = serviceName ?? string.Empty,
                                 ActualResult = instance,
                                 Container = this,
                                 OriginalResult = instance,
                                 ServiceType = serviceType
                             };

            // Let each postprocessor inspect 
            // the results and/or modify the 
            // returned object instance
            foreach (IPostProcessor postProcessor in PostProcessors)
            {
                if (postProcessor == null)
                    continue;

                postProcessor.PostProcess(result);
            }

            return result;
        }
    }
}