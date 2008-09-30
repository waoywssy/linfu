using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LinFu.IoC.Configuration.Interfaces;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// Represents a class that can choose a member that best matches
    /// the services currently available in a given <see cref="IServiceContainer"/> instance.
    /// </summary>
    /// <typeparam name="TMember">The member type that will be searched.</typeparam>
    public abstract class MemberResolver<TMember> : IMemberResolver<TMember>
        where TMember : MethodBase
    {
        /// <summary>
        /// Uses the <paramref name="container"/> to determine which member to use from
        /// the <paramref name="concreteType">concrete type</paramref>.
        /// </summary>
        /// <param name="concreteType">The target type.</param>
        /// <param name="container">The container that contains the member values that will be used to invoke the members.</param>
        /// <param name="additionalArguments">The additional arguments that will be used to evaluate the best match to use to invoke the target member.</param>
        /// <returns>A member instance if a match is found; otherwise, it will return <c>null</c>.</returns>
        public TMember ResolveFrom(Type concreteType, IServiceContainer container,
            params object[] additionalArguments)
        {
            var constructors = GetMembers(concreteType);
            if (constructors == null)
                return null;

            var resolver = container.GetService<IMethodFinder<TMember>>();
            TMember bestMatch = resolver.GetBestMatch(constructors, additionalArguments, container);

            // If all else fails, find the
            // default constructor and use it as the
            // best match by default
            if (bestMatch == null)
            {
                var defaultResult = GetDefaultResult(concreteType);

                bestMatch = defaultResult;
            }

            return bestMatch;
        }

        /// <summary>
        /// The method used to retrieve the default result if no
        /// other alternative is found.
        /// </summary>
        /// <param name="concreteType">The target type that contains the default member.</param>
        /// <returns>The default member result.</returns>
        protected abstract TMember GetDefaultResult(Type concreteType);

        /// <summary>
        /// Lists the members associated with the <paramref name="concreteType"/>.
        /// </summary>
        /// <param name="concreteType">The target type that contains the type members.</param> 
        /// <returns>A list of members that belong to the concrete type.</returns>
        protected abstract IEnumerable<TMember> GetMembers(Type concreteType);
    }
}
