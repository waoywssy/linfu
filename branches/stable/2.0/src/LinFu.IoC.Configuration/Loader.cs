using System.IO;
using LinFu.IoC.Configuration.Interfaces;
using LinFu.IoC.Interfaces;
using LinFu.IoC.Plugins;
using LinFu.Reflection;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// Represents a class that can dynamically configure
    /// <see cref="IServiceContainer"/> instances at runtime.
    /// </summary>
    public class Loader : Loader<IServiceContainer>
    {
        /// <summary>
        /// Initializes the loader using the default values.
        /// </summary>
        public Loader()
        {
            var containerLoader = new AssemblyContainerLoader();
            containerLoader.TypeLoaders.Add(new FactoryAttributeLoader());
            containerLoader.TypeLoaders.Add(new ImplementsAttributeLoader());
            containerLoader.TypeLoaders.Add(new PostProcessorLoader());

            // Add the default services
            QueuedActions.Add(container => container.AddService<IArgumentResolver>(new ArgumentResolver()));
            QueuedActions.Add(container => container.AddService<IConstructorInvoke>(new ConstructorInvoke()));
            QueuedActions.Add(container => container.AddService<IConstructorResolver>(new ConstructorResolver()));
            QueuedActions.Add(container => container.PostProcessors.Add(new Initializer()));

            Plugins.Add(new InitializerPlugin());

            FileLoaders.Add(containerLoader);
        }
    }
}
