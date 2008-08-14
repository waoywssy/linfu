using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC;
using LinFu.IoC.Configuration;

namespace SampleLibrary
{
    [Factory(typeof(ISampleService))]
    class SampleStronglyTypedFactory : IFactory<ISampleService>
    {
        public ISampleService CreateInstance(IContainer container)
        {
            return new SampleClass();
        }
    }
}
