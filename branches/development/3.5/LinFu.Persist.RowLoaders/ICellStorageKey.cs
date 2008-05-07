using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist
{
    public interface ICellStorageKey
    {
        string TableName { get; set; }
        string ColumnName { get; set; }

        ICellStorage CellStorage { get; set; }
        IEnumerable<KeyValuePair<string, object>> PrimaryKeys { get; }
    }
}
