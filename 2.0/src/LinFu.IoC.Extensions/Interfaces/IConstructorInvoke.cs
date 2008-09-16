using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LinFu.IoC.Extensions.Interfaces
{
    /// <summary>
    /// Represents a type that can instantiate an object instance
    /// using a given constructor and a given set of constructor arguments.
    /// </summary>
    public interface IConstructorInvoke
    {
        /// <summary>
        /// Instantiates an object instance with the <paramref name="targetConstructor"/>
        /// and <paramref name="arguments"/>.
        /// </summary>
        /// <param name="targetConstructor">The constructor that will be used to create the object instance.</param>
        /// <param name="arguments">The arguments to be used with the constructor.</param>
        /// <returns>An object reference that represents the newly-instantiated object.</returns>
        object CreateWith(ConstructorInfo targetConstructor, object[] arguments);
    }
}
