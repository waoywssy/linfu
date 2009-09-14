using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Interfaces;

namespace SampleLibrary.AOP
{
    public class SampleAroundInvokeProvider : IAroundInvokeProvider
    {
        public bool GetSurroundingImplementationWasCalled
        {
            get; private set;
        }

        public IAroundInvoke GetSurroundingImplementation(IInvocationInfo context)
        {
            GetSurroundingImplementationWasCalled = true;
            return null;
        }
    }
}
