using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Simple.IoC;

namespace LinFu.Persist
{
    public abstract class BaseRowLoadStrategy : IRowLoadStrategy,IInitialize
    {
        #region IRowLoadStrategy Members        

        public abstract IEnumerable<IRow> Load(IEnumerable<IRowTaskItem> tasks);
        
        #endregion

        #region IInitialize Members

        public void Initialize(IContainer container)
        {
            Container = container;
        }

        #endregion

        protected IContainer Container { get; set; }

        protected IEnumerable<IRow> CreateRows(IDataReader reader, IEnumerable<string> columns)
        {            
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
            return rows;
        }
        protected virtual ICell CreateCell(string columnName, object value)
        {
            return new Cell() { Value = value };
        }

       
    }
    

}
