using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist
{
    public interface IRowIndexer
    {
        IDictionary<object, IList<IRow>> IndexByColumn(string columnName, ITable targetTable);
    }
}
