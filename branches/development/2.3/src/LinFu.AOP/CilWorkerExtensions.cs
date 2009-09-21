using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinFu.AOP.Cecil
{
    /// <summary>
    /// A class that extends the <see cref="CilWorker"/> class
    /// with helper methods that make it easier to save
    /// information about the method currently being implemented.
    /// </summary>
    public static class CilWorkerExtensions
    {
        /// <summary>
        /// Saves the ref arguments of a given method using the
        /// <paramref name="arguments"/> from the <paramref name="invocationInfo"/>
        /// object.
        /// </summary>
        /// <param name="IL">The <see cref="CilWorker"/> that will emit the method body.</param>
        /// <param name="parameters">The parameters of the target method.</param>
        /// <param name="invocationInfo">The local variable that contains the <see cref="IInvocationInfo"/> instance.</param>
        /// <param name="arguments">The local variable that will store the arguments from the <see cref="IInvocationInfo"/> instance.</param>
        public static void SaveRefArguments(this CilWorker IL, IEnumerable<ParameterDefinition> parameters,
            VariableDefinition invocationInfo, VariableDefinition arguments)
        {
            var body = IL.GetBody();
            var targetMethod = body.Method;
            var declaringType = targetMethod.DeclaringType;
            var module = declaringType.Module;

            // Save the arguments returned from the handler method
            var getArguments = module.ImportMethod<IInvocationInfo>("get_Arguments");

            IL.Emit(OpCodes.Ldloc, invocationInfo);
            IL.Emit(OpCodes.Callvirt, getArguments);
            IL.Emit(OpCodes.Stloc, arguments);

            int index = 0;
            foreach (var param in parameters)
            {
                if (!param.IsByRef())
                {
                    index++;
                    continue;
                }

                // Load the destination address
                IL.Emit(OpCodes.Ldarg, index + 1);

                // Load the argument value
                IL.Emit(OpCodes.Ldloc, arguments);
                IL.Emit(OpCodes.Ldc_I4, index++);
                IL.Emit(OpCodes.Ldelem_Ref);

                // Determine the actual parameter type
                var referenceType = param.ParameterType as ReferenceType;
                if (referenceType == null)
                    continue;

                var actualParameterType = referenceType.ElementType;
                IL.Emit(OpCodes.Unbox_Any, actualParameterType);
                IL.Stind(param.ParameterType);
            }
        }
    }
}
