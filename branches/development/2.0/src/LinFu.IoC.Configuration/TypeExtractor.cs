﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LinFu.IoC.Configuration.Interfaces;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// Represents a type that can extract <see cref="System.Type"/>
    /// objects from an <see cref="Assembly"/> instance.
    /// </summary>
    public class TypeExtractor : ITypeExtractor
    {
        /// <summary>
        /// Returns a set of types from a given assembly.
        /// </summary>
        /// <param name="targetAssembly">The <see cref="Assembly"/> that contains the target types.</param>
        /// <returns>An <see cref="IEnumerable{Type}"/> of types from the target assembly.</returns>
        public IEnumerable<Type> GetTypes(Assembly targetAssembly)
        {
            Type[] loadedTypes = null;
            try
            {
                loadedTypes = targetAssembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                loadedTypes = ex.Types;
            }
            return loadedTypes;
        }
    }
}
