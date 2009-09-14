﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinFu.AOP.Cecil
{
    /// <summary>
    /// Represents a class that adds helper methods to the <see cref="MethodDefinition"/> class.
    /// </summary>
    public static class MethodDefinitionExtensions
    {
        /// <summary>
        /// Adds a local variable to the target <paramref name="methodDef">method</paramref>.
        /// </summary>
        /// <param name="methodDef">The target method.</param>
        /// <param name="localType">The <see cref="TypeReference"/> that represents the local variable type.</param>
        /// <returns>A <see cref="VariableDefinition"/> instance that represents the newly-created local variable.</returns>
        public static VariableDefinition AddLocal(this MethodDefinition methodDef, Type localType)
        {
            var declaringType = methodDef.DeclaringType;
            var module = declaringType.Module;
            var variableType = module.Import(localType);
            var result = new VariableDefinition(variableType);

            methodDef.Body.Variables.Add(result);

            return result;
        }
    }
}
