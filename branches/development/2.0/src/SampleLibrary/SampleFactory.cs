using System;
using LinFu.IoC;
using LinFu.IoC.Configuration;

namespace SampleLibrary
{
    [Factory(typeof (ISampleService))]
    public class SampleFactory : IFactory
    {
        #region IFactory Members

        public object CreateInstance(Type serviceType, IContainer container)
        {
            return new SampleClass();
        }

        #endregion
    }
}