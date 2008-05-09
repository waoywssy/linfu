using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist
{
    public interface IRowLoadStrategyProvider
    {
        IRowLoadStrategy GetStrategy(string tableName, long rowCount, IEnumerable<string> keyColumns);
        Int64 BulkLoadThreshold { get; set; }
    }
}
