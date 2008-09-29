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
    public class AttributedPropertyInjectionFilter : BasePropertyInjectionFilter
    {
        private readonly Type _attributeType;

        /// <summary>
        /// Initializes the class and uses the <see cref="InjectAttribute"/>
        /// to specify which properties should be automatically injected with
        /// services from the container.
        /// </summary>
        public AttributedPropertyInjectionFilter()
        {
            _attributeType = typeof (InjectAttribute);
        }

        /// <summary>
        /// Initializes the class and uses the <paramref name="attributeType"/>
        /// to specify which properties should be automatically injected with
        /// services from the container.
        /// </summary>
        /// <param name="attributeType">The custom property attribute that will be used to mark properties for automatic injection.</param>
        public AttributedPropertyInjectionFilter(Type attributeType)
        {
            _attributeType = attributeType;
        }

        /// <summary>
        /// Determines which properties should be injected from the <see cref="IServiceContainer"/> instance.
        /// </summary>
        /// <param name="container">The source container that will supply the property values for the selected properties.</param>
        /// <param name="properties">The list of properties to be filtered.</param>
        /// <returns>A list of properties that should be injected.</returns>
        protected override IEnumerable<PropertyInfo> Filter(IServiceContainer container, 
            IEnumerable<PropertyInfo> properties)
        {
            // The property must have the custom attribute defined 
            var results = from p in properties
                          let propertyType = p.PropertyType                          
                          let attributes = p.GetCustomAttributes(_attributeType, false)
                          where attributes != null && attributes.Length > 0
                          select p;

            return results;
        }
    }
}
