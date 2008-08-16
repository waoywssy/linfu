using System;

namespace LinFu.IoC
{
    /// <summary>
    /// Allows an object to create its own service instances.
    /// </summary>
    public interface IFactory
    {
        /// <summary>
        /// Creates a service instance using the given <paramref name="container"/>.
        /// </summary>
        /// <param name="container">The container that will ultimately hold the given service instance</param>
        /// <param name="serviceType">The type of service to create.</param>
        /// <returns>An object instance that represents the service to be created. This cannot be <c>null</c>.</returns>
        object CreateInstance(Type serviceType, IContainer container);
    }
}