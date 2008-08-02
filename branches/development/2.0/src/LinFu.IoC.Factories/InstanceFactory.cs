using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IOC;

namespace LinFu.IoC
{
    public class InstanceFactory : IFactory
    {
        private readonly object _instance;
        public InstanceFactory(object instance)
        {
            _instance = instance;
        }

        #region IFactory Members

        public object CreateInstance(IContainer container)
        {
            return _instance;
        }

        #endregion
    }
}
