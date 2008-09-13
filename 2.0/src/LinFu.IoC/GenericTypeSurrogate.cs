using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC
{
    /// <summary>
    /// A postprocessor that routes all service requests
    /// for a generic type to a single <see cref="IFactory"/> class.    
    /// </summary>
    public class GenericTypeSurrogate : IPostProcessor
    {
        private readonly IFactory _factory;
        private readonly Type _typeDefinition;
        private readonly string _serviceName;

        /// <summary>
        /// Initializes the <see cref="GenericTypeSurrogate"/> class
        /// with the <paramref name="typeDefinition"/> as the family of services
        /// to be created and the <paramref name="factory"/> as the <see cref="IFactory"/>
        /// instance that will be used to create the service instances.
        /// </summary>
        /// <param name="typeDefinition">The generic type definition that represents the family of services that will be created.</param>
        /// <param name="factory">The <see cref="IFactory"/> instance that will be used to create the service instances.</param>
        public GenericTypeSurrogate(Type typeDefinition, IFactory factory)
        {
            _typeDefinition = typeDefinition;
            _factory = factory;
        }

        /// <summary>
        /// Initializes the <see cref="GenericTypeSurrogate"/> class
        /// with the <paramref name="typeDefinition"/> as the family of services
        /// to be created and the <paramref name="factory"/> as the <see cref="IFactory"/>
        /// instance that will be used to create the service instances.
        /// </summary>
        /// <param name="serviceName">The service name that will be associated with the family of types that will be created.</param>
        /// <param name="typeDefinition">The generic type definition that represents the family of services that will be created.</param>
        /// <param name="factory">The <see cref="IFactory"/> instance that will be used to create the service instances.</param>
        public GenericTypeSurrogate(string serviceName, Type typeDefinition, IFactory factory)
        {
            _serviceName = serviceName;
            _typeDefinition = typeDefinition;
            _factory = factory;
        }

        /// <summary>
        /// The <see cref="Type"/> instance that represents the family of 
        /// services that the factory will be able to create.
        /// </summary>
        public Type GenericTypeDefinition
        {
            get { return _typeDefinition; }
        }

        /// <summary>
        /// The <see cref="IFactory"/> instance that will be responsible
        /// for creating service instances that are a part of the family of services
        /// that represent the <see cref="GenericTypeDefinition"/>.
        /// </summary>
        public IFactory Factory
        {
            get { return _factory; }
        }

        /// <summary>
        /// The service name that will be associated with the family of types that will be created.
        /// </summary>
        public string ServiceName
        {
            get { return _serviceName; }
        }

        /// <summary>
        /// Intercepts the requests for a particular generic <see cref="GenericTypeDefinition">service instance</see>
        /// and routes the requests to the <see cref="Factory"/> instance.
        /// </summary>
        /// <param name="result">The <see cref="IServiceRequestResult"/> that represents the intercepted service request.</param>
        public void PostProcess(IServiceRequestResult result)
        {
            // Replace the result if and only
            // if the container returned nothing
            if (result.OriginalResult != null || Factory == null)
                return;

            // Match the service name, if possible
            if (!string.IsNullOrEmpty(ServiceName) && result.ServiceName != ServiceName)
                return;

            Type serviceType = result.ServiceType;

            // The type must be a generic
            if (!serviceType.IsGenericType)
                return;

            // The requested service must have the same
            // type definition as the one defined on this surrogate
            Type typeDefinition = serviceType.GetGenericTypeDefinition();
            if (typeDefinition != GenericTypeDefinition)
                return;

            // Pass the call to the factory
            result.ActualResult = Factory.CreateInstance(serviceType, result.Container);
        }
    }
}
