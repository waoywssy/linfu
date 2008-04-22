using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist
{
    public interface IDictionaryEvents<TKey, TItem>
    {
        event EventHandler<DictionaryEventArgs<TKey, TItem>> ItemAdded;
        event EventHandler<DictionaryEventArgs<TKey, TItem>> ItemModified;
        event EventHandler<DictionaryEventArgs<TKey, TItem>> ItemRemoved;
        event EventHandler ItemsCleared;
    }
}
