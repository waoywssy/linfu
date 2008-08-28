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

            // TODO: Generate the rest of the method body
            throw new NotImplementedException();
        }

        public void Initialize(IServiceContainer source)
        {
            InvocationInfoEmitter = source.GetService<IEmitInvocationInfo>();
        }
    }
}
