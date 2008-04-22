using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.DynamicProxy;

namespace LinFu.Persist
{
    public class CollectionWatcher<T> : IInvokeWrapper, ICollectionEvents<T>
    {
        private ICollection<T> _targetCollection;
        private int _oldCount;
        public CollectionWatcher(ICollection<T> targetCollection)
        {
            _targetCollection = targetCollection;
        }
        public virtual void AfterInvoke(InvocationInfo info, object returnValue)
        {
            var arguments = info.Arguments;
            var targetMethod = info.TargetMethod;
            string methodName = targetMethod.Name;

            if (targetMethod.DeclaringType != typeof(ICollection<T>))
                return;

            if (methodName == "Clear" && ItemsCleared != null)
                ItemsCleared(_targetCollection, EventArgs.Empty);

            if (arguments == null || arguments.Length != 1)
                return;

            // Cast the current item
            // to the appropriate target type
            T item = default(T);
            var argument = arguments[0];
            if (argument != null && argument is T)
                item = (T)argument;

            // Notify users that an item has been added
            var args = new EventArgs<T>() { Item = item };
            if (methodName == "Add" && ItemAdded != null)
                ItemAdded(_targetCollection, args);

            // Notify users that an item has been removed
            if (methodName == "Remove" && ItemRemoved != null)
                ItemRemoved(_targetCollection, args);

        }

        public void BeforeInvoke(InvocationInfo info)
        {
            var targetMethod = info.TargetMethod;

            if (targetMethod.DeclaringType != typeof(ICollection<T>))
                return;

            _oldCount = _targetCollection.Count;

            if (targetMethod.Name != "Clear")
                return;

            if (ItemRemoved == null)
                return;

            // HACK: Call the ItemRemoved event for every
            // item that will be removed from the collection
            foreach (var item in _targetCollection)
            {
                var args = new EventArgs<T>() { Item = item };
                ItemRemoved(_targetCollection, args);
            }
        }

        public virtual object DoInvoke(InvocationInfo info)
        {
            object target = _targetCollection;
            var targetMethod = info.TargetMethod;

            // Redirect IListEvents<T> subscriptions to this interceptor
            if (targetMethod.DeclaringType == typeof(ICollectionEvents<T>))
                target = this;

            return info.TargetMethod.Invoke(target, info.Arguments);
        }

        public event EventHandler<EventArgs<T>> ItemAdded;
        public event EventHandler<EventArgs<T>> ItemRemoved;
        public event EventHandler ItemsCleared;
    }
}
