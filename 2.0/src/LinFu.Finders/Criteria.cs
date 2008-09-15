using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.Finders.Interfaces;

namespace LinFu.Finders
{
    /// <summary>
    /// Represents the default implementation of the <see cref="ICriteria{T}"/> interface.
    /// </summary>
    /// <typeparam name="T">The type of item to test.</typeparam>
    public class Criteria<T> : ICriteria<T>
    {
        /// <summary>
        /// Determines whether or not a failed <see cref="Predicate"/>
        /// match should be counted against the the given <typeparamref name="T">target type</typeparamref>.
        /// </summary>
        public bool IsOptional
        {
            get; set;
        }

        /// <summary>
        /// The condition that will determine whether or not
        /// the target item matches the criteria.
        /// </summary>
        public Func<T, bool> Predicate
        {
            get; set;
        }

        /// <summary>
        /// The weight of the given <see cref="Predicate"/>.
        /// </summary>
        public int Weight
        {
            get; set;
        }
    }
}
