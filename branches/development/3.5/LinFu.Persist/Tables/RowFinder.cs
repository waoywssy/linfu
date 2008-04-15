using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist
{
    public class RowFinder : IRowFinder
    {
        private ITable _table;
        public RowFinder(ITable table)
        {
            _table = table;
            Indexer = new DefaultIndexer();
        }
        public IRowIndexer Indexer { get; set; }
        public bool HasRows(string targetTableName, string searchColumnName, object key)
        {
            if (_table == null)
                return false;

            if (_table.TableName != targetTableName)
                return false;

            if (Indexer == null)
                return false;

            return true;
        }

        public IEnumerable<IRow> GetRows(string targetTableName, string searchColumnName, object key)
        {
            if (_table == null || _table.TableName != targetTableName || Indexer == null)
                return new IRow[0];


            var index = Indexer.IndexByColumn(searchColumnName, _table);

            // Find the list of rows that have that particular column value
            if (!index.ContainsKey(key))
                return new IRow[0];


            return index[key];
        }
    }
}
