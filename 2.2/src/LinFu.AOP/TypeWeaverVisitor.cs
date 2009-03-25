using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Cecil.Interfaces;
using Mono.Cecil;

namespace LinFu.AOP.Cecil
{
    /// <summary>
    /// Represents a visitor class that can iterate over <see cref="TypeDefinition"/>
    /// instances.
    /// </summary>
    public class TypeWeaverVisitor : LinFu.AOP.Cecil.BaseReflectionVisitor
    {
        private ITypeWeaver _weaver;

        /// <summary>
        /// Initializes a new instance of the TypeWeaverVisitor class.
        /// </summary>
        /// <param name="weaver">The <see cref="ITypeWeaver"/> that will be used to modify a given type.</param>
        public TypeWeaverVisitor(ITypeWeaver weaver)
        {
            _weaver = weaver;
        }

        /// <summary>
        /// Visits a <see cref="ModuleDefinition"/> instance.
        /// </summary>
        /// <param name="module">A <see cref="ModuleDefinition"/> object.</param>
        public override void VisitModuleDefinition(ModuleDefinition module)
        {
            _weaver.ImportReferences(module);
            _weaver.AddAdditionalMembers(module);

            base.VisitModuleDefinition(module);
        }

        /// <summary>
        /// Visits a <see cref="TypeDefinition"/> instance.
        /// </summary>
        /// <param name="type">A <see cref="TypeDefinition"/> object.</param>
        public override void VisitTypeDefinition(TypeDefinition type)
        {
            if (!_weaver.ShouldWeave(type))
                return;

            _weaver.Weave(type);

            base.VisitTypeDefinition(type);
        }
    }
}
