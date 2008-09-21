using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LinFu.IoC.Configuration.Interfaces;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// A default implementation of the <see cref="IPropertyInjectionFilter"/>
    /// class that returns properties which have the <see cref="InjectAttribute"/>
    /// defined.
    /// </summary>
    [Implements(typeof(IPropertyInjectionFilter), LifecycleType.OncePerRequest)]
    public class PropertyInjectionFilter : IPropertyInjectionFilter
    {
        /// <summary>
        /// Returns the list of <see cref="PropertyInfo"/> objects
        /// whose setters should be injected with arbitrary values.
        /// </summary>
        /// <remarks>This implementation selects properties that are marked with the <see cref="InjectAttribute"/>.</remarks>
        /// <param name="targetType">The target type that contains the target properties.</param>
        /// <returns>A set of properties that describe which parameters should be injected.</returns>
        public IEnumerable<PropertyInfo> GetInjectableProperties(Type targetType)
        {
            var results = from p in targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                          let attributes = p.GetCustomAttributes(typeof(InjectAttribute), false) 
                          where attributes != null && attributes.Length > 0 && p.CanWrite
                          select p;

            return results;
        }
    }
}
