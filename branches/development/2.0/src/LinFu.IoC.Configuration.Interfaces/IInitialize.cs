using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// Represents service classes that need to be initialized
    /// every time a particular <see cref="IServiceContainer"/>
    /// instance creates that type.
    /// </summary>
    public interface IInitialize
    {
        /// <summary>
        /// Initializes the target with the
        /// particular <paramref name="container"/> instance.
        /// </summary>
        /// <param name="container">The <see cref="IServiceContainer"/> instance that will hold the target type.</param>
        void Initialize(IServiceContainer container);
    }
}
