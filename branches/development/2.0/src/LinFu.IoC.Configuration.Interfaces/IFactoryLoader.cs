using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.IoC.Configuration.Interfaces
{
    /// <summary>
    /// Generates one or more <see cref="IFactory"/> instances
    /// from a given source type so that it can be used
    /// by an <see cref="IContainer"/> instance.
    /// </summary>
    public interface IFactoryLoader
    {
        /// <summary>
        /// Converts a given <see cref="System.Type"/> into
        /// a set of <see cref="Action{IServiceContainer}"/> instances so that
        /// the <see cref="IContainer"/> instance can be loaded
        /// with the given factories.
        /// </summary>
        /// <param name="sourceType">The input type from which one or more factories will be created.</param>
        /// <returns>A set of <see cref="Action{IServiceContainer}"/> instances. This cannot be null.</returns>
        IEnumerable<Action<IServiceContainer>> LoadFactoriesFrom(Type sourceType);
    }
}
