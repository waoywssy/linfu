using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace LinFu.IoC.Factories
{
    public class SingletonFactory<T> : BaseFactory<T>
    {
        private readonly Func<IContainer, T> _createInstance;
        public SingletonFactory(Func<IContainer, T> createInstance)
        {
            _createInstance = createInstance;
        }

        public override T CreateInstance(IContainer container)
        {
            return SingletonCache.CreateInstance(container, _createInstance);
        }       
    }
}
