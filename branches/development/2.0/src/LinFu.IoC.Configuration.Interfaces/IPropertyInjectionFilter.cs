using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LinFu.IoC.Configuration.Interfaces
{
    /// <summary>
    /// An interface responsible for determining which properties
    /// should be injected.
    /// </summary>
    public interface IPropertyInjectionFilter
    {
        /// <summary>
        /// Returns the list of <see cref="PropertyInfo"/> objects
        /// whose setters should be injected with arbitrary values.
        /// </summary>
        /// <param name="targetType">The target type that contains the target properties.</param>
        /// <returns>A set of properties that describe which parameters should be injected.</returns>
        IEnumerable<PropertyInfo> GetInjectableProperties(Type targetType);
    }
}
