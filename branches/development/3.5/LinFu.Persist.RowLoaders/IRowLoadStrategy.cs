using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace LinFu.Persist
{
    public interface IRowLoadStrategy
    {        
        IEnumerable<IRow> Load(IEnumerable<IRowTaskItem> tasks);
    }
}
