using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using LinFu.AOP;
using LinFu.AOP.Interfaces;
using LinFu.DynamicProxy2.Interfaces;
using LinFu.IoC;
using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinFu.DynamicProxy2
{
    /// <summary>
    /// Provides the default implementation for the
    /// <see cref="IMethodBodyEmitter"/> interface.
    /// </summary>
    [Implements(typeof (IMethodBodyEmitter), LifecycleType.OncePerRequest)]
    public class MethodBodyEmitter : IMethodBodyEmitter, IInitialize 
    {        
        /// <summary>
        /// Initializes the class with the default values.
        /// </summary>
        public MethodBodyEmitter()
        {
            InvocationInfoEmitter = new InvocationInfoEmitter();
        }

        /// <summary>
        /// The <see cref="IEmitInvocationInfo"/> instance that
        /// </summary>
        public IEmitInvocationInfo InvocationInfoEmitter { get; set; }

        /// <summary>
        /// Generates a method body for the <paramref name="targetMethod"/>.
        /// </summary>
        /// <param name="targetMethod">The target method that will contain the new method body.</param>
        public void Emit(MethodDefinition targetMethod)
        {
            var invocationInfo = targetMethod.AddLocal<IInvocationInfo>();

            // Emit the code to generate the IInvocationInfo instance
            // and save it into the invocationInfo local variable
            if (InvocationInfoEmitter != null)
                InvocationInfoEmitter.Emit(targetMethod, invocationInfo);

            var declaringType = targetMethod.DeclaringType;
            var module = declaringType.Module;
            var proxyType = module.ImportType<IProxy>();
            var getInterceptorMethod = module.ImportMethod("get_Interceptor", typeof (IProxy));
            var interceptor = targetMethod.AddLocal<IInterceptor>();
            var arguments = targetMethod.AddLocal(typeof (object[]));

            // if (!(this is IProxy))
            var IL = targetMethod.GetILGenerator();
            IL.Emit(OpCodes.Ldarg_0);
            IL.Emit(OpCodes.Isinst, proxyType);

            var throwNotImplementedException = IL.Create(OpCodes.Nop);
            IL.Emit(OpCodes.Brfalse, throwNotImplementedException);

            var endLabel = IL.Create(OpCodes.Nop);
            IL.Emit(OpCodes.Ldarg_0);
            IL.Emit(OpCodes.Isinst, proxyType);
            IL.Emit(OpCodes.Callvirt, getInterceptorMethod);
            IL.Emit(OpCodes.Stloc, interceptor);
            
            // If (interceptor == null)
            //     throw a not implemented exception here
            IL.Emit(OpCodes.Ldloc, interceptor);
            IL.Emit(OpCodes.Brfalse, throwNotImplementedException);


            // var returnValue = interceptor.Intercept(info);
            var voidType = module.ImportType(typeof (void));
            var interceptMethod = module.ImportMethod<IInterceptor>("Intercept", typeof (IInvocationInfo));
            IL.Emit(OpCodes.Ldloc, interceptor);
            IL.Emit(OpCodes.Ldloc, invocationInfo);
            IL.Emit(OpCodes.Callvirt, interceptMethod);

            // Save the ref arguments
            var parameters = from ParameterDefinition param in targetMethod.Parameters
                             select param;

            SaveRefArguments(IL, parameters, invocationInfo, arguments);

            // Determine the return type
            var returnType = targetMethod.ReturnType != null ?
                targetMethod.ReturnType.ReturnType : voidType;

            IL.PackageReturnValue(module, returnType);
            IL.Emit(OpCodes.Br, endLabel);

            // Mark the throwNotImplementedException label            
            IL.Append(throwNotImplementedException);

            // throw new NotImplementedException();
            var notImplementedConstructor = module.ImportConstructor<NotImplementedException>();
            IL.Emit(OpCodes.Newobj, notImplementedConstructor);
            IL.Emit(OpCodes.Throw);

            IL.Append(endLabel);
            IL.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// Saves the ref arguments of a given method using the
        /// <paramref name="arguments"/> from the <paramref name="invocationInfo"/>
        /// object.
        /// </summary>
        /// <param name="IL">The <see cref="CilWorker"/> that will emit the method body.</param>
        /// <param name="parameters">The parameters of the target method.</param>
        /// <param name="invocationInfo">The local variable that contains the <see cref="IInvocationInfo"/> instance.</param>
        /// <param name="arguments">The local variable that will store the arguments from the <see cref="IInvocationInfo"/> instance.</param>
        private static void SaveRefArguments(CilWorker IL, IEnumerable<ParameterDefinition> parameters,
            VariableDefinition invocationInfo, VariableDefinition arguments)
        {
            var body = IL.GetBody();
            var targetMethod = body.Method;
            var declaringType = targetMethod.DeclaringType;
            var module = declaringType.Module;

            // Save the arguments returned from the handler method
            var getArguments = module.ImportMethod<IInvocationInfo>("get_Arguments");

            IL.Emit(OpCodes.Ldloc, invocationInfo);
            IL.Emit(OpCodes.Call, getArguments);
            IL.Emit(OpCodes.Stloc, arguments);

            foreach (var param in parameters)
            {
                var typeName = param.ParameterType.Name;                
                if (!param.IsByRef())
                    continue;

                // Load the destination address
                IL.Emit(OpCodes.Ldarg, param.Sequence + 1);

                // Load the argument value
                IL.Emit(OpCodes.Ldloc, arguments);
                IL.Emit(OpCodes.Ldc_I4, param.Sequence);
                IL.Emit(OpCodes.Ldelem_Ref);

                typeName = typeName.Replace("&", "");
                Type unboxedType = Type.GetType(typeName);
                var unboxedTypeRef = module.ImportType(unboxedType);

                IL.Emit(OpCodes.Unbox_Any, unboxedTypeRef);
                IL.Stind(param.ParameterType);                
            }
        }
        /// <summary>
        /// Initializes the MethodBodyEmitter class.
        /// </summary>
        /// <param name="source"></param>
        public void Initialize(IServiceContainer source)
        {
            InvocationInfoEmitter = source.GetService<IEmitInvocationInfo>();
        }
    }
}
