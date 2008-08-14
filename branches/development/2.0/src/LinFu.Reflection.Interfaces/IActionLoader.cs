using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Reflection
{
    /// <summary>
    /// Represents a class that can configure 
    /// a target of type <typeparamref name="TTarget"/>
    /// using an input type of <typeparamref name="TInput"/>.
    /// </summary>
    /// <typeparam name="TTarget">The target type to configure.</typeparam>
    /// <typeparam name="TInput">The input that will be used to configure the target.</typeparam>
    public interface IActionLoader<TTarget, TInput>
    {
        /// <summary>
        /// Determines if the current loader
        /// can configure the target using
        /// the current input.
        /// </summary>
        /// <param name="input">The input that will be used to configure the target.</param>
        /// <returns>A set of <see cref="Action{TTarget}"/> instances. This cannot be <c>null</c>.</returns>
        bool CanLoad(TInput input);
        
        /// <summary>
        /// Generates a set of <see cref="Action{TTarget}"/> instances
        /// using the given <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The input that will be used to configure the target.</param>
        /// <returns>A set of <see cref="Action{TTarget}"/> instances. This cannot be <c>null</c>.</returns>
        IEnumerable<Action<TTarget>> Load(TInput input);
    }
}
