using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;

namespace SampleLibrary
{
    [Factory(typeof (ISampleService))]
    public class SampleStronglyTypedFactory : IFactory<ISampleService>
    {
        #region IFactory<ISampleService> Members

        public ISampleService CreateInstance(IContainer container, params object[] additionalArguments)
        {
            return new SampleClass();
        }

        #endregion
    }
}