using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simple.IoC.Loaders;
using System.Data;

namespace LinFu.Persist
{
    [Implements(typeof(IFillTable), LifecycleType.OncePerRequest)]
    public class FillTable : IFillTable
    {
        public void Fill(ITable table, IDbCommand command)
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

                newRow = new Row();
                PopulateRowCells(reader, fieldCount, newRow);

                table.Rows.Add(newRow);
            }

            reader.Close();
            reader.Dispose();
        }
        private void PopulateRowCells(IDataReader reader, int fieldCount, IRow newRow)
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
