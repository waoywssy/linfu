﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using LinFu.IoC.Configuration.Interfaces;

namespace LinFu.Reflection
{
    /// <summary>
    /// Represents a class that loads assemblies into memory
    /// from disk.
    /// </summary>
    public class AssemblyLoader : IAssemblyLoader
    {
        /// <summary>
        /// Loads the target assembly into memory.
        /// </summary>
        /// <param name="assemblyFile">The full path and filename of the assembly to load.</param>
        /// <returns>A loaded <see cref="Assembly"/> instance.</returns>
        public Assembly Load(string assemblyFile)
        {
            return Assembly.LoadFrom(assemblyFile);
        }
    }
}
