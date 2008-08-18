using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Interfaces;

namespace LinFu.AOP.Interfaces
{
    /// <summary>
    /// Represents a special type of interceptor that can
    /// wrap itself around a method call.
    /// </summary>
    public interface IInvokeWrapper
    {
        /// <summary>
        /// This method will be called just before the actual
        /// <see cref="DoInvoke">method call</see> is executed.
        /// </summary>
        /// <param name="info">The <see cref="IInvocationInfo"/> associated with the method call.</param>
        /// <seealso cref="IInvocationInfo"/>
        void BeforeInvoke(IInvocationInfo info);

        /// <summary>
        /// This method will provide the actual implementation
        /// for the <see cref="IInvocationInfo.TargetMethod">target method</see>
        /// instance.
        /// </summary>
        /// <param name="info">The <see cref="IInvocationInfo"/> associated with the method call.</param>
        /// <returns>The actual return value from the <see cref="IInvocationInfo.TargetMethod">.</returns>
        object DoInvoke(IInvocationInfo info);

        /// <summary>
        /// This method will be called immediately after the actual
        /// <see cref="DoInvoke">method call</see> is executed.
        /// </summary>
        /// <param name="info">The <see cref="IInvocationInfo"/> associated with the method call.</param>
        /// <param name="returnValue">The value returned from the actual method call.</param>
        void AfterInvoke(IInvocationInfo info, object returnValue);
    }
}