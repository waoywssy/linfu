using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist
{
    public interface ICollectionEvents<T>
    {
        event EventHandler<EventArgs<T>> ItemAdded;
        event EventHandler<EventArgs<T>> ItemRemoved;
        event EventHandler ItemsCleared;
    }
}
