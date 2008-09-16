using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Extensions.Interfaces
{
    /// <summary>
    /// A type that is responsible for determining which
    /// <see cref="ConstructorInfo">constructor</see> can be used to
    /// instantiate a service type using a given container at runtime.
    /// </summary>
    public interface IConstructorResolver
    {
        /// <summary>
        /// Uses the <paramref name="container"/> to determine which constructor can be used to instantiate
        /// a <paramref name="concreteType">concrete type</paramref>.
        /// </summary>
        /// <param name="concreteType">The target type.</param>
        /// <param name="container">The container that contains the constructor parameters that will be used to invoke the constructor.</param>
        /// <returns>A <see cref="ConstructorInfo"/> instance if a match is found; otherwise, it will return <c>null</c>.</returns>
        ConstructorInfo ResolveFrom(Type concreteType, IServiceContainer container);
    }
}
