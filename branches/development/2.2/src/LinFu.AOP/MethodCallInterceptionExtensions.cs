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
        public static void InterceptMethodCalls(this IReflectionStructureVisitable target, Func<TypeReference, bool> typeFilter, Func<MethodReference, bool> hostMethodFilter, Func<MethodReference, bool> methodCallFilter)
        {
            var rewriter = new InterceptMethodCalls(hostMethodFilter, methodCallFilter);
            target.Accept(new ImplementMethodReplacementHost(typeFilter));
            target.WeaveWith(rewriter, hostMethodFilter);            
        }

        public static void InterceptMethodCalls(this IReflectionVisitable target, Func<TypeReference, bool> typeFilter, Func<MethodReference, bool> hostMethodFilter, Func<MethodReference, bool> methodCallFilter)
        {
            var rewriter = new InterceptMethodCalls(hostMethodFilter, methodCallFilter);
            target.Accept(new ImplementMethodReplacementHost(typeFilter));
            target.WeaveWith(rewriter, hostMethodFilter);
        }
    }
}