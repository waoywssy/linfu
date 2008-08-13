using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LinFu.IoC.Configuration.Interfaces
{
    /// <summary>
    /// Represents a type that can extract <see cref="System.Type"/>
    /// objects from an <see cref="Assembly"/> instance.
    /// </summary>
    public interface ITypeExtractor
    {
        /// <summary>
        /// Returns a set of types from a given assembly.
        /// </summary>
        /// <param name="targetAssembly">The <see cref="Assembly"/> that contains the target types.</param>
        /// <returns>An <see cref="IEnumerable{Type}"/> of types from the target assembly.</returns>
        IEnumerable<Type> GetTypes(Assembly targetAssembly);
    }
}
