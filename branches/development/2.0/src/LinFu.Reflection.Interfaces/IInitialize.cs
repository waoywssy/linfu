using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Reflection
{
    /// <summary>
    /// Represents classes that need to be initialized
    /// every time a particular 
    /// instance creates that type.
    /// </summary>
    public interface IInitialize<T>
    {
        /// <summary>
        /// Initializes the target with the
        /// particular <typeparamref cref="T"/> instance.
        /// </summary>
        /// <param name="source">The <typeparamref cref="T"/> instance that will hold the target type.</param>
        void Initialize(T source);
    }
}
