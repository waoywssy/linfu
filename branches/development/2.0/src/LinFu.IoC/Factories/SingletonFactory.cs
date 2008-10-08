﻿using System;
using System.Collections.Generic;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Factories
{
    /// <summary>
    /// A factory that creates Singletons. Each service that this factory creates will only be created once per concrete type.
    /// </summary>
    /// <typeparam name="T">The type of service to instantiate.</typeparam>
    public class SingletonFactory<T> : BaseFactory<T>
    {
        private static readonly Dictionary<Type, T> _concreteInstances = new Dictionary<Type, T>();
        private readonly Func<Type, IContainer, object[], T> _createInstance;
        private T _instance;
        private Type _concreteType;
        private readonly object _lock = new object();
        /// <summary>
        /// Initializes the factory class using the <paramref name="createInstance"/>
        /// parameter as a factory delegate.
        /// </summary>
        /// <example>
        /// The following is an example of initializing a <c>SingletonFactory&lt;T&gt;</c>
        /// type:
        /// <code>
        ///     // Define the factory delegate
        ///     Func&lt;IContainer, ISomeService&gt; createService = container=>new SomeServiceImplementation();
        /// 
        ///     // Create the factory
        ///     var factory = new SingletonFactory&lt;ISomeService&gt;(createService);
        /// 
        ///     // Use the service instance
        ///     var service = factory.CreateInstance(null);
        ///     
        ///     // ...
        /// </code>
        /// </example>
        /// <param name="createInstance">The delegate that will be used to create each new service instance.</param>
        public SingletonFactory(Func<Type, IContainer, object[], T> createInstance)
        {
            _createInstance = createInstance;
        }

        /// <summary>
        /// A method that creates a service instance as a singleton.
        /// </summary>
        /// <param name="container">The <see cref="IContainer"/> instance that will ultimately instantiate the service.</param>
        /// <param name="additionalArguments">The list of arguments to use with the current factory instance.</param>
        /// <returns>A service instance as a singleton.</returns>
        public override T CreateInstance(IContainer container, params object[] additionalArguments)
        {
            if (!ReferenceEquals(_instance, null))
                return _instance;
            
            lock (_lock)
            {
                T result = _createInstance(typeof(T), container, additionalArguments);
                if (result != null)
                {
                    _concreteType = result.GetType();
                    _concreteInstances[_concreteType] = result;
                    _instance = _concreteInstances[_concreteType];
                }
            }

            return _instance;
        }
    }
}
