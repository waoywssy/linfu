using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Factories
{
    /// <summary>
    /// A class that converts a delegate into an <see cref="IFactory"/> instance.
    /// </summary>
    public class FunctorFactory : IFactory
    {
        private readonly Func<Type, IContainer, object[], object> _factoryMethod;

        /// <summary>
        /// Initializes the class with the given <paramref name="factoryMethod"/>.
        /// </summary>
        /// <param name="factoryMethod">The delegate that will be used to instantiate a type.</param>
        public FunctorFactory(Func<Type, IContainer, object[], object> factoryMethod)
        {
            _factoryMethod = factoryMethod;
        }

        /// <summary>
        /// Instantiates an object reference using the given factory method.
        /// </summary>
        /// <param name="serviceType">The requested service type.</param>
        /// <param name="container">The container processing the request.</param>
        /// /// <param name="additionalArguments">The list of arguments to use with the current factory instance.</param>
        /// <returns>A non-null object reference that represents the service type.</returns>
        public object CreateInstance(Type serviceType, IContainer container, params object[] additionalArguments)
        {
            return _factoryMethod(serviceType, container, additionalArguments);
        }
    }
}
