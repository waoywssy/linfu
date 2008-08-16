using LinFu.IoC.Configuration;
using LinFu.Reflection;
using LinFu.Reflection.Plugins;

namespace LinFu.IoC.Plugins
{
    [LoaderPlugin]
    public class ImplementsAttributePlugin : BaseTargetLoaderPlugin<IServiceContainer>
    {
        protected override void Initialize(ILoader<IServiceContainer> loader,
                                           IAssemblyTargetLoader<IServiceContainer> assemblyLoader)
        {
            assemblyLoader.TypeLoaders.Add(new ImplementsAttributeLoader());
        }
    }
}