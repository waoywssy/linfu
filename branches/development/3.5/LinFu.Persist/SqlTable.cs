using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace LinFu.Persist
{
    // TODO: Remove this from production builds
    public class SqlTable : Table
    {
        public string PrimaryKeyField { get; set; }
        public IRowRegistry RowRegistry { get; set; }
        public void CreateSchemaFrom(IDbDataAdapter adapter)
        {            
            DataSet dataSet = new DataSet();            
            adapter.Fill(dataSet);

            DataTable schemaTable = dataSet.Tables[0];
            Dictionary<string, DataColumn> columnIndex = new Dictionary<string, DataColumn>();

            // Index the table columns            
            foreach (DataColumn column in schemaTable.Columns)
            {
                var columnName = column.ColumnName;
                Columns[columnName] = new Column()
                {
                    ColumnName = columnName,
                    DataType = column.DataType,
                    Table = this
                };
            }
        }

        public void FillWith(IDbCommand command)
        {
            var reader = command.ExecuteReader();

            int fieldCount = reader.FieldCount;
            Dictionary<string, int> ordinals = new Dictionary<string, int>();
            for (int i = 0; i < fieldCount; i++)
            {
                string fieldName = reader.GetName(i);
                ordinals[fieldName] = i;
            }

            while (reader.Read())
            {
                IRow newRow = null;

                object primaryKeyValue = null;
                primaryKeyValue = GetPrimaryKey(reader, ordinals, primaryKeyValue);

                newRow = CreateRow(newRow, primaryKeyValue);
                PopulateRowCells(reader, fieldCount, newRow);

                Rows.Add(newRow);

                // Add the row to the registry, if necessary
                if (primaryKeyValue == null)
                    continue;

                if (RowRegistry == null)
                    continue;

                if (RowRegistry.HasRow(TableName, primaryKeyValue))
                    continue;

                RowRegistry.Register(TableName, newRow, primaryKeyValue);
            }

            reader.Close();
            reader.Dispose();
        }

        private object GetPrimaryKey(IDataReader reader, Dictionary<string, int> ordinals, object primaryKeyValue)
        {
            if (!string.IsNullOrEmpty(PrimaryKeyField) && ordinals.ContainsKey(PrimaryKeyField))
            {
                int targetOrdinal = ordinals[PrimaryKeyField];
                primaryKeyValue = reader.GetValue(targetOrdinal);
            }
            return primaryKeyValue;
        }

        protected virtual IRow CreateRow(IRow newRow, object primaryKeyValue)
        {
            if (RowRegistry != null && primaryKeyValue != null)
            {
                // Reuse the existing row, if possible
                newRow = RowRegistry.GetRow(TableName, primaryKeyValue);
            }

            if (newRow == null)
                newRow = new Row();

            newRow.Table = this;
            return newRow;
        }

        protected virtual ICell CreateCell(IRow parentRow, string fieldName, object value)
        {
            return new Cell() { Value = value };
        }

        private void PopulateRowCells(IDataReader reader, int fieldCount, IRow newRow)
        {
            for (int i = 0; i < fieldCount; i++)
            {
                string fieldName = reader.GetName(i);
                object value = reader.GetValue(i);

                newRow.Cells[fieldName] = CreateCell(newRow, fieldName, value);
            }
        }

        
    }
}
