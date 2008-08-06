using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.IoC.Factories
{
    /// <summary>
    /// A class that ensures that all object references
    /// created from a singleton are truly unique.
    /// </summary>
    internal static class SingletonCache
    {
        private static readonly Dictionary<Type, object> _singletons = new Dictionary<Type, object>();

        /// <summary>
        /// Creates a unique service instance using the given <paramref name="container"/> 
        /// and the <paramref name="createInstance"/> factory delegate.
        /// </summary>
        /// <typeparam name="T">The type of service to create.</typeparam>
        /// <param name="container">The <see cref="IContainer"/> instance that will ultimately instantiate the service.</param>
        /// <param name="createInstance">The factory delegate that will be used in creating the unique service instance.</param>
        /// <returns>A non-null reference pointing to the unique service instance.</returns>
        public static T CreateInstance<T>(IContainer container, 
            Func<IContainer, T> createInstance)
        {
            lock (_singletons)
            {
                // Add a new instance, if necessary
                if (!_singletons.ContainsKey(typeof(T)))
                    _singletons[typeof(T)] = createInstance(container);
            }

            return (T)_singletons[typeof(T)];
        }
    }
}
