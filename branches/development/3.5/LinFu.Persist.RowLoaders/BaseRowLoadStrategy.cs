using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Simple.IoC;

namespace LinFu.Persist
{
    public abstract class BaseRowLoadStrategy : IRowLoadStrategy
    {
        #region IRowLoadStrategy Members

        public IDbConnection Connection { get; set; }

        public abstract IEnumerable<IRow> Load(IEnumerable<IRowTaskItem> tasks);
        
        #endregion


        protected IEnumerable<IRow> CreateRows(IDbCommand command, IEnumerable<string> columns)
        {
            command.Connection = Connection;
            if (Connection.State != ConnectionState.Open)
                Connection.Open();
            IDataReader reader = command.ExecuteReader();
            IList<IRow> rows = new List<IRow>();
            while (reader.Read())
            {
                IRow row = new Row();
                foreach (string column in columns)
                {
                    object value = reader[column];
                    ICell cell = CreateCell(column, value);
                    
                    row.Cells[column] = cell;
                }
                rows.Add(row);
            }
            reader.Close();
            command.Dispose();
            Connection.Close();
            return rows;
        }
        protected virtual ICell CreateCell(string columnName, object value)
        {
            return new Cell() { Value = value };
        }
    }
    

}
