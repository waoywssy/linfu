using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LinFu.Finders;
using LinFu.Finders.Interfaces;
using LinFu.IoC.Configuration.Interfaces;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// Represents the default implementation of the <see cref="IConstructorResolver"/> class.
    /// </summary>
    public class ConstructorResolver : IConstructorResolver
    {
        /// <summary>
        /// Uses the <paramref name="container"/> to determine which constructor can be used to instantiate
        /// a <paramref name="concreteType">concrete type</paramref>.
        /// </summary>
        /// <param name="concreteType">The target type.</param>
        /// <param name="container">The container that contains the constructor parameters that will be used to invoke the constructor.</param>
        /// <param name="additionalArguments">The additional arguments that will be used to evaluate the best constructor to use to instantiate the target type.</param>
        /// <returns>A <see cref="ConstructorInfo"/> instance if a match is found; otherwise, it will return <c>null</c>.</returns>
        public ConstructorInfo ResolveFrom(Type concreteType, IServiceContainer container, 
            params object[] additionalArguments)
        {
            var constructors = concreteType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            if (constructors == null)
                return null;

            var fuzzyList = constructors.AsFuzzyList();

            // Return the first constructor
            // if there are no other alternatives
            if (fuzzyList.Count == 1)
                return fuzzyList[0].Item;

            var additionalArgumentTypes = (from argument in additionalArguments
                                          let argumentType = argument == null ? typeof(object) : argument.GetType()
                                          select argumentType).ToList();

            int additionalArgumentCount = additionalArgumentTypes.Count;
            if (additionalArgumentCount > 0)
            {
                // Eliminate constructors with parameterCounts
                // that are shorter than the additional argument list
                Func<ConstructorInfo, bool> isSmallerThanArgList =
                constructor =>
                    {
                        var parameterCount = constructor.GetParameters().Count();

                        return parameterCount > additionalArgumentCount;
                    };

                fuzzyList.AddCriteria(isSmallerThanArgList, CriteriaType.Critical);


                CheckAdditionalArguments(fuzzyList, additionalArgumentTypes);
            }

            foreach (var fuzzyItem in fuzzyList)
            {
                if (fuzzyItem.Confidence < 0)
                    continue;

                // Check the constructor for any 
                // parameter types that might not exist
                // in the container and eliminate the
                // constructor as a candidate match if
                // that parameter type cannot be found
                var constructor = fuzzyItem.Item;
                var parameters = constructor.GetParameters();
                var parameterCount = parameters.Length;
                var maxRelativeIndex = parameterCount - additionalArgumentCount;

                CheckParameters(fuzzyItem, container, maxRelativeIndex);
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

            // If all else fails, find the
            // default constructor and use it as the
            // best match by default
            if (bestMatch == null)
            {
                var defaultConstructor = concreteType.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null,
                                                                     new Type[0], null);

                bestMatch = defaultConstructor;
            }

            return bestMatch;
        }
        
        /// <summary>
        /// Attempts to match the <paramref name="additionalArgumentTypes"/> against the <paramref name="fuzzyList">list of constructors</paramref>.
        /// </summary>
        /// <param name="fuzzyList">The list of items currently being compared.</param>
        /// <param name="additionalArgumentTypes">The set of <see cref="Type"/> instances that describe each supplied argument type.</param>
        private static void CheckAdditionalArguments(IList<IFuzzyItem<ConstructorInfo>> fuzzyList, 
            IEnumerable<Type> additionalArgumentTypes)
        {
            int reverseOffset = 0;
            foreach(var argumentType in additionalArgumentTypes)
            {
                int currentOffset = reverseOffset;
                var currentArgumentType = argumentType;
                Func<ConstructorInfo, bool> hasCompatibleArgument = constructor=>
                                                                        {
                                                                            var parameters = constructor.GetParameters();
                                                                            var parameterCount = parameters.Length;
                                                                            var targetPosition = parameterCount - currentOffset;
                                                                            var targetParameterIndex = targetPosition - 1;

                                                                            // Make sure that the index is valid
                                                                            if (targetParameterIndex < 0 || targetParameterIndex >= parameterCount)
                                                                                return false;

                                                                            // The parameter type must be compatible with the
                                                                            // given argument type
                                                                            var parameterType = parameters[targetParameterIndex].ParameterType;
                                                                            return parameterType.IsAssignableFrom(currentArgumentType);
                                                                        };

                // Match each additional argument type to its
                // relative position from the end of the parameter
                // list
                fuzzyList.AddCriteria(hasCompatibleArgument, CriteriaType.Critical);
                reverseOffset++;
            }
        }

        /// <summary>
        /// Examines a <see cref="ConstructorInfo"/> instance
        /// and determines if it can be instantiated with the services embedded in
        /// the target <paramref name="container"/>.
        /// </summary>
        /// <param name="fuzzyItem">The <see cref="FuzzyItem{T}"/> that represents the constructor to be examined.</param>
        /// <param name="container">The container that contains the services that will be used to instantiate the target type.</param>
        /// <param name="maxIndex">Indicates the index that 
        /// marks the point where the user-supplied arguments begin.</param>
        private static void CheckParameters(IFuzzyItem<ConstructorInfo> fuzzyItem,
                                            IServiceContainer container, int maxIndex)
        {
            var constructor = fuzzyItem.Item;
            var currentIndex = 0;
            foreach (var param in constructor.GetParameters())
            {
                if (currentIndex == maxIndex)
                    break;

                var parameterType = param.ParameterType;
                var criteria = new Criteria<ConstructorInfo> { Type = CriteriaType.Critical, Weight = 1 };

                // The type must either be an existing service
                // or a list of services that can be created from the container
                var predicate = parameterType.MustExistInContainer()
                    .Or(parameterType.ExistsAsServiceArray())
                    .Or(parameterType.ExistsAsEnumerableSetOfServices());

                criteria.Predicate = currentConstructor => predicate(container);
                fuzzyItem.Test(criteria);

                currentIndex++;
            }
        }
    }
}