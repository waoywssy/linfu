using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleLibrary.AOP
{
    public sealed class SampleClassWithNonVirtualMethodThatThrowsExceptions
    {
        public void DoSomething()
        {
            throw new NotImplementedException();
        }
    }
}
