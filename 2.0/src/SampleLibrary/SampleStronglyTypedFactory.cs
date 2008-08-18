using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;

namespace SampleLibrary
{
    [Factory(typeof (ISampleService))]
    internal class SampleStronglyTypedFactory : IFactory<ISampleService>
    {
        #region IFactory<ISampleService> Members

        public ISampleService CreateInstance(IContainer container)
        {
            return new SampleClass();
        }

        #endregion
    }
}