using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist
{
    public class RowRegistry : IRowRegistry
    {
        private Dictionary<string, Dictionary<object, IRow>> _entries = new Dictionary<string, Dictionary<object, IRow>>();
        public IRow GetRow(string tableName, object primaryKeyValue)
        {
            if (!_entries.ContainsKey(tableName))
                return null;

            var rows = _entries[tableName];

            if (!rows.ContainsKey(primaryKeyValue))
                return null;

            return rows[primaryKeyValue];
        }

        public bool HasRow(string tableName, object primaryKeyValue)
        {
            // Check if the table exists
            if (!_entries.ContainsKey(tableName))
                return false;

            var rows = _entries[tableName];

            if (!rows.ContainsKey(primaryKeyValue))
                return false;

            return true;
        }

        public void Register(string tableName, IRow row, object key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            if (!_entries.ContainsKey(tableName))
                _entries[tableName] = new Dictionary<object, IRow>();

            _entries[tableName][key] = row;
        }
    }
}
