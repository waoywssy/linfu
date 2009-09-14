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
    /// Represents a class that implements the basic behavior for rewriting a given method.
    /// </summary>
    public abstract class BaseMethodRewriter : IMethodRewriter
    {
        private readonly HashSet<TypeDefinition> _modifiedTypes = new HashSet<TypeDefinition>();

        /// <summary>
        /// Rewrites a target method using the given CilWorker.
        /// </summary>
        /// <param name="method">The target method.</param>
        /// <param name="IL">The CilWorker that will be used to rewrite the target method.</param>
        /// <param name="oldInstructions">The original instructions from the target method body.</param>
        public void Rewrite(MethodDefinition method, CilWorker IL, IEnumerable<Instruction> oldInstructions)
        {
            var declaringType = method.DeclaringType;
            var module = declaringType.Module;

            // Interfaces and Enums cannot be modified
            if (declaringType.IsInterface || declaringType.IsEnum)
                return;

            ImportReferences(module);

            AddLocals(method);

            if (!_modifiedTypes.Contains(declaringType))
            {
                AddAdditionalMembers(declaringType);
                _modifiedTypes.Add(declaringType);
            }

            RewriteMethodBody(method, IL, oldInstructions);
        }

        /// <summary>
        /// Rewrites the instructions in the target method body.
        /// </summary>
        /// <param name="method">The target method.</param>
        /// <param name="IL">The <see cref="CilWorker"/> instance that represents the method body.</param>
        /// <param name="oldInstructions">The IL instructions of the original method body.</param>
        protected abstract void RewriteMethodBody(MethodDefinition method, CilWorker IL, IEnumerable<Instruction> oldInstructions);
        

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
    }
}
