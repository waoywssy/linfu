using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.IOC
{
    internal class InstanceFactory : IFactory
    {
        private object _instance;
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
