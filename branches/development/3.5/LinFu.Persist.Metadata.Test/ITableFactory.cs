using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.Persist.Metadata;

namespace LinFu.Persist.Metadata.Test
{
    public interface ITableFactory
    {
        ITableRepository Repository { get; set; }
        ITable CreateTable(string tableName);
        ITable CreateTable(ITableInfo tableInfo);
    }
}
