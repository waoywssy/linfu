using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LinFu.Finders;
using LinFu.Finders.Interfaces;
using LinFu.IoC.Configuration;
using LinFu.IoC.Extensions.Interfaces;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Extensions
{
    /// <summary>
    /// Represents the default implementation of the <see cref="IConstructorResolver"/> class.
    /// </summary>
    [Implements(typeof(IConstructorResolver), LifecycleType.OncePerRequest)]
    public class ConstructorResolver : IConstructorResolver
    {
        /// <summary>
        /// Uses the <paramref name="container"/> to determine which constructor can be used to instantiate
        /// a <paramref name="concreteType">concrete type</paramref>.
        /// </summary>
        /// <param name="concreteType">The target type.</param>
        /// <param name="container">The container that contains the constructor parameters that will be used to invoke the constructor.</param>
        /// <returns>A <see cref="ConstructorInfo"/> instance if a match is found; otherwise, it will return <c>null</c>.</returns>
        public ConstructorInfo ResolveFrom(Type concreteType, IServiceContainer container)
        {
            var constructors = concreteType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            if (constructors == null)
                return null;

            var fuzzyList = constructors.AsFuzzyList();
            foreach (var fuzzyItem in fuzzyList)
            {
                // Check the constructor for any 
                // parameter types that might not exist
                // in the container and eliminate the
                // constructor as a candidate match if
                // that parameter type cannot be found
                CheckParameters(fuzzyItem, container);
            }

            var candidates = fuzzyList.Where(fuzzy => fuzzy.Confidence > 0);

            // Since the remaining constructors all have
            // parameter types that currently exist
            // in the container as a service,
            // the best match will be the constructor with
            // the most parameters
            
            int bestParameterCount = -1;
            ConstructorInfo bestMatch = null;
            foreach (var candidate in candidates)
            {
                var constructor = candidate.Item;
                var parameters = constructor.GetParameters();
                var parameterCount = parameters.Count();

                if (parameterCount <= bestParameterCount)
                    continue;

                bestMatch = constructor;
                bestParameterCount = parameterCount;
            }

            return bestMatch;
        }

        /// <summary>
        /// Examines a <see cref="ConstructorInfo"/> instance
        /// and determines if it can be instantiated with the services embedded in
        /// the target <paramref name="container"/>.
        /// </summary>
        /// <param name="fuzzyItem">The <see cref="FuzzyItem{ConstructorInfo}"/> that represents the constructor to be examined.</param>
        /// <param name="container">The container that contains the services that will be used to instantiate the target type.</param>
        private static void CheckParameters(IFuzzyItem<ConstructorInfo> fuzzyItem,
            IServiceContainer container)
        {
            var constructor = fuzzyItem.Item;
            foreach (var param in constructor.GetParameters())
            {
                var parameterType = param.ParameterType;
                var criteria = new Criteria<ConstructorInfo>();
                criteria.Type = CriteriaType.Critical;
                criteria.Weight = 1;

                // The type must either be an existing service
                // or a list of services that can be created from the container
                var predicate = parameterType.MustExistInContainer()
                    .Or(parameterType.ExistsAsServiceList());

                criteria.Predicate = currentConstructor => predicate(container);
                fuzzyItem.Test(criteria);
            }
        }
    }
}
