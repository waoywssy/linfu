using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LinFu.IoC.Configuration.Interfaces;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// Defines the basic behavior of the <see cref="IPropertyInjectionFilter"/> interface.
    /// </summary>
    public abstract class BasePropertyInjectionFilter : IPropertyInjectionFilter, IInitialize
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
        public virtual IEnumerable<PropertyInfo> GetInjectableProperties(Type targetType)
        {
            IEnumerable<PropertyInfo> properties = null;

            // Retrieve the property list only once
            if (!_propertyCache.ContainsKey(targetType))
            {
                // The property must have a getter and the current type
                // must exist as either a service list or exist as an 
                // existing service inside the current container
                properties = from p in targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                             let propertyType = p.PropertyType
                             let isServiceArray = propertyType.ExistsAsServiceArray()
                             let isCompatible = isServiceArray(_container) || _container.Contains(propertyType)
                             where p.CanWrite && isCompatible
                             select p;

                lock (_propertyCache)
                {
                    _propertyCache[targetType] = properties;
                }
            }

            properties = _propertyCache[targetType];

            return Filter(_container, properties);
        }

        /// <summary>
        /// Determines which properties should be injected from the <see cref="IServiceContainer"/> instance.
        /// </summary>
        /// <param name="container">The source container that will supply the property values for the selected properties.</param>
        /// <param name="properties">The list of properties to be filtered.</param>
        /// <returns>A list of properties that should be injected.</returns>
        protected virtual IEnumerable<PropertyInfo> Filter(IServiceContainer container,
                                                            IEnumerable<PropertyInfo> properties)
        {
            return properties;
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