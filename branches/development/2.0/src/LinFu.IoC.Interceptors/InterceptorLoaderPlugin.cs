using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Interfaces;
using LinFu.Reflection.Plugins;
using LinFu.Reflection;

namespace LinFu.IoC.Interceptors
{
    /// <summary>
    /// Represents the class that adds interception support to <see cref="IServiceContainer"/>
    /// instances.
    /// </summary>
    [LoaderPlugin]
    public class InterceptorLoaderPlugin : BaseTargetLoaderPlugin<IServiceContainer>
    {
        /// <summary>
        /// Injects the <see cref="InterceptorAttributeLoader"/> class into
        /// the <see cref="IAssemblyTargetLoader{TTarget}"/> instance
        /// so that types marked with the <see cref="InterceptsAttribute"/>
        /// can be loaded as interceptors.
        /// </summary>
        /// <param name="loader">The target loader.</param>
        /// <param name="assemblyLoader">The assembly loader that will load the target types.</param>
        protected override void Initialize(ILoader<IServiceContainer> loader, 
            IAssemblyTargetLoader<IServiceContainer> assemblyLoader)
        {   
            // Only add the attribute loader once
            if (assemblyLoader.TypeLoaders.HasElementWith(e=>e is InterceptorAttributeLoader))
                return;

            assemblyLoader.TypeLoaders.Add(new InterceptorAttributeLoader(loader));
        }       
    }
}
