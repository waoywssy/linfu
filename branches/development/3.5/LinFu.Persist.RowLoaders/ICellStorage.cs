using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist
{
    public interface ICellStorage
    {
        bool Contains(ICellStorageKey key);
        void Store(ICellStorageKey key, ICell cell);
        ICell Retrieve(ICellStorageKey key);
    }
}
