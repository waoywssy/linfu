using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using LinFu.AOP.Cecil;
using LinFu.AOP.Cecil.Interfaces;
using LinFu.AOP.Interfaces;
using LinFu.Proxy.Interfaces;
using LinFu.IoC;
using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinFu.Proxy
{
    /// <summary>
    /// Provides the default implementation for the
    /// <see cref="IMethodBodyEmitter"/> interface.
    /// </summary>
    [Implements(typeof(IMethodBodyEmitter), LifecycleType.OncePerRequest)]
    internal class MethodBodyEmitter : IMethodBodyEmitter, IInitialize
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
        /// <param name="originalMethod">The method currently being intercepted.</param>
        /// <param name="targetMethod">The target method that will contain the new method body.</param>
        public void Emit(MethodInfo originalMethod, MethodDefinition targetMethod)
        {
            var invocationInfo = targetMethod.AddLocal<IInvocationInfo>();
            invocationInfo.Name = "___invocationInfo___";

            // Emit the code to generate the IInvocationInfo instance
            // and save it into the invocationInfo local variable
            if (InvocationInfoEmitter != null)
                InvocationInfoEmitter.Emit(originalMethod, targetMethod, invocationInfo);

            var declaringType = targetMethod.DeclaringType;
            var module = declaringType.Module;
            var proxyType = module.ImportType<IProxy>();
            var getInterceptorMethod = module.ImportMethod("get_Interceptor", typeof(IProxy));
            var interceptor = targetMethod.AddLocal<IInterceptor>();
            var arguments = targetMethod.AddLocal<object[]>();

            // if (!(this is IProxy))
            var IL = targetMethod.GetILGenerator();
            IL.Emit(OpCodes.Ldarg_0);
            IL.Emit(OpCodes.Isinst, proxyType);

            var noImplementationFound = IL.Create(OpCodes.Nop);
            IL.Emit(OpCodes.Brfalse, noImplementationFound);

            var endLabel = IL.Create(OpCodes.Nop);
            EmitGetInterceptorInstruction(IL, proxyType, getInterceptorMethod);
            IL.Emit(OpCodes.Stloc, interceptor);

            //If (interceptor == null)
            //    throw a not implemented exception here
            IL.Emit(OpCodes.Ldloc, interceptor);
            IL.Emit(OpCodes.Brfalse, noImplementationFound);


            // var returnValue = interceptor.Intercept(info);
            var voidType = module.ImportType(typeof(void));
            var interceptMethod = module.ImportMethod<IInterceptor>("Intercept", typeof(IInvocationInfo));
            IL.Emit(OpCodes.Ldloc, interceptor);
            IL.Emit(OpCodes.Ldloc, invocationInfo);
            IL.Emit(OpCodes.Callvirt, interceptMethod);

            // Save the ref arguments
            var parameters = from ParameterDefinition param in targetMethod.Parameters
                             select param;
            
            // Determine the return type
            var returnType = targetMethod.ReturnType != null ?
                targetMethod.ReturnType.ReturnType : voidType;

            IL.PackageReturnValue(module, returnType);

            IL.SaveRefArguments(parameters, invocationInfo, arguments);
            IL.Emit(OpCodes.Br, endLabel);

            // This code at this point will execute if no implementation
            // is found
            IL.Append(noImplementationFound);

            ImplementNotFound(IL);

            IL.Append(endLabel);
            IL.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// Emits the IL instructions to obtain an <see cref="IInterceptor"/> instance for the proxy type.
        /// </summary>
        /// <param name="IL">The <see cref="CilWorker"/> responsible for emitting the method body.</param>
        /// <param name="proxyType">The proxy type.</param>
        /// <param name="getInterceptorMethod">The getter method for the interceptor.</param>
        protected virtual void EmitGetInterceptorInstruction(CilWorker IL, TypeReference proxyType, MethodReference getInterceptorMethod)
        {
            IL.Emit(OpCodes.Ldarg_0);
            IL.Emit(OpCodes.Isinst, proxyType);
            IL.Emit(OpCodes.Callvirt, getInterceptorMethod);
        }

        /// <summary>
        /// Causes the <see cref="CilWorker"/> to make the method throw a
        /// <see cref="NotImplementedException"/> if the method cannot be found.
        /// </summary>
        /// <param name="IL">The <see cref="CilWorker"/> responsible for emitting the method body.</param>
        protected virtual void ImplementNotFound(CilWorker IL)
        {
            var body = IL.GetBody();
            var declaringType = body.Method.DeclaringType;
            ModuleDefinition module = declaringType.Module;

            // throw new NotImplementedException();
            var notImplementedConstructor = module.ImportConstructor<NotImplementedException>();
            IL.Emit(OpCodes.Newobj, notImplementedConstructor);
            IL.Emit(OpCodes.Throw);
        }
        
        /// <summary>
        /// Initializes the MethodBodyEmitter class.
        /// </summary>
        /// <param name="source"></param>
        public void Initialize(IServiceContainer source)
        {
            InvocationInfoEmitter = (IEmitInvocationInfo)source.GetService(typeof(IEmitInvocationInfo));
        }
    }
}
