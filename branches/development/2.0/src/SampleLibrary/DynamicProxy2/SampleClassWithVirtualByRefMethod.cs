using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleLibrary.DynamicProxy2
{
    public abstract class SampleClassWithVirtualByRefMethod
    {
        public abstract void ByRefMethod(ref int a);        
    }
}
