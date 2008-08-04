using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.IoC
{
    public interface IFactory
    {
        object CreateInstance(IContainer container);
    }
}
