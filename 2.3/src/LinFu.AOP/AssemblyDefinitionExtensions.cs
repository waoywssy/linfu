using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinFu.AOP.Cecil
{
    /// <summary>
    /// A class that extends <see cref="AssemblyDefinition"/> instances.
    /// </summary>
    public static class AssemblyDefinitionExtensions
    {
        /// <summary>
        /// Removes the strong-name signature from the <paramref name="sourceAssembly"/>.
        /// </summary>
        /// <param name="sourceAssembly"></param>
        public static void RemoveStrongName(this AssemblyDefinition sourceAssembly)
        {
            var nameDef = sourceAssembly.Name;

            // Remove the strong name
            nameDef.PublicKey = null;
            nameDef.PublicKeyToken = null;
            nameDef.HashAlgorithm = AssemblyHashAlgorithm.None;
            nameDef.Flags = ~AssemblyFlags.PublicKey;
            nameDef.HasPublicKey = false;
        }

        /// <summary>
        /// Gets a type from the given assembly.
        /// </summary>
        /// <param name="assembly">The assembly that contains the target type.</param>
        /// <param name="className">The type name of the type to be retrieved from the assembly.</param>
        /// <returns>A type that matches the given <paramref name="className"/>.</returns>
        public static TypeDefinition GetType(this AssemblyDefinition assembly, string className)
        {
            var module = assembly.MainModule;
            var targetTypeDefinition = (from TypeDefinition t in module.Types
                                        where t.Name == className
                                        select t).First();

            return targetTypeDefinition;
        }
    }
}
