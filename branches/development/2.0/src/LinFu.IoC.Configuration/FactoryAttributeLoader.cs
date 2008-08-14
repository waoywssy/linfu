using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LinFu.IoC.Configuration.Interfaces;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// A class that injects custom <see cref="IFactory"/> and <see cref="IFactory{T}"/>
    /// instances into an <see cref="IServiceContainer"/> instance.
    /// </summary>
    public class FactoryAttributeLoader : ITypeLoader
    {
        /// <summary>
        /// Loads an <see cref="IFactory"/> and <see cref="IFactory{T}"/> instance
        /// into a <see cref="IServiceContainer"/> instance using the given
        /// <paramref name="sourceType"/>.
        /// </summary>
        /// <param name="sourceType">The input type from which one or more factories will be created.</param>
        /// <returns>A set of <see cref="Action{IServiceContainer}"/> instances. This cannot be null.</returns>
        public IEnumerable<Action<IServiceContainer>> Load(Type sourceType)
        {
            if (sourceType == null)
                throw new ArgumentNullException("sourceType");

            // Extract the factory attributes from the current type
            var attributes = sourceType.GetCustomAttributes(typeof(FactoryAttribute), true);
            var attributeList = attributes.Cast<FactoryAttribute>()
                .Where(f => f != null).ToList();

            #region Validation
            // The target type must have at least one
            // factory attribute
            if (attributeList.Count == 0)
                return new Action<IServiceContainer>[0];

            // The factory must have a default constructor
            // in order to be instantiated
            var defaultConstructor = sourceType.GetConstructor(new Type[0]);
            if (defaultConstructor == null)
                throw new ArgumentException("The type '{0}' needs a default constructor to be instantiated.", sourceType.AssemblyQualifiedName);

            // Make sure the factory is created only once
            var factoryInstance = Activator.CreateInstance(sourceType, new object[0]);
            if (factoryInstance == null)
                throw new NullReferenceException(string.Format("Unable to create factory type '{0}'",
                    sourceType.AssemblyQualifiedName));


            // The factory instance must implement either
            // IFactory or IFactory<T>
            var untypedInstance = factoryInstance as IFactory;
            var factoryInterfaces = (from t in sourceType.GetInterfaces()
                                     where t.IsGenericType &&
                                     t.GetGenericTypeDefinition() == typeof(IFactory<>)
                                     select t);


            if (untypedInstance == null && factoryInterfaces.Count() == 0)
            {
                var message = string.Format("The factory type '{0}' must implement either the IFactory interface or the IFactory<T> interface.", sourceType.AssemblyQualifiedName);
                throw new ArgumentException(message, "sourceType");
            }
            #endregion

            var implementedInterfaces = new HashSet<Type>(factoryInterfaces);

            Func<Type, object, IFactory> createFactory =
                (currentServiceType, factory) =>
                {                    
                    // Determine if the factory implements
                    // the generic IFactory<T> instance
                    // and use that instance if possible
                    IFactory result = null;
                    var genericType = typeof(IFactory<>).MakeGenericType(currentServiceType);
                    
                    if (implementedInterfaces.Contains(genericType))
                    {
                        // Convert the IFactory<T> instance down to an IFactory
                        // instance so that it can be used by the target container
                        var adapterType = typeof (FactoryAdapter<>).MakeGenericType(currentServiceType);
                        result = (IFactory)Activator.CreateInstance(adapterType, new object[] {factory});
                        return result;
                    }                    

                    // Otherwise, use the untyped IFactory instance instead
                    result = factory as IFactory;
                    return result;
                };

            
            // Build the list of services that this factory can implement
            var servicesToImplement = from f in attributeList
                                      let serviceName = f.ServiceName
                                      let serviceType = f.ServiceType
                                      let factory = createFactory(serviceType, factoryInstance)
                                      where factory != null
                                      select new
                                                 {
                                                     ServiceName=serviceName, 
                                                     ServiceType=serviceType,
                                                     FactoryInstance=factory
                                                 };
            
            
            var results = new List<Action<IServiceContainer>>();
            foreach(var currentService in servicesToImplement)
            {
                var serviceName = currentService.ServiceName;
                var serviceType = currentService.ServiceType;
                var factory = currentService.FactoryInstance;


                // Add each service to the container on initialization
                if (!serviceType.IsGenericTypeDefinition)
                {
                    results.Add(container => container.AddFactory(serviceName, serviceType, factory));
                    continue;
                }

                // HACK: Use a GenericTypeSurrogate to route
                // generic service requests to a 
                // single factory instance
                var surrogate = new GenericTypeSurrogate(serviceType, factory);
                results.Add(container => container.PostProcessors.Add(surrogate));
            }

            return results;
        }

        public bool CanLoad(Type sourceType)
        {
            return sourceType.IsClass;
        }
    }
}
