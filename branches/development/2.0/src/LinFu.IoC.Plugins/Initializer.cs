using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Plugins
{
    /// <summary>
    /// A class that initializes service instances that use
    /// the <see cref="IInitialize"/> interface.
    /// </summary>
    internal class Initializer : IPostProcessor
    {
        /// <summary>
        /// Initializes every service that implements
        /// the <see cref="IInitialize"/> interface.
        /// </summary>
        /// <param name="result">The <see cref="IServiceRequestResult"/> instance that contains the service instance to be initialized.</param>
        public void PostProcess(IServiceRequestResult result)
        {
            var target = result.OriginalResult as IInitialize;
            if (target == null)
                return;
            
            target.Initialize(result.Container);
        }
    }
}
