﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using LinFu.Finders;
using LinFu.Finders.Interfaces;
using LinFu.IoC.Configuration;
using LinFu.IoC.Configuration.Interfaces;
using LinFu.IoC.Interfaces;

namespace LinFu.IoC.Configuration.Resolvers
{
    /// <summary>
    /// A <see cref="MethodFinder{TMethod}"/> type that uses a <see cref="IServiceContainer"/>
    /// instance to find a method with the most resolvable parameters.
    /// </summary>
    /// <typeparam name="TMethod">The method type that will be searched.</typeparam>
    public class MethodFinderFromContainer<TMethod> : MethodFinder<TMethod>, IInitialize
        where TMethod : MethodBase
    {
        private IServiceContainer _container;

        /// <summary>
        /// Examines a <see cref="ConstructorInfo"/> instance
        /// and determines if it can be instantiated with the services embedded in
        /// the target <paramref name="container"/>.
        /// </summary>
        /// <param name="fuzzyItem">The <see cref="FuzzyItem{T}"/> that represents the constructor to be examined.</param>
        /// <param name="container">The container that contains the services that will be used to instantiate the target type.</param>
        /// <param name="maxIndex">Indicates the index that 
        /// marks the point where the user-supplied arguments begin.</param>
        private static void CheckParameters(IFuzzyItem<TMethod> fuzzyItem,
                                            IServiceContainer container, int maxIndex)
        {
            var constructor = fuzzyItem.Item;
            var currentIndex = 0;
            foreach (var param in constructor.GetParameters())
            {
                if (currentIndex == maxIndex)
                    break;

                var parameterType = param.ParameterType;
                var criteria = new Criteria<TMethod> { Type = CriteriaType.Critical, Weight = 1 };

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

        /// <summary>
        /// Adds additional <see cref="ICriteria{T}"/> to the fuzzy search list.
        /// </summary>
        /// <param name="methods">The list of methods to rank.</param>
        /// <param name="argumentTypes">The list of <see cref="Type"/> objects that describe the arguments passed to the method.</param>
        protected override void Rank(IList<IFuzzyItem<TMethod>> methods, IList<Type> argumentTypes)
        {
            int argumentCount = argumentTypes.Count;
            foreach (var fuzzyItem in methods)
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
                var maxRelativeIndex = parameterCount - argumentCount;

                CheckParameters(fuzzyItem, _container, maxRelativeIndex);
            }
        }
        
        public void Initialize(IServiceContainer container)
        {
            _container = container;
        }
    }
}
