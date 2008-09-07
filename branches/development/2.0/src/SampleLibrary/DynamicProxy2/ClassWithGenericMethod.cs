using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleLibrary.DynamicProxy2
{
    public class ClassWithGenericMethod
    {
        public virtual void DoSomething<T>()
        {
            throw new NotImplementedException();
        }
    }
}
