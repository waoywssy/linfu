using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LinFu.IoC.Factories
{
    /// <summary>
    /// A factory that creates service instances that are unique
    /// from within the same thread as the factory itself.
    /// </summary>
    /// <typeparam name="T">The type of service to instantiate.</typeparam>
    public class OncePerThreadFactory<T> : BaseFactory<T>
    {
        private readonly Func<Type, IContainer, T> _createInstance;
        private static Dictionary<int, T> _storage = new Dictionary<int, T>();

        /// <summary>
        /// Initializes the factory class using the <paramref name="createInstance"/>
        /// parameter as a factory delegate.
        /// </summary>
        /// <example>
        /// The following is an example of initializing a <c>OncePerThreadFactory&lt;T&gt;</c>
        /// type:
        /// <code>
        ///     // Define the factory delegate
        ///     Func&lt;IContainer, ISomeService&gt; createService = container=>new SomeServiceImplementation();
        /// 
        ///     // Create the factory
        ///     var factory = new OncePerThreadFactory&lt;ISomeService&gt;(createService);
        /// 
        ///     // Use the service instance
        ///     var service = factory.CreateInstance(null);
        ///     
        ///     // ...
        /// </code>
        /// </example>
        /// <param name="createInstance">The delegate that will be used to create each new service instance.</param>
        public OncePerThreadFactory(Func<Type, IContainer, T> createInstance)
        {
            _createInstance = createInstance;
        }
        /// <summary>
        /// Creates the service instance using the given <paramref name="container"/>
        /// instance. Every service instance created from this factory will
        /// only be created once per thread.
        /// </summary>
        /// <param name="container">The <see cref="IContainer"/> instance that will ultimately instantiate the service.</param>
        /// <returns>A a service instance as thread-wide singleton.</returns>
        public override T CreateInstance(IContainer container)
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;

            T result = default(T);
            lock (_storage)
            {
                // Create the service instance only once
                if (!_storage.ContainsKey(threadId))
                    _storage[threadId] = _createInstance(typeof(T), container);

                result = _storage[threadId];
            }

            return result;
        }
    }
}
