using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace LinFu.Persist
{
    public interface IRowLoadStrategy
    {
        IDbConnection Connection { get; set; }
        IEnumerable<IRow> Load(IEnumerable<IRowTaskItem> tasks);
    }
}
