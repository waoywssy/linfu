﻿using System;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Factories
{
    /// <summary>
    /// A factory that creates a unique service instance every time
    /// the <see cref="CreateInstance"/> method is called.
    /// </summary>
    /// <typeparam name="T">The type of service to instantiate.</typeparam>
    public class OncePerRequestFactory<T> : BaseFactory<T>
    {
        private readonly Func<Type, IContainer, object[], T> _createInstance;
        private readonly Type _serviceType;
        /// <summary>
        /// Initializes the factory class using the <paramref name="createInstance"/>
        /// parameter as a factory delegate.
        /// </summary>
        /// <example>
        /// The following is an example of initializing a <c>OncePerRequestFactory&lt;T&gt;</c>
        /// type:
        /// <code>
        ///     // Define the factory delegate
        ///     Func&lt;IContainer, ISomeService&gt; createService = container=>new SomeServiceImplementation();
        /// 
        ///     // Create the factory
        ///     var factory = new OncePerRequestFactory&lt;ISomeService&gt;(createService);
        /// 
        ///     // Use the service instance
        ///     var service = factory.CreateInstance(null);
        ///     
        ///     // ...
        /// </code>
        /// </example>
        /// <param name="createInstance">The delegate that will be used to create each new service instance.</param>
        public OncePerRequestFactory(Func<Type, IContainer, object[], T> createInstance)
        {
            _createInstance = createInstance;
            _serviceType = typeof(T);
        }

        /// <summary>
        /// This method creates a new service instance every time
        /// it is invoked. 
        /// </summary>
        /// <param name="container">The <see cref="IContainer"/> instance that will ultimately instantiate the service.</param>
        /// <param name="additionalArguments">The list of arguments to use with the current factory instance.</param>
        /// <returns>A non-null object reference.</returns>
        public override T CreateInstance(IContainer container, params object[] additionalArguments)
        {
            return _createInstance(_serviceType, container, additionalArguments);
        }
    }
}