using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Extensions.Interfaces
{
    /// <summary>
    /// Represents a type that can generate constructor arguments
    /// from an existing <see cref="IServiceContainer"/> instance.
    /// </summary>
    public interface IArgumentResolver
    {
        /// <summary>
        /// Generates constructor arguments from the given <paramref name="constructor"/>
        /// and <paramref name="container"/>.
        /// </summary>
        /// <param name="constructor">The constructor that will be used to instantiate an object instance.</param>
        /// <param name="container">The container that will provide the constructor arguments.</param>
        /// <returns>An array of objects that represent the arguments to be passed to the target constructor.</returns>
        object[] ResolveFrom(ConstructorInfo constructor, IServiceContainer container);
    }
}
