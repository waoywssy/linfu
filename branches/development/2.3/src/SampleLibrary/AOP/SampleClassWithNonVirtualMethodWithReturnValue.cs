using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleLibrary.AOP
{
    public sealed class SampleClassWithNonVirtualMethodWithReturnValue
    {
        public int DoSomething(int a, int b)
        {
            return a + b;
        }
    }
}
