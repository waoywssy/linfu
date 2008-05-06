using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist
{
    public interface IRowLoader
    {
        IEnumerable<IRow> CreateRows(IEnumerable<IRowTaskItem> tasks);
    }
}
