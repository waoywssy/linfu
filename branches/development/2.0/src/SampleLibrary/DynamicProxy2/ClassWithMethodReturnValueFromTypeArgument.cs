using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleLibrary.DynamicProxy2
{
    public class ClassWithMethodReturnValueFromTypeArgument<T>
    {
        public virtual T DoSomething()
        {
            return default(T);
        }
    }
}
