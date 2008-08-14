using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC;
using LinFu.IoC.Configuration;

namespace SampleLibrary
{
    [Factory(typeof(ISampleGenericService<>))]
    public class SampleOpenGenericFactory : IFactory
    {
        public object CreateInstance(Type serviceType, IContainer container)
        {
            var typeArgument = serviceType.GetGenericArguments()[0];
            return typeof (SampleGenericImplementation<>).MakeGenericType(typeArgument);
        }
    }
}
