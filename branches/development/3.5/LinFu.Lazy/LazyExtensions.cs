using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Lazy
{
    public static class LazyExtensions
    {
        public static T QueueLazyAction<T>(this T target, Action<T> action)
            where T : class
        {
            Func<T> createItem = () =>
            {
                // Perform the action, then return
                // the target in its modified state
                return target;
            };

            return LazyBuilder.CreateLazyItem<T>(createItem);
        }
    }
}
