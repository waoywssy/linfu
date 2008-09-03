using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.Finders.Interfaces;

namespace LinFu.Finders
{
    /// <summary>
    /// A class that adds fuzzy search support to <see cref="IList{T}"/> instances.
    /// </summary>
    public static class FinderExtensions
    {
        /// <summary>
        /// Applies a criteria to the <paramref name="list"/> of 
        /// fuzzy items.
        /// </summary>
        /// <typeparam name="TItem">The type of item to test.</typeparam>
        /// <param name="list">The list of <see cref="IFuzzyItem{T}"/> instances that represent a single test case in a fuzzy search.</param>
        /// <param name="criteria">The criteria to test against each item in the list.</param>
        public static void AddCriteria<TItem>(this IList<IFuzzyItem<TItem>> list, ICriteria<TItem> criteria)
        {
            foreach(var item in list)
            {
                if (item == null)
                    continue;

                item.Test(criteria);
            }
        }
        /// <summary>
        /// Returns the FuzzyItem with the highest confidence score in a given
        /// <see cref="IFuzzyItem{T}"/> list.
        /// </summary>
        /// <typeparam name="TItem">The type of item being compared.</typeparam>
        /// <param name="list">The fuzzy list that contains the list of possible matches.</param>
        /// <returns>The item with the highest match.</returns>
        public static IFuzzyItem<TItem> BestMatch<TItem>(this IList<IFuzzyItem<TItem>> list)
        {
            double bestScore = 0;
            IFuzzyItem<TItem> bestMatch = null;
            foreach(var item in list)
            {
                if (item.Confidence <= bestScore)
                    continue;

                bestMatch = item;
                bestScore = item.Confidence;
            }

            return bestMatch;
        }
    }
}
