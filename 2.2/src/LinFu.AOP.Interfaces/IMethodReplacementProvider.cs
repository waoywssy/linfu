using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.AOP.Interfaces
{
    public interface IMethodReplacementProvider
    {
        bool CanReplace(object host, IInvocationInfo info);
        IInterceptor GetMethodReplacement(object host, IInvocationInfo info);
    }
}
