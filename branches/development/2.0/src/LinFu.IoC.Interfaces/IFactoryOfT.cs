using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.IoC
{
    /// <summary>
    /// A strongly-typed version of <see cref="IFactory"/>. Allows users
    /// to create their own service instances
    /// </summary>
    /// <typeparam name="T">The instance type that can be created by this factory.</typeparam>
    public interface IFactory<T>
    {
        /// <summary>
        /// Creates a service instance using the given <paramref name="container"/>.
        /// </summary>
        /// <param name="container">The container that will ultimately hold the given service instance</param>
        /// <returns>An object instance that represents the service to be created. This cannot be <c>null</c>.</returns>
        T CreateInstance(IContainer container);
    }
}
