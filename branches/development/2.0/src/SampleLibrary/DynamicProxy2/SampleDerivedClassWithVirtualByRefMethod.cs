using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleLibrary.DynamicProxy2
{
    public class SampleDerivedClassWithVirtualByRefMethod : SampleClassWithVirtualByRefMethod
    {
        public override void ByRefMethod(ref int a)
        {
            a = 54321;
        }
    }
}
