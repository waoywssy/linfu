using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simple.IoC.Loaders;

namespace LinFu.Persist
{
    [Implements(typeof(IRowIndexer), LifecycleType.OncePerRequest)]
    public class DefaultIndexer : IRowIndexer
    {
        public IDictionary<object, IList<IRow>> IndexByColumn(string columnName, ITable targetTable)
        {
            Dictionary<object, IList<IRow>> rowsByKey = new Dictionary<object, IList<IRow>>();
            foreach (var row in targetTable.Rows)
            {
                object key = row.Cells[columnName].Value;

                IList<IRow> currentList = null;
                if (!rowsByKey.ContainsKey(key))
                {
                    currentList = new List<IRow>();
                    rowsByKey[key] = currentList;
                }

                currentList = rowsByKey[key];
                currentList.Add(row);
            }

            return rowsByKey;
        }
    }
}
