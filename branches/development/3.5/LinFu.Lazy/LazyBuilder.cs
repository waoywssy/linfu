using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.DynamicProxy;

namespace LinFu.Lazy
{
    public static class LazyBuilder
    {
        public static T CreateLazyItem<T>(Func<T> createItem)
            where T : class
        {
            ProxyFactory factory = new ProxyFactory();
            T result = factory.CreateProxy<T>(new LazyInterceptor<T>(createItem));

            return result;
        }        
    }
}
