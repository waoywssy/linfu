using LinFu.IoC;
using LinFu.IoC.Configuration;

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