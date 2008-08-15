using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.Reflection;

namespace LinFu.IoC.Plugins
{
    /// <summary>
    /// Ensures that every service instance that implments
    /// the <see cref="IInitialize{IServiceContainer}"/> interface
    /// will be initialized with the current service container.
    /// </summary>
    [LoaderPlugin]
    public class InitializerPlugin : ILoaderPlugin<IServiceContainer>
    {
        /// <summary>
        /// Injects the initializer into the <paramref name="target"/>
        /// container instance at the beginning of the load operation.
        /// </summary>
        /// <param name="target">The container being loaded.</param>
        public void BeginLoad(IServiceContainer target)
        {
            // Inject the initializer
            target.PostProcessors.Add(new Initializer());
        }
        
        /// <summary>
        /// Overridden. This method implementation does absolutely nothing.
        /// </summary>
        /// <param name="target">The container being loaded.</param>
        public void EndLoad(IServiceContainer target)
        {
            // Do nothing.
        }
    }
}
