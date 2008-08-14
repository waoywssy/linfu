using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.Reflection;

namespace LinFu.IoC.Configuration.Interfaces
{
    /// <summary>
    /// Generates one or more <see cref="Action{IServiceContainer}"/> instances
    /// from a given source type so that it can be used
    /// against an <see cref="IContainer"/> instance.
    /// </summary>
    public interface ITypeLoader : IActionLoader<IServiceContainer, Type>
    {
    }
}
