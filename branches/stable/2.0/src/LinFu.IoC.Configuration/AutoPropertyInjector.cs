using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Configuration.Interfaces;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Configuration
{
    internal class AutoPropertyInjector : IPostProcessor, IContainerPlugin
    {
        private static readonly HashSet<Type> _excludedServices = new HashSet<Type>(new
            Type[] { typeof(IPropertyInjectionFilter), typeof(IArgumentResolver), typeof(IPropertySetter) });

        private bool _inProcess = false;
        /// <summary>
        /// Automatically injects service instances
        /// into properties as soon as they are initialized.
        /// </summary>
        /// <param name="result"></param>
        public void PostProcess(IServiceRequestResult result)
        {
            // Prevent recursion
            if (_inProcess)
                return;
            
            lock (this)
            {
                _inProcess = true;
            }

            AutoInject(result);

            lock (this)
            {
                _inProcess = false;
            }
        }

        private void AutoInject(IServiceRequestResult result)
        {
            // Ignore the excluded services
            if (_excludedServices.Contains(result.ServiceType))
                return;

            var container = result.Container;
            var filter = container.GetService<IPropertyInjectionFilter>();
            if (filter == null || result.ActualResult == null)
                return;

            // Determine which properties can be injected
            var targetType = result.ActualResult.GetType();
            var properties = filter.GetInjectableProperties(targetType).ToList();
            if (properties.Count == 0)
                return;

            // Get the setter instance
            var setter = container.GetService<IPropertySetter>();
            if (setter == null)
                return;

            // Use the resolver to determine
            // which property value should be injected
            // into the setter
            var resolver = container.GetService<IArgumentResolver>();
            if (resolver == null)
                return;

            var target = result.ActualResult;
            foreach (var property in properties)
            {
                var results = resolver.ResolveFrom(new Type[] { property.PropertyType }, container);
                var propertyValue = results.FirstOrDefault();

                setter.Set(target, property, propertyValue);
            }
        }

        /// <summary>
        /// Does absolutely nothing.
        /// </summary>
        /// <param name="target">The target container.</param>
        public void BeginLoad(IServiceContainer target)
        {
        }

        /// <summary>
        /// Injects the <see cref="AutoPropertyInjector"/> class at the end
        /// of the PostProcessor chain.
        /// </summary>
        /// <param name="target">The target container.</param>
        public void EndLoad(IServiceContainer target)
        {
            target.PostProcessors.Add(this);
        }
    }
}
