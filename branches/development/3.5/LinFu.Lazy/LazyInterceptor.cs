using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.DynamicProxy;

namespace LinFu.Lazy
{
    public class LazyInterceptor<T> : IInterceptor
        where T : class
    {
        private Func<T> _getItem;
        private T _actualObject;
        public LazyInterceptor(Func<T> getItem)
        {
            _getItem = getItem;
        }
        public object Intercept(InvocationInfo info)
        {
            if (_actualObject == null)
                _actualObject = _getItem();

            if (_actualObject != null)
                return info.TargetMethod.Invoke(_actualObject, info.Arguments);

            throw new NotImplementedException();
        }
    }
}
