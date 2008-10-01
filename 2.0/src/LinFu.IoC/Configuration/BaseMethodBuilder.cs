﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using LinFu.IoC.Configuration.Interfaces;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// Represents the default implementation of the <see cref="IMethodBuilder{TMethod}"/> interface.
    /// </summary>
    /// <typeparam name="TMethod">The method type to generate.</typeparam>
    public abstract class BaseMethodBuilder<TMethod> : IMethodBuilder<TMethod>
        where TMethod : MethodBase
    {
        /// <summary>
        /// Creates a method from the <paramref name="existingMethod"/>.
        /// </summary>
        /// <param name="existingMethod">The method that will be used to define the new method.</param>
        /// <returns>A method based on the old method.</returns>
        public MethodBase CreateMethod(TMethod existingMethod)
        {
            var returnType = GetReturnType(existingMethod);
            var parameterTypes = (from p in existingMethod.GetParameters()
                                  select p.ParameterType).ToArray();

            var dynamicMethod = new DynamicMethod(string.Empty, returnType, parameterTypes);
            var IL = dynamicMethod.GetILGenerator();

            PushInstance(IL, existingMethod);

            // Push the method arguments onto the stack
            for (var index = 0; index < parameterTypes.Length; index++)
            {
                IL.Emit(OpCodes.Ldarg, index);
            }

            EmitCall(IL, existingMethod);
            IL.Emit(OpCodes.Ret);

            return dynamicMethod;
        }

        /// <summary>
        /// Pushes the method target onto the stack.
        /// </summary>
        /// <param name="IL">The <see cref="ILGenerator"/> that belongs to the method body.</param>
        /// <param name="method">The current method.</param>
        protected virtual void PushInstance(ILGenerator IL, TMethod method)
        {
        }

        /// <summary>
        /// Determines the return type from the target <paramref name="method"/>.
        /// </summary>
        /// <param name="method">The target method itself.</param>
        /// <returns>The method return type.</returns>
        protected abstract Type GetReturnType(TMethod method);

        /// <summary>
        /// Emits the instruction to call the target <paramref name="method"/>
        /// </summary>
        /// <param name="IL">The <see cref="ILGenerator"/> of the target method body.</param>
        /// <param name="method">The method that will be invoked.</param>
        protected abstract void EmitCall(ILGenerator IL, TMethod method);
    }
}
