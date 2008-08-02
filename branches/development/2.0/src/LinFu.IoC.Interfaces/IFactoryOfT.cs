using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.IOC
{
    public interface IFactory<T>
    {
        T CreateInstance(IContainer container);
    }
}
