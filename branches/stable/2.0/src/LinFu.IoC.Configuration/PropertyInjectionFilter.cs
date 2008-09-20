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
        public IEnumerable<PropertyInfo> GetInjectableProperties(Type targetType)
        {
            var results = from p in targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                          let attributes = p.GetCustomAttributes(typeof(InjectAttribute), false) 
                          where attributes != null && attributes.Length > 0
                          select p;

            return results;
        }
    }
}
