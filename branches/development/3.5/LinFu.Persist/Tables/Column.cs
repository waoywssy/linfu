using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist
{
    public class Column : IColumn
    {
        public string ColumnName { get; set; }
        public Type DataType { get; set; }
        public ITable Table { get; set; }
    }
}
