using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace LinFu.Reflection.Emit.Interfaces
{
    /// <summary>
    /// Represents a factory that is capable of creating
    /// <see cref="AssemblyDefinition"/> types.
    /// </summary>
    public interface IAssemblyFactory
    {
        /// <summary>
        /// Creates an <see cref="AssemblyDefinition"/> type
        /// with the given <paramref name="assemblyName"/>
        /// and <paramref name="assemblyKind"/>.
        /// </summary>
        /// <param name="assemblyName">The name of the assembly to create.</param>
        /// <param name="assemblyKind">The type of binary to generate.</param>
        /// <returns>An <see cref="AssemblyDefinition"/> that represents the assembly being generated.</returns>
        AssemblyDefinition DefineAssembly(string assemblyName, AssemblyKind assemblyKind);
    }
}
