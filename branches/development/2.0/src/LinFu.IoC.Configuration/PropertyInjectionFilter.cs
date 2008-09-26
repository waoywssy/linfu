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
    public class PropertyInjectionFilter : BasePropertyInjectionFilter
    {
        /// <summary>
        /// Determines which properties should be injected from the <see cref="IServiceContainer"/> instance.
        /// </summary>
        /// <param name="container">The source container that will supply the property values for the selected properties.</param>
        /// <param name="properties">The list of properties to be filtered.</param>
        /// <returns>A list of properties that should be injected.</returns>
        protected override IEnumerable<PropertyInfo> Filter(IServiceContainer container, 
            IEnumerable<PropertyInfo> properties)
        {
            // The property must have the InjectAttribute defined and the
            // service must exist in the container
            var results = from p in properties
                          let propertyType = p.PropertyType
                          let isServiceArray = propertyType.ExistsAsServiceArray()
                          let attributes = p.GetCustomAttributes(typeof(InjectAttribute), false)
                          where isServiceArray(container) || container.Contains(propertyType)
                                                              && attributes != null && attributes.Length > 0
                          select p;

            return results;
        }
    }
}
