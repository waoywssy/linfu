using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simple.IoC.Loaders;

namespace LinFu.Persist
{
    [Implements(typeof(IRowRegistry), LifecycleType.Singleton)]
    public class RowRegistry : IRowRegistry
    {
        public IRow GetRow(string TableName, Dictionary<string, object> primaryKeyValues)
        {
            throw new NotImplementedException();
        }

        public bool HasRow(string tableName, Dictionary<string, object> primaryKeyValues)
        {
            throw new NotImplementedException();
        }

        public void Register(string tableName, IRow row, Dictionary<string, object> primaryKeyValues)
        {
            throw new NotImplementedException();
        }
    }
}
