using System;
using System.IO;
using System.Linq;
using LinFu.IoC.Configuration.Interfaces;
using LinFu.IoC.Interfaces;
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
            QueuedActions.Add(container => container.AddService<IPropertyInjectionFilter>(new PropertyInjectionFilter()));

            // Load everything else into the container
            var hostAssembly = typeof(Loader).Assembly;
            QueuedActions.Add(container => container.LoadFrom(hostAssembly));


            // Make sure that the plugins are only added once
            if (!Plugins.HasElementWith(p=>p is InitializerPlugin))
                Plugins.Add(new InitializerPlugin());

            if (!Plugins.HasElementWith(p => p is AutoPropertyInjector))
                Plugins.Add(new AutoPropertyInjector());

            FileLoaders.Add(containerLoader);
        }
    }
}
