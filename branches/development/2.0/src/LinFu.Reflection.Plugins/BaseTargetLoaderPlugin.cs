using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.Reflection;

namespace LinFu.Reflection.Plugins
{
    /// <summary>
    /// A plugin class that provides the basic implementation
    /// for plugins that work with <see cref="IAssemblyTargetLoader{TTarget}"/> instances.
    /// </summary>
    /// <typeparam name="TTarget">The target type being configured.</typeparam>
    public abstract class BaseTargetLoaderPlugin<TTarget> : ILoaderPlugin<TTarget>,
        IInitialize<ILoader<TTarget>>
    {
        public virtual void BeginLoad(TTarget target)
        {
        }

        public virtual void EndLoad(TTarget target)
        {
        }

        public void Initialize(ILoader<TTarget> source)
        {
            // Use an existing AssemblyContainerLoader
            // instance, if possible
            IAssemblyTargetLoader<TTarget> assemblyLoader = null;

            var matches = (from currentInstance in source.FileLoaders
                           where currentInstance != null &&
                                 currentInstance is IAssemblyTargetLoader<TTarget>
                           select (IAssemblyTargetLoader<TTarget>)currentInstance).ToList();

            if (matches.Count > 0)
                assemblyLoader = matches[0];

            // If no matches were found,
            // create the assembly target loader
            if (matches.Count == 0)
            {
                var loader = new AssemblyTargetLoader<TTarget>();
                source.FileLoaders.Add(loader);
                assemblyLoader = loader;
            }

            if (assemblyLoader == null)
                return;

            Initialize(assemblyLoader);
        }

        protected abstract void Initialize(IAssemblyTargetLoader<TTarget> assemblyLoader);
    }
}
