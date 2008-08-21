using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;
using LinFu.Reflection;
using LinFu.Reflection.Plugins;

namespace LinFu.IoC.Plugins
{
    /// <summary>
    /// A <see cref="ILoaderPlugin{T}"/> type that extends
    /// <see cref="Loader{IServiceContainer}"/> instance to 
    /// automatically load types marked with the <see cref="ImplementsAttribute"/>
    /// type.
    /// </summary>
    [LoaderPlugin]
    public class ImplementsAttributePlugin : BaseTargetLoaderPlugin<IServiceContainer>
    {
        /// <summary>
        /// Injects the <see cref="ImplementsAttributeLoader"/>
        /// into the current loader instance.
        /// </summary>
        /// <param name="loader">The target loader used to load a particular container.</param>
        /// <param name="assemblyLoader">The <see cref="IAssemblyTargetLoader{T}"/> that will load types into the container.</param>
        protected override void Initialize(ILoader<IServiceContainer> loader,
                                           IAssemblyTargetLoader<IServiceContainer> assemblyLoader)
        {
            assemblyLoader.TypeLoaders.Add(new ImplementsAttributeLoader());
        }
    }
}