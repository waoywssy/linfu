using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.AOP.Interfaces
{
    public interface IMethodReplacementHost
    {
        IMethodReplacementProvider MethodReplacementProvider { get; set; }
    }
}
