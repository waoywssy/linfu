using System.Collections.Generic;
using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Plugins
{
    /// <summary>
    /// A class that initializes service instances that use
    /// the <see cref="IInitialize"/> interface.
    /// </summary>
    [PostProcessor]
    public class Initializer : IPostProcessor
    {
        private static readonly HashSet<IInitialize> _instances = new HashSet<IInitialize>();
        #region IPostProcessor Members

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

            // Make sure that the target is initialized only once
            if (_instances.Contains(target))
                return;

            // Initialize the target
            target.Initialize(result.Container);
            _instances.Add(target);                                    
        }

        #endregion
    }
}