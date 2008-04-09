using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist
{
    public interface IRowFinder
    {
        bool HasRows(string targetTableName, string searchColumnName, object key);
        IEnumerable<IRow> GetRows(string targetTableName, string searchColumnName, object key);
    }
}
