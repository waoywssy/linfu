using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.IOC
{
    public class FactoryAdapter<T> : IFactory
    {
        private readonly IFactory<T> _factory;
        public FactoryAdapter(IFactory<T> factory)
        {
            _factory = factory;
        }

        public object CreateInstance(IContainer container)
        {
            if (_factory == null)
                return default(T);

            return _factory.CreateInstance(container);
        }

    }
}
