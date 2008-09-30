using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Interfaces;
using LinFu.IoC.Interceptors;

namespace SampleLibrary.IOC
{
    [Intercepts(typeof(ISampleInterceptedInterface))]
    public class SampleInterceptorClass : IInterceptor
    {
        #region IInterceptor Members

        public object Intercept(IInvocationInfo info)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
