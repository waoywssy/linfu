using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist
{
    public interface IColumn
    {
        string ColumnName { get; set; }
        Type DataType { get; set; }
        ITable Table { get; set; }
    }
}
