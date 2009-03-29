using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.AOP.Interfaces
{
    public static class MethodReplacementProviderRegistry
    {
        private static readonly object _lock = new object();
        private static IMethodReplacementProvider _provider;
        public static IMethodReplacementProvider GetProvider(object host, IInvocationInfo info)
        {
            if (_provider == null)
                return null;

            return _provider.CanReplace(host, info) ? _provider : null;
        }

        public static void SetProvider(IMethodReplacementProvider provider)
        {
            lock (_lock)
            {
                _provider = provider;
            }
        }
    }
}
