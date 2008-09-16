using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LinFu.Finders;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Extensions
{
    /// <summary>
    /// Adds methods that extend LinFu.IoC to support automatic constructor resolution.
    /// </summary>
    public static class ResolutionExtensions
    {
        /// <summary>
        /// Generates a predicate that determines whether or not a specific parameter type
        /// exists in a container.
        /// </summary>
        /// <param name="parameterType">The target <see cref="Type"/>. </param>
        /// <returns>A a predicate that determines whether or not a specific type
        /// exists in a container</returns>
        public static Func<IServiceContainer, bool> MustExistInContainer(this Type parameterType)
        {
            return container => container.Contains(parameterType);
        }

        /// <summary>
        /// Generates a predicate that determines whether or not a specific type is actually
        /// a list of services that can be created from a given container.
        /// </summary>
        /// <param name="parameterType">The target <see cref="Type"/>. </param>
        /// <returns>A a predicate that determines whether or not a specific type
        /// exists as a list of services in a container</returns>
        public static Func<IServiceContainer, bool> ExistsAsServiceList(this Type parameterType)
        {
            // The type must be derived from IEnumerable<T>            
            var enumerableDefinition = typeof(IEnumerable<>);

            if (!parameterType.IsGenericType || parameterType.GetGenericTypeDefinition() != enumerableDefinition)
                return container => false;

            // Determine the individual service type
            var elementType = parameterType.GetGenericArguments()[0];
            var enumerableType = typeof(IEnumerable<>).MakeGenericType(elementType);

            // If this type isn't an IEnumerable<T> type, there's no point in testing
            // if it is a list of services that exists in the container
            if (!enumerableType.IsAssignableFrom(parameterType))
                return container => false;

            // A single service instance implies that a list of services can be created
            // from the current container
            Func<IServiceContainer, bool> hasService = container => container.Contains(elementType);

            // Check for any named services that can be included in the service list
            Func<IServiceContainer, bool> hasNamedService = container =>
                                                                {
                                                                    var matches =
                                                                        (from info in container.AvailableServices
                                                                        where info.ServiceType == elementType
                                                                        select info).Count();

                                                                    return matches > 0;
                                                                };
            
            return hasService.Or(hasNamedService);
        }
    }
}
