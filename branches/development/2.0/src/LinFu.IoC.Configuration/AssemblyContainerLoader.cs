using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using LinFu.IoC.Configuration.Interfaces;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// Represents a class that loads configuration information
    /// from a given assembly.
    /// </summary>
    public class AssemblyContainerLoader : IContainerLoader
    {
        private readonly IList<ITypeLoader> typeLoaders = new List<ITypeLoader>();
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
        /// The list of TypeLoaders that will be used to
        /// configure the container.
        /// </summary>
        public IList<ITypeLoader> TypeLoaders
        {
            get { return typeLoaders; }
        }

        /// <summary>
        /// Determines whether or not the current <see cref="IContainerLoader"/>
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
        /// and converts it into a set of <see cref="Action{IServiceContainer}"/>
        /// instances that can be applied to a target
        /// <see cref="IServiceContainer"/> class.
        /// </summary>
        /// <remarks>This class can only load valid .NET Assemblies.</remarks>
        /// <param name="filename">The target file to be loaded.</param>
        /// <returns>A set of <see cref="Action{IServiceContainer}"/> instances to apply to a target type.</returns>
        public IEnumerable<Action<IServiceContainer>> Load(string filename)
        {
            var defaultResult = new Action<IServiceContainer>[0];

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
            var results = new List<Action<IServiceContainer>>();

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
        /// Generates the list of <see cref="Action{IServiceContainer}"/>
        /// instances which will be used to configure an <see cref="IServiceContainer"/> instance.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> instance that holds the configuration information.</param>
        /// <param name="results">The list that will hold the actions which will configure the container.</param>
        private void LoadResultsFromType(Type type, List<Action<IServiceContainer>> results)
        {
            foreach (var typeLoader in TypeLoaders)
            {
                if (typeLoader == null)
                    continue;

                var actions = typeLoader.LoadContainerFrom(type);

                if (actions.Count() == 0)
                    continue;

                results.AddRange(actions);
            }
        }
    }
}
