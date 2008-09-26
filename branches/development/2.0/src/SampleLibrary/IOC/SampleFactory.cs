using System;
using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;

namespace SampleLibrary
{
    [Factory(typeof (ISampleService))]
    public class SampleFactory : IFactory
    {
        #region IFactory Members

        public object CreateInstance(Type serviceType, IContainer container, params object[] additionalArguments)
        {
            return new SampleClass();
        }

        #endregion
    }
}