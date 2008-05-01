using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.Persist.Metadata;

namespace LinFu.Persist.Metadata.Test
{
    public class TableFactory : ITableFactory
    {
        

        #region ITableFactory Members

        public ITableRepository Repository { get; set; }


        public ITable CreateTable(string tableName)
        {
            return CreateTable(Repository.Tables.Where(t => t.Value.TableName == tableName).First().Value);
        }

        public ITable CreateTable(ITableInfo tableInfo)
        {
            ITable table = new Table();
            foreach (var item in tableInfo.Columns.Values)
            {
                CreateColumn(table, item);
            }
            return table;
        }

        private IColumn CreateColumn(ITable table, IColumnInfo columnInfo)
        {
            IColumn column = new Column();
            column.ColumnName = columnInfo.ColumnName;
            column.Table = table;
            column.DataType = columnInfo.DataType;
            table.Columns.Add(column.ColumnName, column);

            return column;
        }

        #endregion
    }
}
