using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.Reflection;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// Represents service classes that need to be initialized
    /// every time a particular <see cref="IServiceContainer"/>
    /// instance creates that type.
    /// </summary>
    public interface IInitialize : IInitialize<IServiceContainer>
    {
        
    }
}
