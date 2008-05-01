using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist
{
    public class EventArgs<T> : EventArgs
    {
        public EventArgs()
        {
        }
        public EventArgs(T item)
        {
            Item = item;
        }
        public virtual T Item
        {
            get;
            internal set;
        }
    }
}
