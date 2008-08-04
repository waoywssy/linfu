using System;

namespace LinFu.IoC
{
    /// <summary>
    /// An inversion of control container interface.
    /// </summary>
    public interface IContainer 
    {
        /// <summary>
        /// Determines whether or not a container will throw an exception
        /// if the requested service is not found.
        /// </summary>
        bool SuppressErrors { get; set; }
        
        /// <summary>
        /// Adds an <see cref="IFactory"/> instance and associates it
        /// with the given <paramref name="serviceType">service type</paramref>.
        /// </summary>
        /// <param name="serviceType">The service type to associate with the factory</param>
        /// <param name="factory">The <see cref="IFactory"/> instance that will be responsible for creating the service instance</param>
        void AddFactory(Type serviceType, IFactory factory);

        /// <summary>
        /// Specifies whether or not the container can create
        /// the given <paramref name="serviceType">service type</paramref>.
        /// </summary>
        /// <param name="serviceType">The type of service used to determine whether or not the given service can actually be created</param>
        /// <returns>A <see cref="bool">boolean</see> value that indicates whether or not the service exists.</returns>
        bool Contains(Type serviceType);

        /// <summary>
        /// Causes the container to instantiate the service with the given
        /// <paramref name="serviceType">service type</paramref>. If the service type cannot be created, then an
        /// exception will be thrown if the <see cref="SuppressErrors"/> property
        /// is set to false. Otherwise, it will simply return null.
        /// </summary>
        /// <param name="serviceType">The service type to instantiate.</param>
        /// <returns>If successful, it will return a service instance that is compatible with the given type;
        /// otherwise, it will just return a null value.</returns>
        object GetService(Type serviceType);
    }
}