using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace LinFu.AOP.Cecil
{
    /// <summary>
    /// Represents an extension class that adds method body interception support to the Mono.Cecil object model.
    /// </summary>
    public static class MethodBodyInterceptionExtensions
    {
        public static void InterceptMethodBodies(this IReflectionVisitable target, Func<MethodDefinition, bool> methodFilter)
        {
            InterceptMethodBody interceptMethodBody;
            Func<MethodReference, bool> methodInterceptionFilter;
            Func<TypeReference, bool> typeFilter = AddMethodBodyInterception(methodFilter, out interceptMethodBody, out methodInterceptionFilter);
            
            target.WeaveWith(interceptMethodBody, methodInterceptionFilter);
            target.Accept(new ImplementModifiableType(typeFilter));
        }

        public static void InterceptMethodBodies(this IReflectionStructureVisitable target, Func<MethodDefinition, bool> methodFilter)
        {
            InterceptMethodBody interceptMethodBody;
            Func<MethodReference, bool> methodInterceptionFilter;
            Func<TypeReference, bool> typeFilter = AddMethodBodyInterception(methodFilter, out interceptMethodBody, out methodInterceptionFilter);

            target.WeaveWith(interceptMethodBody, methodInterceptionFilter);
            target.Accept(new ImplementModifiableType(typeFilter));
        }

        private static Func<TypeReference, bool> AddMethodBodyInterception(Func<MethodDefinition, bool> methodFilter, out InterceptMethodBody interceptMethodBody, out Func<MethodReference, bool> methodInterceptionFilter)
        {
            Func<TypeReference, bool> typeFilter = type =>
                                                       {
                                                           var actualType = type.Resolve();
                                                           if (actualType.IsValueType || actualType.IsInterface)
                                                               return false;

                                                           return actualType.IsClass;                                                           
                                                       };

            // Ensure that the type filter is consistent with the method filter
            Func<MethodDefinition, bool> actualFilter =
                method => methodFilter(method) && typeFilter(method.DeclaringType);

            methodInterceptionFilter = (method => actualFilter(method.Resolve()));

            interceptMethodBody = new InterceptMethodBody(actualFilter);
            return typeFilter;
        }
    }
}
