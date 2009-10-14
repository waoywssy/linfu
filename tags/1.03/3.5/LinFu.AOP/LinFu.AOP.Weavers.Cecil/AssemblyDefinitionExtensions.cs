using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

using LinFu.AOP.CecilExtensions;

namespace LinFu.AOP.Weavers.Cecil
{
    public static class AssemblyDefinitionExtensions
    {
        public static void InjectAspectFramework(this AssemblyDefinition assembly, 
            LinFu.AOP.CecilExtensions.ITypeFilter typeFilter,
            IMethodFilter methodFilter,
            bool shouldInjectConstructors)
        {
            AspectWeaver weaver = new AspectWeaver();
            weaver.MethodFilter = methodFilter;
            assembly.WeaveWith(typeFilter, weaver);
            if (!shouldInjectConstructors)
                return;

            ConstructorCrossCutter crosscutter = new ConstructorCrossCutter();
            assembly.WeaveWith(crosscutter);
        }
        public static void InjectAspectFramework(this AssemblyDefinition assembly,
            bool shouldInjectConstructors)
        {
            // Use default type and method filters
            InjectAspectFramework(assembly, null, null, shouldInjectConstructors);
        }
        public static void InjectAspectFramework(this AssemblyDefinition assembly)
        {
            assembly.InjectAspectFramework(true);
        }
    }
}
