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
        public void CreateSchemaFrom(SqlCommand schemaCommand)
        {
            SqlDataAdapter adapter = new SqlDataAdapter(schemaCommand);
            DataTable schemaTable = new DataTable();
            adapter.Fill(schemaTable);

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

        public void FillWith(SqlCommand command)
        {
            var reader = command.ExecuteReader();

            int fieldCount = reader.VisibleFieldCount;
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
                if (!string.IsNullOrEmpty(PrimaryKeyField) && ordinals.ContainsKey(PrimaryKeyField))
                {
                    int targetOrdinal = ordinals[PrimaryKeyField];
                    primaryKeyValue = reader.GetValue(targetOrdinal);
                }

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

        private IRow CreateRow(IRow newRow, object primaryKeyValue)
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

        private static void PopulateRowCells(SqlDataReader reader, int fieldCount, IRow newRow)
        {
            for (int i = 0; i < fieldCount; i++)
            {
                string fieldName = reader.GetName(i);
                object value = reader.GetValue(i);

                newRow.Cells[fieldName] = new Cell() { Value = value };
            }
        }
    }
}
