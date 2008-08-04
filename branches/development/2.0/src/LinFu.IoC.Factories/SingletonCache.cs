using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.IoC.Factories
{
    internal static class SingletonCache
    {
        private static readonly Dictionary<Type, object> _singletons = new Dictionary<Type, object>();
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
