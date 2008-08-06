using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Configuration.Interfaces;

namespace LinFu.IoC.Configuration
{
    /// <summary>
    /// A factory converter that scans a type for <see cref="ImplementsAttribute"/>
    /// attribute declarations and creates a factory for each corresponding 
    /// attribute instance.
    /// </summary>
    /// <seealso cref="IFactory"/>
    public class ImplementsAttributeConverter : IFactoryConverter
    {
        public IEnumerable<IFactory> CreateFactoriesFrom(Type sourceType)
        {
            throw new NotImplementedException();
        }
    }
}
