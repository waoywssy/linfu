using System;
using System.Collections.Generic;

namespace LinFu.Reflection
{
    /// <summary>
    /// Represents a specific <see cref="IActionLoader{TTarget, TInput}"/>
    /// type that can load configuration information from an assembly
    /// and apply it to a <typeparamref name="TTarget"/> instance.
    /// </summary>
    /// <typeparam name="TTarget">The target type to configure.</typeparam>
    public interface IAssemblyTargetLoader<TTarget> : IActionLoader<TTarget, string>
    {
        /// <summary>
        /// The <see cref="IAssemblyLoader"/> instance that will load
        /// the target assemblies.
        /// </summary>
        IAssemblyLoader AssemblyLoader { get; set; }

        /// <summary>
        /// The <see cref="ITypeExtractor"/> instance that will
        /// determine which types will be extracted from an assembly.
        /// </summary>
        ITypeExtractor TypeExtractor { get; set; }

        /// <summary>
        /// The list of ActionLoaders that will be used to
        /// configure the target.
        /// </summary>
        IList<IActionLoader<TTarget, Type>> TypeLoaders { get; }

        /// <summary>
        /// Determines whether or not the current type loader
        /// instance can load the current file type.
        /// </summary>
        /// <remarks>
        /// This class only loads assemblies with the ".dll" extension.
        /// </remarks>
        /// <param name="filename">The filename and full path of the target file.</param>
        /// <returns>Returns <c>true</c> if the file can be loaded; otherwise, the result is <c>false</c>.</returns>
        bool CanLoad(string filename);

        /// <summary>
        /// Reads an input file using the given <paramref name="filename"/>
        /// and converts it into a set of <see cref="Action{TTarget}"/>
        /// instances that can be applied to a target class instance..
        /// </summary>
        /// <remarks>This class can only load valid .NET Assemblies.</remarks>
        /// <param name="filename">The target file to be loaded.</param>
        /// <returns>A set of <see cref="Action{IServiceContainer}"/> instances to apply to a target type.</returns>
        IEnumerable<Action<TTarget>> Load(string filename);
    }
}