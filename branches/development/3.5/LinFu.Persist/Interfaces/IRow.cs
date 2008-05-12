using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist
{
    public interface IRow
    {
        ITable Table { get; set; }
        object this[string columnName]
        {
            get;
            set;
        }
        IDictionary<string, ICell> Cells { get; }
    }
}
