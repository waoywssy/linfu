using LinFu.IoC.Configuration;
using LinFu.Reflection;
using LinFu.Reflection.Plugins;

namespace LinFu.IoC.Plugins
{
    /// <summary>
    /// A plugin that adds support for injecting factory types marked with the
    /// <see cref="FactoryAttribute"/> into a <see cref="IServiceContainer"/>
    /// instance.
    /// </summary>
    [LoaderPlugin]
    public class FactoryAttributePlugin : BaseTargetLoaderPlugin<IServiceContainer>
    {
        /// <summary>
        /// Injects an <see cref="FactoryAttributeLoader"/>
        /// </summary>
        /// <param name="assemblyLoader">The assembly loader that will configure the service container instance.</param>
        protected override void Initialize(ILoader<IServiceContainer> loader, IAssemblyTargetLoader<IServiceContainer> assemblyLoader)
        {
            assemblyLoader.TypeLoaders.Add(new FactoryAttributeLoader());
        }
    }
}