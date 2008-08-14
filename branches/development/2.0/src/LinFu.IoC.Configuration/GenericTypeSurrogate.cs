using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// A postprocessor that routes all service requests
    /// for a generic type to a single <see cref="IFactory"/> class.    
    /// </summary>
    public class GenericTypeSurrogate : IPostProcessor
    {
        private readonly Type _typeDefinition;
        private readonly IFactory _factory;
        public GenericTypeSurrogate(Type typeDefinition, IFactory factory)
        {
            _typeDefinition = typeDefinition;
            _factory = factory;
        }

        public Type GenericTypeDefinition
        {
            get { return _typeDefinition; }
        }

        public IFactory Factory
        {
            get { return _factory; }
        }

        public void PostProcess(IServiceRequestResult result)
        {
            // Replace the result if and only
            // if the container returned nothing
            if (result.OriginalResult != null || Factory == null)
                return;

            var serviceType = result.ServiceType;

            // The type must be a generic
            if (!serviceType.IsGenericType)
                return;

            // The requested service must have the same
            // type definition as the one defined on this surrogate
            var typeDefinition = serviceType.GetGenericTypeDefinition();
            if (typeDefinition != GenericTypeDefinition)
                return;

            // Pass the call to the factory
            result.ActualResult = Factory.CreateInstance(serviceType, result.Container);
        }
    }
}
