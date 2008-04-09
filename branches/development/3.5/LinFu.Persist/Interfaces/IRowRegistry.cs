using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist
{
    public interface IRowRegistry
    {
        IRow GetRow(string TableName, object primaryKeyValue);

        bool HasRow(string tableName, object primaryKeyValue);
        void Register(string tableName, IRow row, object key);
    }
}
