using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.IoC.Interfaces
{
    /// <summary>
    /// Represents the results returned when a service request
    /// is made against an <see cref="IContainer"/> instance.
    /// </summary>
    public interface IServiceRequestResult
    {
        /// <summary>
        /// The name of the service being created. By default, this property is blank.
        /// </summary>
        string ServiceName { get; }

        /// <summary>
        /// The raw object reference created by the container itself.
        /// </summary>
        object OriginalResult { get; }

        /// <summary>
        /// The result that will be returned from the container
        /// instead of the <see cref="OriginalResult"/>. 
        /// 
        /// If this property is null, then the original result will be used.
        /// </summary>
        object ActualResult { get; set; }

        /// <summary>
        /// The type of service being requested.
        /// </summary>
        Type ServiceType { get;  }

        /// <summary>
        /// The container that will handle the service request.
        /// </summary>
        IContainer Container { get; }
    }
}
