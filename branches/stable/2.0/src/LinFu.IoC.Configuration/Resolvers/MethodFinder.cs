using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using LinFu.Finders;
using LinFu.Finders.Interfaces;
using LinFu.IoC.Configuration;
using LinFu.IoC.Configuration.Interfaces;
using LinFu.IoC.Interfaces;


namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// Represents a class that determines which method best matches the
    /// services currently in the target container.
    /// </summary>
    /// <typeparam name="T">The method type to search.</typeparam>
    public class MethodFinder<T> : IMethodFinder<T>
        where T : MethodBase
    {
        /// <summary>
        /// Determines which method best matches the
        /// services currently in the target container.
        /// </summary>
        /// <param name="items">The list of methods to search.</param>
        /// <param name="additionalArguments">The additional arguments that will be passed to the method.</param>
        /// <param name="container">The target <see cref="IServiceContainer"/> that will supply the method arguments.</param>
        /// <returns>Returns the method with the most resolvable parameters from the target <see cref="IServiceContainer"/> instance.</returns>
        public T GetBestMatch(IEnumerable<T> items, IEnumerable<object> additionalArguments, 
            IServiceContainer container)        
        {
            T bestMatch = null;
            var fuzzyList = items.AsFuzzyList();

            // Return the first constructor
            // if there is no other alternative
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
                Func<T, bool> isSmallerThanArgList =
                    constructor =>
                    {
                        var parameterCount = constructor.GetParameters().Count();

                        return parameterCount > additionalArgumentCount;
                    };

                fuzzyList.AddCriteria(isSmallerThanArgList, CriteriaType.Critical);

                // Match the parameter types starting from the
                // end of the parameter list
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
            foreach (var candidate in candidates)
            {
                var currentItem = candidate.Item;
                var parameters = currentItem.GetParameters();
                var parameterCount = parameters.Count();

                if (parameterCount <= bestParameterCount)
                    continue;

                bestMatch = currentItem;
                bestParameterCount = parameterCount;
            }
            
            // If all else fails, find the method
            // that matches only the additional arguments
            if (bestMatch == null)
            {
                fuzzyList.Reset();
                CheckAdditionalArguments(fuzzyList, additionalArgumentTypes);

                var nextBestMatch = fuzzyList.BestMatch();

                if (nextBestMatch != null)
                    bestMatch = nextBestMatch.Item;
            }

            return bestMatch;
        }

        /// <summary>
        /// Attempts to match the <paramref name="additionalArgumentTypes"/> against the <paramref name="fuzzyList">list of constructors</paramref>.
        /// </summary>
        /// <param name="fuzzyList">The list of items currently being compared.</param>
        /// <param name="additionalArgumentTypes">The set of <see cref="Type"/> instances that describe each supplied argument type.</param>
        private static void CheckAdditionalArguments(IList<IFuzzyItem<T>> fuzzyList,
            IEnumerable<Type> additionalArgumentTypes)
        {
            int reverseOffset = 0;
            foreach (var argumentType in additionalArgumentTypes)
            {
                int currentOffset = reverseOffset;
                var currentArgumentType = argumentType;
                Func<T, bool> hasCompatibleArgument = constructor =>
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
        private static void CheckParameters(IFuzzyItem<T> fuzzyItem,
                                            IServiceContainer container, int maxIndex)
        {
            var constructor = fuzzyItem.Item;
            var currentIndex = 0;
            foreach (var param in constructor.GetParameters())
            {
                if (currentIndex == maxIndex)
                    break;

                var parameterType = param.ParameterType;
                var criteria = new Criteria<T> { Type = CriteriaType.Critical, Weight = 1 };

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
