using System;
using System.Collections.Generic;
using System.Linq;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC
{
    /// <summary>
    /// Represents a basic inversion of control container
    /// with support for creating custom service instances.
    /// </summary>
    public abstract class BaseContainer : IContainer
    {
        private readonly IFactoryStorage _factoryStorage = new FactoryStorage();

        /// <summary>
        /// Gets or sets a <see cref="bool">System.Boolean</see> value
        /// that determines whether or not the container should throw
        /// a <see cref="ServiceNotFoundException"/> if a requested service
        /// cannot be found or created.
        /// </summary>
        public virtual bool SuppressErrors { get; set; }

        /// <summary>
        /// Adds an <see cref="IFactory"/> instance and associates it
        /// with the given <paramref name="serviceType">service type</paramref>.
        /// </summary>
        /// <param name="serviceType">The service type to associate with the factory</param>
        /// <param name="factory">The <see cref="IFactory"/> instance that will be responsible for creating the service instance</param>
        public virtual void AddFactory(Type serviceType, IFactory factory)
        {
            //_factories[serviceType] = factory;
            FactoryStorage.AddFactory(null, serviceType, factory);
        }

        /// <summary>
        /// Determines whether or not the container can create
        /// the given <paramref name="serviceType">service type</paramref>.
        /// </summary>
        /// <param name="serviceType">The type of service used to determine whether or not the given service can actually be created</param>
        /// <returns>A <see cref="bool">boolean</see> value that indicates whether or not the service exists.</returns>
        public virtual bool Contains(Type serviceType)
        {
            //return _factories.ContainsKey(serviceType);

            return FactoryStorage.ContainsFactory(null, serviceType);
        }

        /// <summary>
        /// Causes the container to instantiate the service with the given
        /// <paramref name="serviceType">service type</paramref>. If the service type cannot be created, then an
        /// exception will be thrown if the <see cref="SuppressErrors"/> property
        /// is set to false. Otherwise, it will simply return null.
        /// </summary>
        /// <param name="serviceType">The service type to instantiate.</param>
        /// <param name="additionalArguments">The additional arguments that will be used to instantiate the service type.</param>
        /// <returns>If successful, it will return a service instance that is compatible with the given type;
        /// otherwise, it will just return a null value.</returns>
        public virtual object GetService(Type serviceType, params object[] additionalArguments)
        {
            object result = null;
            var hasFactory = FactoryStorage.ContainsFactory(null, serviceType);
            if (!hasFactory && !SuppressErrors)
                throw new ServiceNotFoundException(serviceType);

            if (!hasFactory && SuppressErrors)
                return null;

            var factoryRequest = new FactoryRequest()
            {
                ServiceType = serviceType,
                ServiceName = null,
                Arguments = additionalArguments,
                Container = this
            };

            // Use the corresponding factory 
            // and create the service instance
            IFactory factory = FactoryStorage.GetFactory(null, serviceType);
            if (factory != null)
                result = factory.CreateInstance(factoryRequest);

            return result;
        }

        /// <summary>
        /// Gets the value indicating the <see cref="IFactoryStorage"/> instance
        /// that will be used to store each <see cref="IFactory"/> instance.
        /// </summary>
        protected IFactoryStorage FactoryStorage
        {
            get
            {
                return _factoryStorage;
            }
        }
       
        /// <summary>
        /// The list of services currently available inside the container.
        /// </summary>
        public virtual IEnumerable<IServiceInfo> AvailableServices
        {
            get
            {
                return FactoryStorage.AvailableFactories;
            }
        }
    }
}