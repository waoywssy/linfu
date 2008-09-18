using System;
using System.Collections.Generic;
using System.Linq;
using LinFu.IoC.Configuration;
using LinFu.IoC.Extensions;
using LinFu.IoC.Factories;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC
{
    /// <summary>
    /// A class that adds generics support to existing 
    /// <see cref="IContainer"/> and <see cref="IServiceContainer"/>
    /// instances.
    /// </summary>
    public static class ContainerExtensions
    {
        /// <summary>
        /// Creates an instance of <typeparamref name="T"/>
        /// using the given <paramref name="container"/>.
        /// </summary>
        /// <typeparam name="T">The service type to create.</typeparam>
        /// <param name="container">The container that will instantiate the service.</param>
        /// <returns>If successful, it will return a service instance that is compatible with the given type;
        /// otherwise, it will just return a <c>null</c> value.</returns>
        public static T GetService<T>(this IContainer container)
            where T : class
        {
            Type serviceType = typeof(T);
            return container.GetService(serviceType) as T;
        }

        /// <summary>
        /// Instantiates a service that matches the <paramref name="info">service description</paramref>.
        /// </summary>
        /// <param name="container">The container that will instantiate the service.</param>
        /// <param name="info">The description of the requested service.</param>
        /// <returns>If successful, it will return a service instance that is compatible with the given type;
        /// otherwise, it will just return a <c>null</c> value.</returns>
        public static object GetService(this IServiceContainer container, IServiceInfo info)
        {
            return container.GetService(info.ServiceName, info.ServiceType);
        }

        /// <summary>
        /// Creates an instance of <typeparamref name="T"/>
        /// using the given <paramref name="container"/>.
        /// </summary>
        /// <typeparam name="T">The service type to create.</typeparam>
        /// <param name="container">The container that will instantiate the service.</param>
        /// <param name="serviceName">The name of the service to instantiate.</param>
        /// <returns>If successful, it will return a service instance that is compatible with the given type;
        /// otherwise, it will just return a <c>null</c> value.</returns>
        public static T GetService<T>(this IServiceContainer container, string serviceName)
            where T : class
        {
            return container.GetService(serviceName, typeof(T)) as T;
        }

        /// <summary>
        /// Configures the container to instantiate the <paramref name="implementingType"/>
        /// on every request for the <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="container">The container that will hold the service type.</param>
        /// <param name="serviceType">The type of service being implemented.</param>
        /// <param name="implementingType">The concrete type that will implement the service type.</param>
        public static void AddService(this IServiceContainer container, Type serviceType, Type implementingType)
        {
            container.AddService(string.Empty, serviceType, implementingType);
        }
        /// <summary>
        /// Configures the container to instantiate the <paramref name="implementingType"/>
        /// on every request for the <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceName">The name of the service to associate with the given <paramref name="serviceType"/>.</param>
        /// <param name="container">The container that will hold the service type.</param>
        /// <param name="serviceType">The type of service being implemented.</param>
        /// <param name="implementingType">The concrete type that will implement the service type.</param>
        public static void AddService(this IServiceContainer container, string serviceName, Type serviceType, Type implementingType)
        {           
            Func<Type, IContainer, object> factoryMethod = null;

            // Use the standard factory method for non-generic and closed generic types
            if (!serviceType.ContainsGenericParameters)
            {
                if (!serviceType.IsAssignableFrom(implementingType))
                {
                    var message = string.Format("The implementing type '{0}' must be derived from '{1}'",
                                                implementingType.AssemblyQualifiedName, serviceType.AssemblyQualifiedName);
                    throw new ArgumentException(message);
                }

                factoryMethod = (type, currentContainer) =>
                {
                    var serviceContainer = (IServiceContainer)currentContainer;
                    return serviceContainer.AutoCreate(implementingType);
                };
                container.AddFactory(serviceName, serviceType, new OncePerRequestFactory<object>(factoryMethod));
                return;
            }

            // TODO: Determine if the implementing type's type definition directly derives from
            // the service type and throw an exception if the open generic implementation type 
            // does not derive from the service type
            factoryMethod = (type, currentContainer) =>
            {
                // Extract the generic parameterTypes
                var typeArguments = type.GetGenericArguments();

                // Determine the concrete type to instantiate
                var concreteType = implementingType.MakeGenericType(typeArguments);

                var serviceContainer = (IServiceContainer)currentContainer;
                return serviceContainer.AutoCreate(concreteType);
            };

            IFactory factoryInstance = new FunctorFactory(factoryMethod);
            container.AddFactory(serviceName, serviceType, factoryInstance);
        }

        /// <summary>
        /// Adds an <see cref="IFactory"/> instance and associates it
        /// with the given <typeparamref name="T">service type</typeparamref> and
        /// <paramref name="serviceName">service name</paramref>.
        /// </summary>
        /// <param name="serviceName">The name of the service to associate with the given <see cref="IFactory"/> instance.</param>
        /// <param name="container">The container that will hold the factory instance.</param>
        /// <param name="factory">The <see cref="IFactory{T}"/> instance that will create the object instance.</param>
        public static void AddFactory<T>(this IServiceContainer container, string serviceName, IFactory<T> factory)
        {
            IFactory adapter = new FactoryAdapter<T>(factory);
            container.AddFactory(serviceName, typeof(T), adapter);
        }

        /// <summary>
        /// Adds an <see cref="IFactory"/> instance and associates it
        /// with the given <typeparamref name="T">service type</typeparamref>.
        /// </summary>        
        /// <param name="container">The container that will hold the factory instance.</param>
        /// <param name="factory">The <see cref="IFactory{T}"/> instance that will create the object instance.</param>
        public static void AddFactory<T>(this IContainer container, IFactory<T> factory)
        {
            IFactory adapter = new FactoryAdapter<T>(factory);
            container.AddFactory(typeof(T), adapter);
        }

        /// <summary>
        /// Adds an existing service instance to the container.
        /// </summary>
        /// <typeparam name="T">The type of service being added.</typeparam>
        /// <param name="container">The container that will hold the service instance.</param>
        /// <param name="instance">The service instance itself.</param>
        public static void AddService<T>(this IContainer container, T instance)
        {
            container.AddFactory(typeof(T), new InstanceFactory(instance));
        }

        /// <summary>
        /// Adds an existing service instance to the container and
        /// associates it with the <paramref name="serviceName"/>.
        /// </summary>
        /// <typeparam name="T">The type of service being added.</typeparam>
        /// <param name="container">The container that will hold the service instance.</param>
        /// <param name="serviceName">The name that will be associated with the service instance.</param>
        /// <param name="instance">The service instance itself.</param>
        public static void AddService<T>(this IServiceContainer container, string serviceName, T instance)
        {
            container.AddFactory(serviceName, typeof(T), new InstanceFactory(instance));
        }

        /// <summary>
        /// Loads a set of <paramref name="searchPattern">files</paramref> from the <paramref name="directory">target directory</paramref>.
        /// </summary>
        /// <param name="container">The container to be loaded.</param>
        /// <param name="directory">The target directory.</param>
        /// <param name="searchPattern">The search pattern that describes the list of files to be loaded.</param>
        public static void LoadFrom(this IServiceContainer container, string directory,
            string searchPattern)
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var loader = new Loader();

            // Load the LinFu assembly by default
            loader.LoadDirectory(baseDirectory, "LinFu*.dll");

            // Load the target directory
            loader.LoadDirectory(directory, searchPattern);

            // Configure the container
            loader.LoadInto(container);
        }

        /// <summary>
        /// Returns all the services in the container that match the given
        /// <typeparamref name="T">service type</typeparamref>.
        /// </summary>
        /// <typeparam name="T">The type of service to return.</typeparam>
        /// <param name="container">The target container.</param>
        /// <returns>The list of services that implement the given service type.</returns>
        public static IEnumerable<T> GetServices<T>(this IServiceContainer container)
        {
            foreach (var info in container.AvailableServices)
            {
                yield return (T)container.GetService(info.ServiceName, info.ServiceType);
            }
        }

        /// <summary>
        /// Returns a list of services that match the given <paramref name="condition"/>.
        /// </summary>
        /// <param name="condition">The predicate that determines which services should be returned.</param>
        /// <returns>A list of <see cref="IServiceInstance"/> objects that describe the services returned as well as provide a reference to the resulting services themselves.</returns>
        /// <param name="container">the target <see cref="IServiceContainer"/> instance.</param>
        public static IEnumerable<IServiceInstance> GetServices(this IServiceContainer container, Func<IServiceInfo, bool> condition)
        {
            // Create the services that match
            // the given description
            var results = from info in container.AvailableServices
                          where condition(info) && !info.ServiceType.IsGenericTypeDefinition
                          select
                              new ServiceInstance()
                                  {
                                      ServiceInfo = info,
                                      Object = container.GetService(info.ServiceName, info.ServiceType)
                                  } as IServiceInstance;

            return results;
        }
        /// <summary>
        /// Determines whether or not a container contains services that match
        /// the given <paramref name="condition"/>.
        /// </summary>
        /// <param name="container">The target container.</param>
        /// <param name="condition">The predicate that will be used to determine whether or not the requested services exist.</param>
        /// <returns>Returns <c>true</c> if the requested services exist; otherwise, it will return <c>false</c>.</returns>
        public static bool Contains(this IServiceContainer container,
            Func<IServiceInfo, bool> condition)
        {
            var matches = (from info in container.AvailableServices
                           where condition(info)
                           select info).Count();

            return matches > 0;
        }
    }
}