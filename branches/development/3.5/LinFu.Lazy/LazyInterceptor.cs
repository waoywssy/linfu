using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.DynamicProxy;
using System.Reflection;

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
            {
                object result = null;
                try
                {
                    result = info.TargetMethod.Invoke(_actualObject, info.Arguments);
                }
                catch (TargetInvocationException ex)
                {
                    var inner = ex.InnerException;
                    while (inner != null && inner is TargetInvocationException)
                    {
                        inner = inner.InnerException;
                    }

                    throw ex.InnerException;
                }
            }
            throw new NotImplementedException();
        }
    }
}
