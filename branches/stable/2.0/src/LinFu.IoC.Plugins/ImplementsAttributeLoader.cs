using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LinFu.IoC.Configuration;
using LinFu.IoC.Extensions;
using LinFu.IoC.Factories;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Plugins
{
    /// <summary>
    /// A loader class that scans a type for <see cref="ImplementsAttribute"/>
    /// attribute declarations and creates a factory for each corresponding 
    /// attribute instance.
    /// </summary>
    /// <seealso cref="IFactory"/>
    public class ImplementsAttributeLoader : ITypeLoader
    {
        private static readonly Dictionary<LifecycleType, Type> _factoryTypes =
            new Dictionary<LifecycleType, Type>();

        /// <summary>
        /// Initializes the list of factory types.
        /// </summary>
        static ImplementsAttributeLoader()
        {
            _factoryTypes[LifecycleType.OncePerRequest] = typeof(OncePerRequestFactory<>);
            _factoryTypes[LifecycleType.OncePerThread] = typeof(OncePerThreadFactory<>);
            _factoryTypes[LifecycleType.Singleton] = typeof(SingletonFactory<>);
        }

        /// <summary>
        /// Converts a given <see cref="System.Type"/> into
        /// a set of <see cref="Action{IServiceContainer}"/> instances so that
        /// the <see cref="IContainer"/> instance can be loaded
        /// with the given factories.
        /// </summary>
        /// <param name="sourceType">The input type from which one or more factories will be created.</param>
        /// <returns>A set of <see cref="Action{IServiceContainer}"/> instances. This cannot be null.</returns>
        /// 
        public IEnumerable<Action<IServiceContainer>> Load(Type sourceType)
        {
            // Extract the Implements attribute from the source type
            ICustomAttributeProvider provider = sourceType;
            object[] attributes = provider.GetCustomAttributes(typeof(ImplementsAttribute), false);
            List<ImplementsAttribute> attributeList = attributes.Cast<ImplementsAttribute>().ToList();

            var results = new List<Action<IServiceContainer>>();
            IFactory singletonFactory = null;
            foreach (ImplementsAttribute attribute in attributeList)
            {
                string serviceName = attribute.ServiceName ?? string.Empty;
                Type serviceType = attribute.ServiceType;
                LifecycleType lifeCycle = attribute.LifecycleType;

                IFactory currentFactory = CreateFactory(serviceType, sourceType, lifeCycle);
                if (currentFactory == null)
                    continue;

                // If this type is implemented as a factory singleton,
                // it only needs to be implemented once
                if (lifeCycle == LifecycleType.Singleton)
                {
                    if (singletonFactory == null)
                    {
                        // Initialize the singleton instance only once
                        singletonFactory = currentFactory;
                    }
                    else
                    {
                        // Make sure that the same singleton factory instance
                        // is assigned to every single point
                        // where it is marked as a singleton
                        currentFactory = singletonFactory;
                    }
                }

                results.Add(container =>
                            container.AddFactory(serviceName, serviceType, currentFactory));
            }

            return results;
        }

        /// <summary>
        /// Determines whether or not the current <paramref name="sourceType"/>
        /// can be loaded.
        /// </summary>
        /// <param name="sourceType">The source type currently being loaded.</param>
        /// <returns>Returns <c>true</c> if the type is a class type; otherwise, it returns <c>false</c>.</returns>
        public bool CanLoad(Type sourceType)
        {
            return sourceType.IsClass;
        }

        /// <summary>
        /// Creates a factory instance that can create instaces of the given
        /// <paramref name="serviceType"/>  using the <paramref name="implementingType"/>
        /// as the implementation.
        /// </summary>
        /// <param name="serviceType">The service being implemented.</param>
        /// <param name="implementingType">The actual type that will implement the service.</param>
        /// <param name="lifecycle">The <see cref="LifecycleType"/> that determines the lifetime of each instance being created.</param>
        /// <returns></returns>
        private IFactory CreateFactory(Type serviceType, Type implementingType, LifecycleType lifecycle)
        {
            // Determine the factory type
            Type factoryTypeDefinition = _factoryTypes[lifecycle];
            Type factoryType = factoryTypeDefinition.MakeGenericType(serviceType);

            // Create the factory itself
            MulticastDelegate factoryMethod = CreateFactoryMethod(serviceType, implementingType);
            object factoryInstance = Activator.CreateInstance(factoryType, new object[] { factoryMethod });

            var result = factoryInstance as IFactory;

            return result;
        }

        /// <summary>
        /// A <c>private</c> method that creates the factory method delegate
        /// for use with a particular factory class.
        /// </summary>
        /// <seealso cref="SingletonFactory{T}"/>
        /// <seealso cref="OncePerRequestFactory{T}"/>
        /// <seealso cref="OncePerThreadFactory{T}"/>
        /// <param name="serviceType">The service type being instantiated.</param>
        /// <param name="implementingType">The type that will provide the implementation for the actual service.</param>
        /// <returns>A factory method delegate that can create the given service.</returns>
        private MulticastDelegate CreateFactoryMethod(Type serviceType, Type implementingType)
        {
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Static;

            MethodInfo factoryMethodDefinition = typeof(ImplementsAttributeLoader).GetMethod("CreateFactoryMethodInternal", flags);
            MethodInfo factoryMethod = factoryMethodDefinition.MakeGenericMethod(serviceType, implementingType);

            // Create the Func<Type, IContainer, TService> factory delegate
            var result = factoryMethod.Invoke(null, new object[0]) as MulticastDelegate;

            return result;
        }

        /// <summary>
        /// A method that generates the actual lambda function that creates
        /// the new service instance.
        /// </summary>
        /// <typeparam name="TService">The service type being instantiated.</typeparam>
        /// <typeparam name="TImplementation">The type that will provide the implementation for the actual service.</typeparam>
        /// <returns>A strongly-typed factory method delegate that can create the given service.</returns>
        internal static Func<Type, IContainer, TService> CreateFactoryMethodInternal<TService, TImplementation>()
            where TImplementation : TService
        {
            return (type, container) =>
                       {
                           var serviceContainer = container as IServiceContainer;

                           // Attempt to autoresolve the constructor
                           if (serviceContainer != null)
                               return (TService)serviceContainer.AutoCreate(typeof(TImplementation));

                           // Otherwise, use the default constructor
                           return (TService)Activator.CreateInstance(typeof(TImplementation));
                       };
        }
    }
}
