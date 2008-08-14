using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using LinFu.IoC.Configuration.Interfaces;

namespace LinFu.Reflection
{
    /// <summary>
    /// Represents a loader class that takes <see cref="System.Type"/>
    /// instances as input and generates <see cref="Action{T}"/>
    /// instances that can be used to configure a <typeparamref name="TTarget"/>
    /// instance.
    /// </summary>
    /// <typeparam name="TTarget">The target type to configure.</typeparam>
    public class AssemblyTargetLoader<TTarget> : IActionLoader<TTarget, string>
    {
        private readonly IList<IActionLoader<TTarget, Type>> typeLoaders = new List<IActionLoader<TTarget, Type>>();

        /// <summary>
        /// The <see cref="IAssemblyLoader"/> instance that will load
        /// the target assemblies.
        /// </summary>
        public IAssemblyLoader AssemblyLoader { get; set; }

        /// <summary>
        /// The <see cref="ITypeExtractor"/> instance that will
        /// determine which types will be extracted from an assembly.
        /// </summary>
        public ITypeExtractor TypeExtractor
        {
            get;
            set;
        }
        /// <summary>
        /// The list of ActionLoaders that will be used to
        /// configure the target.
        /// </summary>
        public IList<IActionLoader<TTarget, Type>> TypeLoaders
        {
            get { return typeLoaders; }
        }
        /// <summary>
        /// Determines whether or not the current type loader
        /// instance can load the current file type.
        /// </summary>
        /// <remarks>
        /// This class only loads assemblies with the ".dll" extension.
        /// </remarks>
        /// <param name="filename">The filename and full path of the target file.</param>
        /// <returns>Returns <c>true</c> if the file can be loaded; otherwise, the result is <c>false</c>.</returns>
        public bool CanLoad(string filename)
        {
            return Path.GetExtension(filename).ToLower() == ".dll";
        }

        /// <summary>
        /// Reads an input file using the given <paramref name="filename"/>
        /// and converts it into a set of <see cref="Action{TTarget}"/>
        /// instances that can be applied to a target class instance..
        /// </summary>
        /// <remarks>This class can only load valid .NET Assemblies.</remarks>
        /// <param name="filename">The target file to be loaded.</param>
        /// <returns>A set of <see cref="Action{IServiceContainer}"/> instances to apply to a target type.</returns>
        public IEnumerable<Action<TTarget>> Load(string filename)
        {
            var defaultResult = new Action<TTarget>[0];

            Assembly assembly = null;

            if (AssemblyLoader == null)
                throw new ArgumentException("The assembly loader cannot be null");

            // Load the assembly into memory
            assembly = AssemblyLoader.Load(filename);

            // Grab the types embedded in the assembly
            IEnumerable<Type> types = new Type[0];
            if (assembly != null && TypeExtractor != null)
                types = TypeExtractor.GetTypes(assembly);

            if (types.Count() == 0 || TypeLoaders.Count == 0)
                return defaultResult;

            // Pass the loaded types to
            // the type loaders for processing
            var results = new List<Action<TTarget>>();

            foreach (var type in types)
            {
                // Skip any invalid types
                if (type == null)
                    continue;

                LoadResultsFromType(type, results);
            }

            return results;
        }
        /// <summary>
        /// Generates the list of <see cref="Action{TTarget}"/>
        /// instances which will be used to configure a target instance.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> instance that holds the configuration information.</param>
        /// <param name="results">The list that will hold the actions which will configure the container.</param>
        private void LoadResultsFromType(Type type, List<Action<TTarget>> results)
        {
            foreach (var typeLoader in TypeLoaders)
            {
                if (typeLoader == null || !typeLoader.CanLoad(type))
                    continue;

                var actions = typeLoader.Load(type);

                if (actions.Count() == 0)
                    continue;

                results.AddRange(actions);
            }
        }
    }
}
