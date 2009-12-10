using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Interfaces;

namespace SampleLibrary.AOP
{
    public class SampleInterceptor : IInterceptor
    {
        public bool WasInvoked
        {
            get; set;
        }

        public object Intercept(IInvocationInfo info)
        {
            WasInvoked = true;
            return null;
        }
    }
}
