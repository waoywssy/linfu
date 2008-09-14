using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;
using LinFu.Reflection;
using LinFu.Reflection.Plugins;

namespace LinFu.IoC.Plugins
{
    /// <summary>
    /// A plugin that injects postprocessors marked
    /// with the PostProcessorAttribute into any
    /// container currently being configured.
    /// </summary>
    [LoaderPlugin]
    public class PostProcessorPlugin : BaseTargetLoaderPlugin<IServiceContainer>
    {
        /// <summary>
        /// Initializes a <paramref name="loader"/> so that
        /// every <see cref="IPostProcessor"/> class that is marked
        /// with the <see cref="PostProcessorAttribute"/> will
        /// automatically be loaded into memory.
        /// </summary>
        /// <param name="loader">The loader currently being configured.</param>
        /// <param name="assemblyLoader">The <see cref="IAssemblyTargetLoader{TTarget}"/> 
        /// instance that will configure the container using the type information 
        /// embedded in an assembly.</param>
        protected override void Initialize(ILoader<IServiceContainer> loader, IAssemblyTargetLoader<IServiceContainer> assemblyLoader)
        {
            assemblyLoader.TypeLoaders.Add(new PostProcessorLoader());
        }
    }
}