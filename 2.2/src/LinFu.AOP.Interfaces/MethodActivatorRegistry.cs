using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.AOP.Interfaces
{
    /// <summary>
    /// Represents a registry that allows users to statically register <see cref="IMethodActivator"/>
    /// instances.
    /// </summary>
    public static class MethodActivatorRegistry
    {
        private static readonly object _lock = new object();
        private static IMethodActivator _activator;

        /// <summary>
        /// Obtains an activator for the given <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The <see cref="IMethodActivationContext"/> instance that describes the object to be created.</param>
        /// <returns>A method activator.</returns>
        public static IMethodActivator GetActivator(IMethodActivationContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if (_activator == null)
                return null;

            return _activator;
        }

        /// <summary>
        /// Sets the <see cref="IMethodActivator"/> that will be used to 
        /// instantiate object instances.
        /// </summary>
        /// <param name="activator">The <see cref="IMethodActivator"/> that will instantiate types.</param>
        public static void SetActivator(IMethodActivator activator)
        {
            lock (_lock)
            {
                _activator = activator;
            }
        }
    }
}
