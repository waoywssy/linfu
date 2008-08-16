using System;

namespace LinFu.IoC.Factories
{
    /// <summary>
    /// A factory that creates Singletons. Each service that this factory creates will only be created once.
    /// </summary>
    /// <typeparam name="T">The type of service to instantiate.</typeparam>
    public class SingletonFactory<T> : BaseFactory<T>
    {
        private readonly Func<Type, IContainer, T> _createInstance;

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
        public SingletonFactory(Func<Type, IContainer, T> createInstance)
        {
            _createInstance = createInstance;
        }

        /// <summary>
        /// A method that creates a service instance as a singleton.
        /// </summary>
        /// <param name="container">The <see cref="IContainer"/> instance that will ultimately instantiate the service.</param>
        /// <returns>A service instance as a singleton.</returns>
        public override T CreateInstance(IContainer container)
        {
            return SingletonCache.CreateInstance(container, _createInstance);
        }
    }
}