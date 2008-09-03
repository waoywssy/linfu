using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using LinFu.AOP.Interfaces;
using LinFu.IoC.Configuration;
using LinFu.Reflection.Emit;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinFu.AOP
{
    /// <summary>
    /// Represents the default implementation for the
    /// <see cref="IEmitInvocationInfo"/> class.
    /// </summary>
    [Implements(typeof(IEmitInvocationInfo), LifecycleType.OncePerRequest)]
    public class InvocationInfoEmitter : IEmitInvocationInfo
    {
        private static readonly ConstructorInfo _invocationInfoConstructor;
        private static readonly MethodInfo _getTypeFromHandle;
        static InvocationInfoEmitter()
        {
            var types = new [] { typeof(object), 
                                        typeof(MethodInfo), 
                                        typeof(StackTrace), 
                                        typeof(Type[]), 
                                        typeof(Type[]), 
                                        typeof(Type), 
                                        typeof(object[]) };

            _invocationInfoConstructor = typeof (InvocationInfo).GetConstructor(types);

            _getTypeFromHandle = typeof (Type).GetMethod("GetTypeFromHandle",
                                                         BindingFlags.Static | BindingFlags.Public);
        }
        #region IEmitInvocationInfo Members

        public void Emit(MethodDefinition targetMethod, VariableDefinition invocationInfo)
        {
            var module = targetMethod.DeclaringType.Module;
            var currentMethod = targetMethod.AddLocal(typeof(MethodBase));
            var parameterTypes = targetMethod.AddLocal(typeof(Type[]));
            var arguments = targetMethod.AddLocal(typeof(object[]));
            var typeArguments = targetMethod.AddLocal(typeof(Type[]));
            var systemType = module.ImportType(typeof(Type));

            var IL = targetMethod.GetILGenerator();
            #region Initialize the InvocationInfo constructor arguments

            // Type[] typeArguments = new Type[genericTypeCount];
            var genericParameterCount = targetMethod.GenericParameters.Count;
            IL.Emit(OpCodes.Ldc_I4, genericParameterCount);
            IL.Emit(OpCodes.Newarr, systemType);
            IL.Emit(OpCodes.Stloc, typeArguments);

            // Push the generic type arguments onto the stack
            if (genericParameterCount > 0)
                IL.PushGenericArguments(targetMethod, module, typeArguments);

            // object[] arguments = new object[argumentCount];            
            IL.PushArguments(targetMethod, module, arguments);

            // object target = this;
            IL.PushInstance(targetMethod);
            IL.PushMethod(targetMethod, module);

            IL.Emit(OpCodes.Stloc, currentMethod);

            // MethodInfo targetMethod = currentMethod as MethodInfo;
            var methodInfoType = module.Import(typeof(MethodInfo));
            IL.Emit(OpCodes.Ldloc, currentMethod);
            IL.Emit(OpCodes.Isinst, methodInfoType);

            // Get the current stack trace
            IL.PushStackTrace(module);

            // Push the type arguments back onto the stack
            IL.Emit(OpCodes.Ldloc, typeArguments);

            // Save the parameter types
            IL.Emit(OpCodes.Ldc_I4, targetMethod.Parameters.Count);
            IL.Emit(OpCodes.Newarr, systemType);
            IL.Emit(OpCodes.Stloc, parameterTypes);
            IL.SaveParameterTypes(targetMethod, module, parameterTypes);
            IL.Emit(OpCodes.Ldloc, parameterTypes);

            // Save the return type
            var getTypeFromHandle = module.Import(_getTypeFromHandle);

            var returnType = targetMethod.ReturnType.ReturnType;
            IL.Emit(OpCodes.Ldtoken, returnType);
            IL.Emit(OpCodes.Call, getTypeFromHandle);

            // Push the arguments back onto the stack
            IL.Emit(OpCodes.Ldloc, arguments);
            #endregion

            // InvocationInfo info = new InvocationInfo(...);
            var infoConstructor = module.Import(_invocationInfoConstructor);
            IL.Emit(OpCodes.Newobj, infoConstructor);
            IL.Emit(OpCodes.Stloc, invocationInfo);
        }

        #endregion
    }
}
