using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.DynamicProxy;

namespace LinFu.Persist
{
    public class DictionaryWatcher<TKey, TItem> : IInvokeWrapper, IDictionaryEvents<TKey, TItem>
    {
        private IDictionary<TKey, TItem> _target;
        private int _oldCount;
        public DictionaryWatcher(IDictionary<TKey, TItem> target)
        {
            _target = target;
        }

        public void AfterInvoke(InvocationInfo info, object returnValue)
        {
            var targetMethod = info.TargetMethod;
            var declaringType = targetMethod.DeclaringType;
            var arguments = info.Arguments;
            int argumentCount = arguments == null ? 0 : arguments.Length;

            string methodName = targetMethod.Name;

            #region Handle the Clear() method
            if (methodName == "Clear" && declaringType == typeof(ICollection<KeyValuePair<TKey, TItem>>) 
                && ItemsCleared != null)
                ItemsCleared(_target, EventArgs.Empty);
            #endregion

            if (declaringType != typeof(IDictionary<TKey, TItem>))
                return;

            if (argumentCount < 1)
                return;
            #region Handle the Remove method
            TKey key = (TKey)arguments[0];
            if (methodName == "Remove" && ItemRemoved != null)
            {
                ItemRemoved(_target, new DictionaryEventArgs<TKey, TItem>()
                {
                    Key = key,
                    Item = default(TItem)
                });
            }
            #endregion

            if (argumentCount != 2)
                return;

            TItem item = (TItem)arguments[1];
            int currentCount = _target.Count;
            var args = new DictionaryEventArgs<TKey, TItem>() { Key = key, Item = item };

            if ((methodName == "Add" || methodName == "set_Item") &&
                currentCount > _oldCount && ItemAdded != null)
                ItemAdded(_target, args);

            if (methodName == "set_Item" && currentCount == _oldCount && ItemModified != null)
                ItemModified(_target, args);
        }

        public void BeforeInvoke(InvocationInfo info)
        {
            // Keep track of the number of items in the dictionary
            // to distinguish between whether or not the dictionary items
            // have been modified or added
            _oldCount = _target.Count;
        }

        public object DoInvoke(InvocationInfo info)
        {
            var targetMethod = info.TargetMethod;

            object target = _target;
            if (targetMethod.DeclaringType == typeof(IDictionaryEvents<TKey, TItem>))
            {
                target = this;
                return targetMethod.Invoke(target, info.Arguments);
            }

            return targetMethod.Invoke(_target, info.Arguments);
        }

        public event EventHandler<DictionaryEventArgs<TKey, TItem>> ItemAdded;
        public event EventHandler<DictionaryEventArgs<TKey, TItem>> ItemRemoved;        
        public event EventHandler<DictionaryEventArgs<TKey, TItem>> ItemModified;
        public event EventHandler ItemsCleared;
    }
}
