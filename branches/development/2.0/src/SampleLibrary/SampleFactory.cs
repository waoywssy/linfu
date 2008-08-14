using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC;
using LinFu.IoC.Configuration;

namespace SampleLibrary
{
    [Factory(typeof(ISampleService))]
    public class SampleFactory : IFactory
    {
        public object CreateInstance(IContainer container)
        {
            return new SampleClass();
        }
    }
}
