using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist
{
    public interface IRowTaskItem
    {
        string TableName { get; set; }
        IEnumerable<string> Columns { get; }
        IEnumerable<KeyValuePair<string, object>> PrimaryKeyValues { get; }
    }
}

