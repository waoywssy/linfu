using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinFu.Reflection.Emit
{
    /// <summary>
    /// A class that extends the <see cref="MethodDefinition"/>
    /// class with features similar to the features in the
    /// System.Reflection.Emit namespace.
    /// </summary>
    public static class MethodDefinitionExtensions
    {
        /// <summary>
        /// Returns the <see cref="CilWorker"/> instance
        /// associated with the body of the <paramref name="method">target method</paramref>.
        /// </summary>
        /// <param name="method">The target method to be modified.</param>
        /// <returns>The <see cref="CilWorker"/> instance that points to the instructions of the method body.</returns>
        public static CilWorker GetILGenerator(this MethodDefinition method)
        {
            return method.Body.CilWorker;
        }
        
        /// <summary>
        /// Adds a <see cref="VariableDefinition">local variable</see>
        /// instance to the target <paramref name="methodDef">method definition</paramref>.
        /// </summary>
        /// <param name="methodDef">The <paramref name="methodDef"/> instance which will contain the local variable.</param>
        /// <param name="localType">The object <see cref="System.Type">type</see> that describes the type of objects that will be stored by the local variable.</param>
        /// <returns>A <see cref="VariableDefinition"/> that represents the local variable itself.</returns>
        public static VariableDefinition AddLocal(this MethodDefinition methodDef, Type localType)
        {
            var declaringType = methodDef.DeclaringType;
            var module = declaringType.Module;
            var variableType = module.Import(localType);
            var result = new VariableDefinition(variableType);

            methodDef.Body.Variables.Add(result);

            return result;
        }
        
        /// <summary>
        /// Adds a <see cref="VariableDefinition">local variable</see>
        /// instance to the target <paramref name="methodDef">method definition</paramref>.
        /// </summary>
        /// <typeparam name="T">The object <see cref="System.Type">type</see> that describes the type of objects that will be stored by the local variable.</typeparam>
        /// <param name="methodDef">The <paramref name="methodDef"/> instance which will contain the local variable.</param>        
        /// <returns>A <see cref="VariableDefinition"/> that represents the local variable itself.</returns>        
        public static VariableDefinition AddLocal<T>(this MethodDefinition methodDef)
        {
            return methodDef.AddLocal(typeof (T));
        }
    }
}
