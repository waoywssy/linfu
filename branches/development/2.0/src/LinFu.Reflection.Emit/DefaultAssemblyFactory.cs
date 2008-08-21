using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Configuration;
using LinFu.Reflection.Emit.Interfaces;
using Mono.Cecil;

namespace LinFu.Reflection.Emit
{
    /// <summary>
    /// Represents the basic implementation for creating
    /// an <see cref="AssemblyDefinition"/> type.
    /// </summary>
    [Implements(typeof(IAssemblyFactory), LifecycleType.OncePerRequest)]
    public class DefaultAssemblyFactory : IAssemblyFactory
    {
        /// <summary>
        /// Creates an <see cref="AssemblyDefinition"/> type
        /// with the given <paramref name="assemblyName"/>
        /// and <paramref name="assemblyKind"/>.
        /// </summary>
        /// <param name="assemblyName">The name of the assembly to create.</param>
        /// <param name="assemblyKind">The type of binary to generate.</param>
        /// <returns>An <see cref="AssemblyDefinition"/> that represents the assembly being generated.</returns>
        public AssemblyDefinition DefineAssembly(string assemblyName, AssemblyKind assemblyKind)
        {
            var result = AssemblyFactory.DefineAssembly(assemblyName, assemblyKind);            

            return result;
        }
    }
}
