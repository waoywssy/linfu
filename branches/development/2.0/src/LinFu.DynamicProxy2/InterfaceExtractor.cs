using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.DynamicProxy2.Interfaces;
using LinFu.IoC.Configuration;

namespace LinFu.DynamicProxy2
{
    /// <summary>
    /// Provides the default implementation for the 
    /// <see cref="IExtractInterfaces"/> interface.
    /// </summary>
    [Implements(typeof(IExtractInterfaces), LifecycleType.OncePerRequest)]
    public class InterfaceExtractor : IExtractInterfaces
    {
        /// <summary>
        /// Determines which interfaces a given type should implement.
        /// </summary>
        /// <param name="currentType">The base type that holds the list of interfaces to implement.</param>
        /// <param name="interfaces">The list of interfaces already being implemented. </param>
        public void GetInterfaces(Type currentType, HashSet<Type> interfaces)
        {
            var currentInterfaces = from t in currentType.GetInterfaces()
                                    where !interfaces.Contains(t) && t.IsInterface
                                    select t;

            foreach (var type in currentInterfaces)
            {
                GetInterfaces(type, interfaces);
            }

            if (currentType.IsInterface)
                interfaces.Add(currentType);
        }
    }
}
