using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist
{
    public interface IRowRegistry
    {
        IRow GetRow(string TableName, Dictionary<string, object> primaryKeyValues);

        bool HasRow(string tableName, Dictionary<string, object> primaryKeyValues);
        void Register(string tableName, IRow row, Dictionary<string, object> primaryKeyValues);
    }
}
