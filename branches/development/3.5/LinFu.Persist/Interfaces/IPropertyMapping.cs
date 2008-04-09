using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist
{
    public interface IPropertyMapping
    {
        Type PropertyType { get; set; }
        Type ColumnType { get; set; }
        string ColumnName { get; set; }
        string PropertyName { get; set; }
    }
}
