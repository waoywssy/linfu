using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.DynamicProxy2.Interfaces
{
    /// <summary>
    /// Represents a class that can dynamically intercept method calls.
    /// </summary>
    public interface IInterceptor
    {
        /// <summary>
        /// Intercepts a method call using the given
        /// <see cref="IInvocationInfo"/> instance.
        /// </summary>
        /// <param name="info">The <see cref="IInvocationInfo"/> instance that will 
        /// contain all the necessary information associated with a 
        /// particular method call.</param>
        /// <returns></returns>
        object Intercept(IInvocationInfo info);
    }
}
