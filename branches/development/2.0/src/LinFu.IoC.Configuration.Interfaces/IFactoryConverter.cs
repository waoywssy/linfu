using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.IoC.Configuration.Interfaces
{
    public interface IFactoryConverter
    {
        IEnumerable<IFactory> CreateFactoriesFrom(Type sourceType);
    }
}
