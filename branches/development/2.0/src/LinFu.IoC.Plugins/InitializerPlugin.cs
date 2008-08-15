using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.Reflection;

namespace LinFu.IoC.Plugins
{
    [LoaderPlugin]
    public class InitializerPlugin : ILoaderPlugin<IServiceContainer>
    {
        public void BeginLoad(IServiceContainer target)
        {
            // Inject the initializer
            target.PostProcessors.Add(new Initializer());
        }

        public void EndLoad(IServiceContainer target)
        {
            
        }
    }
}
