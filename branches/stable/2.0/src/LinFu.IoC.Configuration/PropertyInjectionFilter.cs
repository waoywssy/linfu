using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Configuration.Interfaces;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// An <see cref="IPropertyInjectionFilter"/> implementation
    /// that automatically selects properties whose property types
    /// currently exist in the target container.
    /// </summary>
    public class PropertyInjectionFilter : BasePropertyInjectionFilter
    {
    }
}
