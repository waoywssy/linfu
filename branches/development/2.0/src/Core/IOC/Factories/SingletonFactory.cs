using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace LinFu.IOC.Factories
{
    public class SingletonFactory<T> : IFactory<T>, IFactory
    {
        private Func<IContainer, T> _createInstance;
        public SingletonFactory(Func<IContainer, T> createInstance)
        {
            _createInstance = createInstance;
        }

        #region IFactory<T> Members

        public T CreateInstance(IContainer container)
        {
            return SingletonCache.CreateInstance(container, _createInstance);
        }

        #endregion

        #region IFactory Members

        object IFactory.CreateInstance(IContainer container)
        {
            return (T)CreateInstance(container);
        }

        #endregion
    }
}
