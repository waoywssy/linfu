using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.IoC
{
    /// <summary>
    /// An adapter that converts strongly-typed IFactory&lt;T&gt; 
    /// instances into an equivalent IFactory instance.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FactoryAdapter<T> : IFactory
    {
        private readonly IFactory<T> _factory;
        
        /// <summary>
        /// Creates the factory adapter using the given
        /// IFactory&lt;T&gt; instance.
        /// </summary>
        /// <param name="factory">The strongly-typed factory instance that
        /// will be called every time the <see cref="IFactory.CreateInstance"/> method
        /// is called. </param>
        public FactoryAdapter(IFactory<T> factory)
        {
            _factory = factory;
        }
        /// <summary>
        /// Overridden. Uses the strongly-typed factory
        /// to create the service instance every time
        /// the <see cref="IFactory.CreateInstance"/> method 
        /// is called.
        /// </summary>
        /// <param name="container">The <see cref="IContainer"/> instance that will ultimately instantiate the service.</param>
        /// <returns>An object instance that represents the service to be created. This cannot be <c>null</c>.</returns>
        public object CreateInstance(IContainer container)
        {
            if (_factory == null)
                return default(T);

            return _factory.CreateInstance(container);
        }
    }
}
