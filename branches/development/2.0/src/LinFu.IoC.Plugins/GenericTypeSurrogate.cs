using System;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Plugins
{
    /// <summary>
    /// A postprocessor that routes all service requests
    /// for a generic type to a single <see cref="IFactory"/> class.    
    /// </summary>
    public class GenericTypeSurrogate : IPostProcessor
    {
        private readonly IFactory _factory;
        private readonly Type _typeDefinition;

        /// <summary>
        /// Initializes the class using the <paramref name="typeDefinition"/>
        /// parameter as the generic type definition type that the <paramref name="factory"/>
        /// will be able to create.
        /// </summary>
        /// <param name="typeDefinition">The class of generic types that this factory will be able to create.</param>
        /// <param name="factory">The <see cref="IFactory"/> instance itself.</param>
        public GenericTypeSurrogate(Type typeDefinition, IFactory factory)
        {
            _typeDefinition = typeDefinition;
            _factory = factory;
        }

        /// <summary>
        /// The generic type definition that will define
        /// the family of types that can be created by the
        /// <see cref="Factory"/> instance.
        /// </summary>
        public Type GenericTypeDefinition
        {
            get { return _typeDefinition; }
        }

        /// <summary>
        /// The <see cref="IFactory">factory</see> responsible
        /// for creating the generic types.
        /// </summary>
        public IFactory Factory
        {
            get { return _factory; }
        }

        /// <summary>
        /// This method routes all service requests
        /// for a generic type to the <see cref="Factory"/> instance.
        /// </summary>
        /// <param name="result">The result of the original service request.</param>
        /// <seealso cref="IServiceRequestResult"/>
        public void PostProcess(IServiceRequestResult result)
        {
            // Replace the result if and only
            // if the container returned nothing
            if (result.OriginalResult != null || Factory == null)
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