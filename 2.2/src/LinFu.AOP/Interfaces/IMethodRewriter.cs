using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinFu.AOP.Cecil.Interfaces
{
    /// <summary>
    /// Represents a type that can modify method bodies.
    /// </summary>
    public interface IMethodRewriter : IHostWeaver<TypeDefinition>
    {
        /// <summary>
        /// Obtains the new instructions for a particular method.
        /// </summary>
        /// <param name="method">The target method.</param>
        /// <param name="IL">The <see cref="CilWorker"/> responsible for modifying the method body.</param>
        /// <param name="oldInstructions">The list of old instructions.</param>
        /// <returns>The new instructions that will be used to replace the old instructions in the target method body.</returns>
        IEnumerable<Instruction> GetNewInstructions(MethodDefinition method, CilWorker IL, IEnumerable<Instruction> oldInstructions);
    }
}
