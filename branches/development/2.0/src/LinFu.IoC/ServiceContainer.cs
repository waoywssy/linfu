using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC
{
    /// <summary>
    /// Represents a service container with additional
    /// extension points for customizing service instances
    /// </summary>
    public class ServiceContainer : ServiceContainerBase
    {
        /// <summary>
        /// Overridden. This method modifies the original
        /// <see cref="BaseContainer.GetService"/> method
        /// so that its results can be handled by the 
        /// postprocessors.
        /// </summary>
        /// <param name="serviceName">The name of the service to instantiate.</param>
        /// <param name="serviceType">The service type to instantiate.</param>        
        /// <returns>If successful, it will return a service instance that is compatible with the given type;
        /// otherwise, it will just return a <c>null</c> value.</returns>
        public override object GetService(string serviceName, Type serviceType)
        {
            // Attempt to create the service
            // without throwing an exception
            object instance = null;

            // Save the old state
            var suppressErrors = SuppressErrors;
            lock (this)
            {
                SuppressErrors = true;
                instance = base.GetService(serviceName, serviceType);
                SuppressErrors = suppressErrors;
            }
            
            var result = PostProcess(serviceName, serviceType, instance);

            // Use the modified result, if possible; otherwise,
            // use the original result.
            instance = result.ActualResult ?? result.OriginalResult;

            if (suppressErrors == false && instance == null)
                throw new NamedServiceNotFoundException(serviceName, serviceType);

            return instance;
        }

        /// <summary>
        /// Overridden. Causes the container to instantiate the service with the given
        /// <paramref name="serviceType">service type</paramref>. If the service type cannot be created, then an
        /// exception will be thrown if the <see cref="IContainer.SuppressErrors"/> property
        /// is set to false. Otherwise, it will simply return null.
        /// </summary>
        /// <remarks>
        /// This overload of the <c>GetService</c> method has been overridden
        /// so that its results can be handled by the postprocessors.
        /// </remarks>
        /// <seealso cref="IPostProcessor"/>
        /// <param name="serviceType">The service type to instantiate.</param>
        /// <returns>If successful, it will return a service instance that is compatible with the given type;
        /// otherwise, it will just return a null value.</returns>
        public override object GetService(Type serviceType)
        {
            object instance = null;

            // Save the old state
            var suppressErrors = SuppressErrors;

            // Attempt to create the service
            // without throwing an exception
            lock (this)
            {                
                this.SuppressErrors = true;
                instance = base.GetService(serviceType);
                this.SuppressErrors = suppressErrors;
            }
            
            var result = PostProcess(string.Empty, serviceType, instance);

            // Use the modified result, if possible; otherwise,
            // use the original result.
            instance = result.ActualResult ?? result.OriginalResult;


            if (suppressErrors == false && instance == null)
                throw new ServiceNotFoundException(serviceType);

            return instance;
        }

        /// <summary>
        /// A method that searches the current container for
        /// postprocessors and passes every request result made
        /// to the list of <see cref="IServiceContainer.PostProcessors"/>.
        /// </summary>
        /// <param name="serviceName">The name of the service being requested. By default, this is usually blank.</param>
        /// <param name="serviceType">The type of service being requested.</param>
        /// <param name="instance">The original instance returned by container's service instantiation attempt.</param>
        /// <returns>A <see cref="IServiceRequestResult"/> representing the results returned as a result of the postprocessors.</returns>
        private IServiceRequestResult PostProcess(string serviceName, Type serviceType, object instance)
        {
            // Initialize the results
            var result = new ServiceRequestResult()
            {
                ServiceName = serviceName ?? string.Empty,
                ActualResult = instance,
                Container = this,
                OriginalResult = instance,
                ServiceType = serviceType
            };

            // Let each postprocessor inspect 
            // the results and/or modify the 
            // returned object instance
            foreach (var postProcessor in PostProcessors)
            {
                if (postProcessor == null)
                    continue;

                postProcessor.PostProcess(result);
            }
            
            return result;
        }
    }
}
