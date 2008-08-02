using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.IOC.Factories
{
    public class OncePerRequestFactory<T> : BaseFactory<T>
    {
        private readonly Func<IContainer, T> _createInstance;
        public OncePerRequestFactory(Func<IContainer, T> createInstance)
        {
            _createInstance = createInstance;
        }

        public override T CreateInstance(IContainer container)
        {
            return _createInstance(container);
        }        
    }
}
