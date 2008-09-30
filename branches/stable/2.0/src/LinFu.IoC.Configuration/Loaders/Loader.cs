using System;
using System.IO;
using System.Linq;
using System.Reflection;
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
            AddService<IArgumentResolver, ArgumentResolver>();

            // Add the constructor related services
            AddService<IMemberResolver<ConstructorInfo>, ConstructorResolver>();
            AddService<IMethodBuilder<ConstructorInfo>, ConstructorMethodBuilder>();
            AddService<IMethodInvoke<ConstructorInfo>, MethodInvoke<ConstructorInfo>>();
            AddService<IMethodBuilder<MethodInfo>, MethodBuilder>();

            // Add the method finders for constructors and methods
            AddService<IMethodFinder<ConstructorInfo>, MethodFinder<ConstructorInfo>>();            
            AddService<IMethodFinder<MethodInfo>, MethodFinder<MethodInfo>>();

            // Add the injection filters
            AddService<IMemberInjectionFilter<MethodInfo>, AttributedMethodInjectionFilter>();
            AddService<IMemberInjectionFilter<PropertyInfo>, AttributedPropertyInjectionFilter>();
            AddService<IMemberInjectionFilter<FieldInfo>, AttributedFieldInjectionFilter>();

            // Load everything else into the container
            var hostAssembly = typeof(Loader).Assembly;
            QueuedActions.Add(container => container.LoadFrom(hostAssembly));


            // Make sure that the plugins are only added once
            if (!Plugins.HasElementWith(p => p is InitializerPlugin))
                Plugins.Add(new InitializerPlugin());

            if (!Plugins.HasElementWith(p => p is AutoPropertyInjector))
                Plugins.Add(new AutoPropertyInjector());

            if (!Plugins.HasElementWith(p => p is AutoMethodInjector))
                Plugins.Add(new AutoMethodInjector());

            if (!Plugins.HasElementWith(p => p is AutoFieldInjector))
                Plugins.Add(new AutoFieldInjector());

            FileLoaders.Add(containerLoader);
        }

        /// <summary>
        /// Adds a service to the list of default services that will be implemented by the container.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <typeparam name="TImplementation">The concrete service implementation type.</typeparam>
        private void AddService<TService, TImplementation>()
            where TImplementation : TService, new()
        {
            QueuedActions.Add(container => container.AddService<TService>(new TImplementation()));
        }
    }
}
