using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Configuration;
using LinFu.Reflection;
using LinFu.Reflection.Plugins;

namespace LinFu.IoC.Plugins
{
    [LoaderPlugin]
    public class ImplementsAttributePlugin : BaseTargetLoaderPlugin<IServiceContainer>
    {
        protected override void Initialize(IAssemblyTargetLoader<IServiceContainer> assemblyLoader)
        {
            assemblyLoader.TypeLoaders.Add(new ImplementsAttributeLoader());
        }
    }
}
