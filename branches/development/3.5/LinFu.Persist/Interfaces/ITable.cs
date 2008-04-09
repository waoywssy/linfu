using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist
{
    public interface ITable
    {
        string TableName { get; set; }

        IDictionary<string, IColumn> Columns { get; }
        IList<IRelation> Relations { get; }
        IList<IRow> Rows { get; }
    }
}
