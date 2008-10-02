﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Interceptors
{
    /// <summary>
    /// Represents a class that automatically injects a proxy instance
    /// instead of an actual service instance.
    /// </summary>
    internal class ProxyInjector : IPostProcessor
    {
        private readonly Func<IServiceRequestResult, bool> _filterPredicate;
        private readonly Func<IServiceRequestResult, object> _createProxy;

        /// <summary>
        /// Initializes the class with the <paramref name="filterPredicate"/>
        /// and the <paramref name="createProxy"/> factory method.
        /// </summary>
        /// <param name="filterPredicate">The predicate that will determine which service requests will be proxied.</param>
        /// <param name="createProxy">The factory method that will generate the proxy instance itself.</param>
        internal ProxyInjector(Func<IServiceRequestResult, bool> filterPredicate, 
            Func<IServiceRequestResult, object> createProxy)
        {
            _filterPredicate = filterPredicate;
            _createProxy = createProxy;
        }

        public void PostProcess(IServiceRequestResult result)
        {
            if (!_filterPredicate(result))
                return;

            // Replace the actual result with the proxy itself
            result.ActualResult = _createProxy(result);
        }
    }
}