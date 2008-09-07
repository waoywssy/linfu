using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleLibrary.DynamicProxy2
{
    public class ClassWithVirtualMethodWithOutParameter
    {
        public virtual void DoSomething(out int a)
        {
            a = 12345;
        }
    }
}
