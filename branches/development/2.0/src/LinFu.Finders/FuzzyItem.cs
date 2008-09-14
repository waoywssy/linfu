using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.Finders.Interfaces;

namespace LinFu.Finders
{
    /// <summary>
    /// Represents the default implementation of a weighted item in
    /// a fuzzy list.
    /// </summary>
    /// <typeparam name="T">The item type to be tested.</typeparam>
    public class FuzzyItem<T> : IFuzzyItem<T>
    {
        private readonly T _item;
        private int _testCount;
        private int _matches;

        /// <summary>
        /// Initializes the <see cref="FuzzyItem{T}"/> class with the given <paramref name="item"/>.
        /// </summary>
        /// <param name="item">An instance of the <typeparamref name="T">item type</typeparamref> that will be tested.</param>
        public FuzzyItem(T item)
        {
            _item = item;
        }

        /// <summary>
        /// Reports the probability of a match
        /// based on the <see cref="ICriteria{T}"/>
        /// that has been tested so far. 
        /// 
        /// A value of 1.0 indicates a 100% match;
        /// A value of 0.0 equals a zero percent match.
        /// </summary>
        public double Confidence
        {
            get
            {
                double result = 0;
                if (_testCount == 0)
                    return 0;

                result = ((double)_matches) / ((double)_testCount);

                return result;
            }
        }

        /// <summary>
        /// Gets the target item.
        /// </summary>
        public T Item
        {
            get { return _item; }
        }

        /// <summary>
        /// Tests if the current item matches the given
        /// <paramref name="criteria"/>.
        /// </summary>
        /// <param name="criteria">The <see cref="ICriteria{T}"/> that determines whether or not the <see cref="Item"/> meets a particular description.</param>
        public void Test(ICriteria<T> criteria)
        {
            var predicate = criteria.Predicate;
            if (predicate == null)
                return;

            // Determine the weight multiplier of this test
            var weight = criteria.Weight;
            var result = predicate(_item);
            
            if (result)
                _matches += weight;

            // Don't count the result if the criteria
            // is optional
            if (result != true && criteria.IsOptional)
                return;

            _testCount += weight;
        }

        /// <summary>
        /// Resets the item back to its initial state.
        /// </summary>
        public void Reset()
        {
            _testCount = 0;
            _matches = 0;
        }
    }
}
