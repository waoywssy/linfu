using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LinFu.IoC.Configuration;
using LinFu.IoC.Extensions.Interfaces;
using System.Reflection.Emit;

namespace LinFu.IoC.Extensions
{
    /// <summary>
    /// Represents the default implementation of the <see cref="IConstructorInvoke"/> interface.
    /// </summary>
    [Implements(typeof(IConstructorInvoke), LifecycleType.Singleton)]
    public class ConstructorInvoke : IConstructorInvoke
    {
        private static readonly Dictionary<ConstructorInfo, MethodInfo> _cache = new Dictionary<ConstructorInfo, MethodInfo>();

        /// <summary>
        /// Instantiates an object instance with the <paramref name="targetConstructor"/>
        /// and <paramref name="arguments"/>.
        /// </summary>
        /// <param name="targetConstructor">The constructor that will be used to create the object instance.</param>
        /// <param name="arguments">The arguments to be used with the constructor.</param>
        /// <returns>An object reference that represents the newly-instantiated object.</returns>
        public object CreateWith(ConstructorInfo targetConstructor, 
            object[] arguments)
        {
            object result = null;

            // Reuse the cached results, if possible
            if (!_cache.ContainsKey(targetConstructor))
            {
                GenerateFactoryMethod(targetConstructor);
            }

            var factoryMethod = _cache[targetConstructor];

            result = factoryMethod.Invoke(null, arguments);

            return result;
        }

        /// <summary>
        /// Creates a <see cref="DynamicMethod"/> that will be used as the 
        /// factory method for creating a new type.
        /// </summary>
        /// <param name="targetConstructor">The constructor that will be used to instantiate the target type.</param>
        private static void GenerateFactoryMethod(ConstructorInfo targetConstructor)
        {
            var returnType = targetConstructor.DeclaringType;
            var parameterTypes = (from p in targetConstructor.GetParameters()
                                  select p.ParameterType).ToArray();

            var dynamicMethod = new DynamicMethod(string.Empty, returnType, parameterTypes);
            var IL = dynamicMethod.GetILGenerator();

            // Push the constructor arguments onto the stack
            for(var index = 0; index < parameterTypes.Length; index++)
            {
                IL.Emit(OpCodes.Ldarg, index);
            }

            // Instantiate the object
            IL.Emit(OpCodes.Newobj, targetConstructor);
            IL.Emit(OpCodes.Ret);

            // Save the results
            lock (_cache)
            {
                _cache[targetConstructor] = dynamicMethod;
            }
        }
    }
}
