using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LinFu.IoC;
using LinFu.IoC.Interfaces;
using LinFu.Reflection.Emit;
using LinFu.DynamicProxy2.Interfaces;
using LinFu.IoC.Configuration;
using Mono.Cecil;

namespace LinFu.DynamicProxy2
{
    /// <summary>
    /// Represents the default implementation of the
    /// <see cref="IMethodBuilder"/> interface.
    /// </summary>
    [Implements(typeof(IMethodBuilder), LifecycleType.OncePerRequest, ServiceName = "ProxyMethodBuilder")]
    public class ProxyMethodBuilder : IMethodBuilder, IInitialize
    {
        /// <summary>
        /// Initializes the <see cref="ProxyMethodBuilder"/> class with the default property values.
        /// </summary>
        public ProxyMethodBuilder()
        {
            Emitter = new MethodBodyEmitter();
        }
        /// <summary>
        /// Creates a method that matches the signature defined in the
        /// <paramref name="method"/> parameter.
        /// </summary>
        /// <param name="targetType">The type that will host the new method.</param>
        /// <param name="method">The method from which the signature will be derived.</param>
        public MethodDefinition CreateMethod(TypeDefinition targetType, MethodInfo method)
        {
            #region Match the method signature
            var module = targetType.Module;
            var returnType = module.ImportType(method.ReturnType);
            var methodName = method.Name;
            var baseAttributes = Mono.Cecil.MethodAttributes.Virtual | 
                Mono.Cecil.MethodAttributes.HideBySig | Mono.Cecil.MethodAttributes.NewSlot;

            var attributes = default(Mono.Cecil.MethodAttributes);

            #region Match the visibility of the target method

            if (method.IsFamilyOrAssembly)
                attributes = baseAttributes | Mono.Cecil.MethodAttributes.FamORAssem;

            if (method.IsFamilyAndAssembly)
                attributes = baseAttributes | Mono.Cecil.MethodAttributes.FamANDAssem;

            if (method.IsPublic)
                attributes = baseAttributes | Mono.Cecil.MethodAttributes.Public;
            
            #endregion

            // Build the list of parameter types
            var parameterTypes = (from param in method.GetParameters()
                                  let type = param.ParameterType
                                  let importedType = type != null ? module.ImportType(type) : null
                                  where importedType != null
                                  select importedType).ToArray();
            
            
            var newMethod = targetType.DefineMethod(methodName, attributes,
                                                    returnType, parameterTypes);

            newMethod.Body.InitLocals = true;
            newMethod.ImplAttributes = Mono.Cecil.MethodImplAttributes.IL | Mono.Cecil.MethodImplAttributes.Managed;
            newMethod.HasThis = true;
            
            // Match the generic type arguments
            var typeArguments = method.GetGenericArguments();

            if (typeArguments != null || typeArguments.Length > 0)
                MatchGenericArguments(newMethod, typeArguments);
            
            #endregion

            // Define the method body
            if (Emitter != null)
                Emitter.Emit(method, newMethod);

            return newMethod;
        }

        /// <summary>
        /// The <see cref="IMethodBodyEmitter"/> instance that will be
        /// responsible for generating the method body.
        /// </summary>
        public IMethodBodyEmitter Emitter
        {
            get; set;
        }

        /// <summary>
        /// Matches the generic parameters of <paramref name="newMethod">a target method</paramref>
        /// </summary>
        /// <param name="newMethod">The generic method that contains the generic type arguments.</param>
        /// <param name="typeArguments">The array of <see cref="Type"/> objects that describe the generic parameters for the current method.</param>
        private static void MatchGenericArguments(IGenericParameterProvider newMethod, ICollection<Type> typeArguments)
        {
            var typeNames = new List<string>();
            for (var index = 0; index < typeArguments.Count; index++)
            {
                typeNames.Add(string.Format("T{0}", index));
            }

            foreach (var name in typeNames)
            {
                var parameter = new GenericParameter(name, newMethod);
                newMethod.GenericParameters.Add(parameter);
            }
        }

        /// <summary>
        /// Initializes the class with the <paramref name="source"/> container.
        /// </summary>
        /// <param name="source">The <see cref="IServiceContainer"/> instance that will create the class instance.</param>
        public void Initialize(IServiceContainer source)
        {
            Emitter = source.GetService<IMethodBodyEmitter>();
        }
    }
}
