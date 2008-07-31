using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.IOC.Factories
{
    public class OncePerRequestFactory<T> : IFactory<T>, IFactory
    {
        private Func<IContainer, T> _createInstance;
        public OncePerRequestFactory(Func<IContainer, T> createInstance)
        {
            _createInstance = createInstance;
        }

        #region IFactory<T> Members

        public T CreateInstance(IContainer container)
        {
            return _createInstance(container);
        }

        #endregion

        #region IFactory Members

        object IFactory.CreateInstance(IContainer container)
        {
            return CreateInstance(container);
        }

        #endregion
    }
}
