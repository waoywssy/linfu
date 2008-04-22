using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist
{
    public class DictionaryEventArgs<TKey, TItem> : EventArgs
    {
        public TItem Item { get; internal set; }
        public TKey Key { get; internal set; }
    }
}
