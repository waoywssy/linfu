using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist
{
    public static partial class ListExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            if (action == null)
                return;

            foreach (T item in list)
            {
                action(item);
            }
        }
    }
}
