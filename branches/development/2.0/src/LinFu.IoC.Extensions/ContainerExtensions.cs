using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC;

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
            var serviceType = typeof (T);
            return container.GetService(serviceType) as T;
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
            return container.GetService(serviceName, typeof (T)) as T;
        }
        /// <summary>
        /// Adds an <see cref="IFactory"/> instance and associates it
        /// with the given <typeparamref name="T">service type</paramref> and
        /// <paramref name="serviceName">service name</paramref>.
        /// </summary>
        /// <param name="serviceName">The name of the service to associate with the given <see cref="IFactory"/> instance.</param>
        /// <param name="container">The container that will hold the factory instance.</param>
        /// <param name="factory">The <see cref="IFactory{T}"/> instance that will create the object instance.</param>
        public static void AddFactory<T>(this IServiceContainer container, string serviceName, IFactory<T> factory)
        {
            IFactory adapter = new FactoryAdapter<T>(factory);
            container.AddFactory(serviceName, typeof (T), adapter);
        }

        /// <summary>
        /// Adds an <see cref="IFactory"/> instance and associates it
        /// with the given <typeparamref name="T">service type</paramref>.
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
            container.AddFactory(typeof (T), new InstanceFactory(instance));
        }
    }
}
