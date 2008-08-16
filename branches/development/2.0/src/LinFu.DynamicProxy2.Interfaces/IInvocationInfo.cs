using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LinFu.DynamicProxy2.Interfaces
{
    /// <summary>
    /// Represents the information associated with 
    /// a single method call.
    /// </summary>
    public interface IInvocationInfo
    {
        /// <summary>
        /// The target instance currently being called.
        /// </summary>
        /// <remarks>This typically is a reference to a proxy object.</remarks>
        object Target { get; }

        /// <summary>
        /// The method currently being called.
        /// </summary>
        MethodInfo TargetMethod { get; }

        /// <summary>
        /// The <see cref="StackTrace"/> associated
        /// with the method call when the call was made.
        /// </summary>
        StackTrace StackTrace { get; }

        /// <summary>
        /// This is the actual calling method that invoked the <see cref="TargetMethod"/>.
        /// </summary>
        MethodInfo CallingMethod { get; }

        /// <summary>
        /// If the <see cref="TargetMethod"/> method is a generic method, 
        /// this will hold the generic type arguments used to construct the
        /// method.
        /// </summary>
        Type[] TypeArguments { get; }

        /// <summary>
        /// The arguments used in the method call.
        /// </summary>
        object[] Arguments { get; }
    }
}
