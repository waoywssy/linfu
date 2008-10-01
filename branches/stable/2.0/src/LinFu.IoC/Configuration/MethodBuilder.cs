using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// A class that dynamically generates calls to a <see cref="MethodInfo"/> instance.
    /// </summary>
    public class MethodBuilder : BaseMethodBuilder<MethodInfo>
    {
        /// <summary>
        /// Pushes the method target onto the evaluation stack.
        /// </summary>
        /// <param name="IL">The <see cref="ILGenerator"/> of the method body.</param>
        /// <param name="method">The target method.</param>
        protected override void PushInstance(ILGenerator IL, MethodInfo method)
        {
            if (method.IsStatic)
                return;

            IL.Emit(OpCodes.Ldarg_0);
        }

        /// <summary>
        /// Determines the return type from the target <paramref name="method"/>.
        /// </summary>
        /// <param name="method">The target method itself.</param>
        /// <returns>The method return type.</returns>
        protected override Type GetReturnType(MethodInfo method)
        {
            return method.ReturnType;
        }

        /// <summary>
        /// Emits the instruction to call the target <paramref name="method"/>
        /// </summary>
        /// <param name="IL">The <see cref="ILGenerator"/> of the target method body.</param>
        /// <param name="method">The method that will be invoked.</param>
        protected override void EmitCall(ILGenerator IL, MethodInfo method)
        {
            var callInstruction = method.IsVirtual ? OpCodes.Callvirt : OpCodes.Call;
            IL.Emit(callInstruction, method);
        }
    }
}
