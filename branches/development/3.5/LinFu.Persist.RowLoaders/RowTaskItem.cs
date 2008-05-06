using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace LinFu.Persist
{
    public class RowTaskItem : IRowTaskItem
    {
        private IEnumerable<string> _columns;
        private IEnumerable<KeyValuePair<string, object>> _keys;
        public RowTaskItem(string tablename, IEnumerable<string> columns, IEnumerable<KeyValuePair<string, object>> keys)
        {
            TableName = tablename;
            _columns = columns;
            _keys = keys;
        }
        public string TableName
        {
            get;
            set;
        }

        public IEnumerable<string> Columns
        {
            get
            {
                return _columns;
            }
        }

        public IEnumerable<KeyValuePair<string, object>> PrimaryKeyValues
        {
            get
            {
                return _keys;
            }
        }

    }
}
