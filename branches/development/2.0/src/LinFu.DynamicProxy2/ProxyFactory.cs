using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LinFu.DynamicProxy2.Interfaces;
using LinFu.IoC;
using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil;
using TypeAttributes=Mono.Cecil.TypeAttributes;

namespace LinFu.DynamicProxy2
{
    /// <summary>
    /// Provides the basic implementation for a proxy factory class.
    /// </summary>
    [Implements(typeof(IProxyFactory), LifecycleType.OncePerRequest)]
    public class ProxyFactory : IProxyFactory, IInitialize
    {
        /// <summary>
        /// Initializes the proxy factory with the default values.
        /// </summary>
        public ProxyFactory()
        {
            // Use the forwarding proxy type by default
            ProxyBuilder = new ForwardingProxyBuilder();
        }
        #region IProxyFactory Members

        /// <summary>
        /// Creates a proxy type using the given
        /// <paramref name="baseType"/> as the base class
        /// and ensures that the proxy type implements the given
        /// interface types.
        /// </summary>
        /// <param name="baseType">The base class from which the proxy type will be derived.</param>
        /// <param name="baseInterfaces">The list of interfaces that the proxy will implement.</param>
        /// <returns>A forwarding proxy.</returns>
        public Type CreateProxyType(Type baseType, IEnumerable<Type> baseInterfaces)
        {
            #region Generate the assembly

            var assemblyName = Guid.NewGuid().ToString();
            var assembly = AssemblyFactory.DefineAssembly(assemblyName, AssemblyKind.Dll);
            var mainModule = assembly.MainModule;
            var importedBaseType = mainModule.Import(baseType);
            var attributes = TypeAttributes.AutoClass | TypeAttributes.Class |
                                        TypeAttributes.Public | TypeAttributes.BeforeFieldInit;

            #endregion

            #region Initialize the proxy type
            var typeName = string.Format("{0}Proxy{1}", baseType.Name, Guid.NewGuid());
            var namespaceName = "LinFu.DynamicProxy2";
            var proxyType = mainModule.DefineClass(typeName, namespaceName,
                                                              attributes, importedBaseType);

            mainModule.Types.Add(proxyType);
            #endregion

            if (ProxyBuilder == null)
                throw new NullReferenceException("The 'ProxyBuilder' property cannot be null");

            // Hand it off to the builder for construction
            ProxyBuilder.Construct(baseType, baseInterfaces, mainModule, proxyType);

            #region Compile the results
            var compiledAssembly = assembly.ToAssembly();

            var result = (from t in compiledAssembly.GetTypes()
                           where t != null && t.IsClass
                           select t).FirstOrDefault();
            #endregion

            return result;
        }

        #endregion

        /// <summary>
        /// The <see cref="IProxyBuilder"/> instance that is
        /// responsible for generating the proxy type.
        /// </summary>
        public IProxyBuilder ProxyBuilder { get; set; }

        /// <summary>
        /// Initializes the <see cref="ProxyFactory"/> instance
        /// with the <paramref name="source"/> container.
        /// </summary>
        /// <param name="source">The <see cref="IServiceContainer"/> instance that will hold the ProxyFactory.</param>
        public void Initialize(IServiceContainer source)
        {
            ProxyBuilder = source.GetService<IProxyBuilder>();
        }
    }
}