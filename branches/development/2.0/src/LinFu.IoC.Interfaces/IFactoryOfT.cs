using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.IoC
{
    public interface IFactory<T>
    {
        T CreateInstance(IContainer container);
    }
}
