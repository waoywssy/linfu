using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Cecil.Interfaces;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinFu.AOP.Cecil
{
    /// <summary>
    /// Provides the basic functionality for the <see cref="IMethodRewriter"/> interface.
    /// </summary>
    public abstract class MethodRewriter : IMethodRewriter
    {
        /// <summary>
        /// Initializes a new instance of the MethodRewriter class.
        /// </summary>
        protected MethodRewriter() { }

        /// <summary>
        /// Obtains the new instructions for a particular method.
        /// </summary>
        /// <param name="method">The target method.</param>
        /// <param name="IL">The <see cref="CilWorker"/> responsible for modifying the method body.</param>
        /// <param name="oldInstructions">The list of old instructions.</param>
        /// <returns>The new instructions that will be used to replace the old instructions in the target method body.</returns>
        public IEnumerable<Instruction> GetNewInstructions(MethodDefinition method, CilWorker IL, IEnumerable<Instruction> oldInstructions)
        {
            var newInstructions = new Queue<Instruction>();
            foreach (var instruction in oldInstructions)
            {
                // Intercept only the load field and the load static field instruction
                if (!ShouldReplace(instruction, method))
                {
                    newInstructions.Enqueue(instruction);
                    continue;
                }

                Replace(instruction, method, newInstructions);
            }

            return newInstructions;
        }


        /// <summary>
        /// Adds additional members to the host type.
        /// </summary>
        /// <param name="host">The host type.</param>
        public virtual void AddAdditionalMembers(TypeDefinition host)
        {
        }

        /// <summary>
        /// Adds additional references to the target module.
        /// </summary>
        /// <param name="module">The host module.</param>
        public virtual void ImportReferences(ModuleDefinition module)
        {
        }

        /// <summary>
        /// Adds local variables to the <paramref name="hostMethod"/>.
        /// </summary>
        /// <param name="hostMethod">The target method.</param>
        public virtual void AddLocals(MethodDefinition hostMethod)
        {
        }

        /// <summary>
        /// Determines whether or not the method rewriter should replace the <paramref name="oldInstruction"/>.
        /// </summary>
        /// <param name="oldInstruction">The instruction that is currently being evaluated.</param>
        /// <param name="hostMethod">The method that hosts the current instruction.</param>
        /// <returns><c>true</c> if the method should be replaced; otherwise, it should return <c>false</c>.</returns>
        protected abstract bool ShouldReplace(Instruction oldInstruction, MethodDefinition hostMethod);


        /// <summary>
        /// Replaces the <paramref name="oldInstruction"/> with a set of <paramref name="newInstructions"/>.
        /// </summary>
        /// <param name="oldInstruction">The instruction currently being evaluated.</param>
        /// <param name="hostMethod">The method that contains the target instruction.</param>
        /// <param name="newInstructions">The list of instructions that will replace the old instruction.</param>
        protected abstract void Replace(Instruction oldInstruction, MethodDefinition hostMethod, Queue<Instruction> newInstructions);
    }
}
