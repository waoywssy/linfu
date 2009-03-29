using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.Reflection.Emit;
using Mono.Cecil;

namespace LinFu.AOP.Cecil
{
    public static class MethodCallInterceptionExtensions
    {
        public static void InterceptMethodCalls(this AssemblyDefinition assembly,
            Func<MethodReference, bool> methodWeaverFilter, Func<TypeReference, bool> typeFilter)
        {
            // Make the method filter consistent with the type filter
            Func<MethodReference, bool> methodFilter =
                method =>
                {
                    var declaringType = method.GetDeclaringType();
                    return methodWeaverFilter(method) && typeFilter(declaringType);
                };

            assembly.Accept(new ImplementMethodReplacementHost(typeFilter));
            assembly.WeaveWith(new InterceptMethodCalls(methodFilter), methodFilter);
        }
    }
}