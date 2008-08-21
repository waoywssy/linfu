using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Extensions
{
    /// <summary>
    /// Represents the <c>internal</c> context class that will be used to 
    /// incrementally build enough information to inject a specific
    /// <see cref="IFactory{T}"/> instance into a container.
    /// </summary>
    /// <typeparam name="TService">The service type to be created.</typeparam>
    internal class InjectionContext<TService>
    {
        /// <summary>
        /// The service type to be created.
        /// </summary>
        public Type ServiceType 
        { 
            get
            {
                return typeof (TService);
            }
        }

        /// <summary>
        /// The name of the service to be created.
        /// </summary>
        public string ServiceName
        {
            get; set;
        }

        /// <summary>
        /// The actual <see cref="IServiceContainer"/>
        /// that ultimately will hold the service instance.
        /// </summary>
        public IServiceContainer Container
        {
            get; set;
        }

        /// <summary>
        /// The factory method that will be used to
        /// instantiate the actual <typeparamref name="TService"/>
        /// instance.
        /// </summary>
        public Func<Type, IServiceContainer, TService> FactoryMethod
        {
            get; set;
        }
    }
}
