using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.IOC
{
    public static class ContainerExtensions
    {
        public static T GetService<T>(this IContainer container)
            where T : class
        {
            var serviceType = typeof (T);
            return container.GetService(serviceType) as T;
        }
        public static T GetService<T>(this IContainer container, string serviceName)
            where T : class
        {
            return container.GetService(serviceName, typeof (T)) as T;
        }
    }
}
