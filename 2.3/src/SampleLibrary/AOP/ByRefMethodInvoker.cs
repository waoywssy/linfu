using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Interfaces;

namespace SampleLibrary.AOP
{
    public class ByRefMethodInvoker
    {
        public static bool DoInvoke(int expectedValue, IMethodReplacementProvider provider)
        {
            int argument = 0;
            var target = new SampleClassWithByRefMethod();
            var modifiedInstance = (IModifiableType) target;
            modifiedInstance.MethodReplacementProvider = provider;

            target.ByRefMethod(ref argument);

            return argument == expectedValue;
        }
    }
}
