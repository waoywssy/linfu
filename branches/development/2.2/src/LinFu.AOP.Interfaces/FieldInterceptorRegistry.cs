using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.AOP.Interfaces
{
    public class FieldInterceptorRegistry
    {
        private static readonly object _lock = new object();
        private static IFieldInterceptor _interceptor;

        public static IFieldInterceptor GetInterceptor(IFieldInterceptionContext context)
        {
            lock (_lock)
            {
                return _interceptor;
            }
        }

        public static void SetInterceptor(IFieldInterceptor interceptor)
        {
            lock (_lock)
            {
                _interceptor = interceptor;
            }
        }
    }
}
