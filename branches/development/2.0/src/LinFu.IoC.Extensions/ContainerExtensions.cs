using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC;

namespace LinFu.IoC
{
    public static class ContainerExtensions
    {
        public static T GetService<T>(this IContainer container)
            where T : class
        {
            var serviceType = typeof (T);
            return container.GetService(serviceType) as T;
        }
        public static T GetService<T>(this INamedContainer container, string serviceName)
            where T : class
        {
            return container.GetService(serviceName, typeof (T)) as T;
        }
        public static void AddFactory<T>(this INamedContainer container, string serviceName, IFactory<T> factory)
        {
            IFactory adapter = new FactoryAdapter<T>(factory);
            container.AddFactory(serviceName, typeof (T), adapter);
        }

        public static void AddFactory<T>(this IContainer container, IFactory<T> factory)
        {
            IFactory adapter = new FactoryAdapter<T>(factory);
            container.AddFactory(typeof(T), adapter);
        }
        public static void AddService<T>(this IContainer container, T instance)
        {
            container.AddFactory(typeof (T), new InstanceFactory(instance));
        }
    }
}
