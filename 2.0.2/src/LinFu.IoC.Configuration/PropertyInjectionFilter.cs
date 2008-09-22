using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LinFu.IoC.Configuration.Interfaces;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// A default implementation of the <see cref="IPropertyInjectionFilter"/>
    /// class that returns properties which have the <see cref="InjectAttribute"/>
    /// defined.
    /// </summary>
    [Implements(typeof(IPropertyInjectionFilter), LifecycleType.OncePerRequest)]
    public class PropertyInjectionFilter : IPropertyInjectionFilter, IInitialize
    {
        private static readonly Dictionary<Type, IEnumerable<PropertyInfo>> _propertyCache =
            new Dictionary<Type, IEnumerable<PropertyInfo>>();

        private IServiceContainer _container;
        /// <summary>
        /// Returns the list of <see cref="PropertyInfo"/> objects
        /// whose setters should be injected with arbitrary values.
        /// </summary>
        /// <remarks>This implementation selects properties that are marked with the <see cref="InjectAttribute"/>.</remarks>
        /// <param name="targetType">The target type that contains the target properties.</param>
        /// <returns>A set of properties that describe which parameters should be injected.</returns>
        public IEnumerable<PropertyInfo> GetInjectableProperties(Type targetType)
        {
            IEnumerable<PropertyInfo> properties = null;

            // Retrieve the property list only once
            if (!_propertyCache.ContainsKey(targetType))
            {
                properties = from p in targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                          let propertyType = p.PropertyType
                          where p.CanWrite
                          select p;
                
                lock(_propertyCache)
                {
                    _propertyCache[targetType] = properties;
                }
            }

            properties = _propertyCache[targetType];

            // The property must have the InjectAttribute defined and the
            // service must exist in the container
            var results = from p in properties
                          let propertyType = p.PropertyType
                          let isServiceArray = propertyType.ExistsAsServiceArray()
                          let attributes = p.GetCustomAttributes(typeof(InjectAttribute), false) 
                          where isServiceArray(_container) || _container.Contains(propertyType)
                          && attributes != null && attributes.Length > 0
                          select p;

            return results;
        }

        /// <summary>
        /// Initializes the <see cref="PropertyInjectionFilter"/> class.
        /// </summary>
        /// <param name="source">The host container.</param>
        public void Initialize(IServiceContainer source)
        {
            _container = source;
        }
    }
}
