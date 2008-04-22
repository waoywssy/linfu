using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace LinFu.Persist
{
    public abstract class BasePropertyAssignmentBehavior : IPropertyAssignmentBehavior
    {
        private IPropertyMapping _mapping;
        protected BasePropertyAssignmentBehavior() { }
        protected BasePropertyAssignmentBehavior(IPropertyMapping mapping)
        {
            _mapping = mapping;
        }
        public IPropertyMapping PropertyMapping
        {
            get { return _mapping; }
            set { _mapping = value; }
        }
        public virtual bool CanModify(PropertyInfo targetProperty, IRow currentRow)
        {
            if (!targetProperty.CanWrite)
                return false;

            var targetTable = currentRow.Table;
            var columnNames = (from n in _mapping.MappedColumns.Columns
                               select n.ColumnName).ToList();

            var columns = targetTable.Columns;

            // The property can be modified if:
            // 1) The target table has the specified source columns
            bool hasColumns = true;

            var tableColumns = (from c in columns
                                select c.Key).AsEnumerable();

            HashSet<string> tableColumnList = new HashSet<string>(tableColumns);
            foreach (var current in columnNames)
            {
                if (tableColumnList.Contains(current))
                    continue;

                hasColumns = false;
                break;
            }


            if (!hasColumns)
                return false;

            // 2) The target property maps to the source column
            if (targetProperty.Name != _mapping.PropertyName)
                return false;

            // 3) The target property type is compatible with the source column type OR
            //    there is a user-defined conversion between the two types

            var targetColumns = from name in columnNames
                                where columns.ContainsKey(name)
                                select columns[name];

            //if (targetProperty.PropertyType.IsAssignableFrom(targetColumn.DataType))
            //    return true;

            if (CanConvertFrom(targetColumns, targetProperty.PropertyType))
                return true;

            return false;
        }

        public void AssignPropertyValue(object target, PropertyInfo targetProperty, IRow sourceRow)
        {
            if (_mapping == null)
                return;

            var columnNames = from c in _mapping.MappedColumns.Columns
                              where c != null
                              select c.ColumnName;

            var propertyName = targetProperty.Name;

            // The columns must exist
            bool columnsExist = true;
            foreach (var name in columnNames)
            {
                if (sourceRow.Cells.ContainsKey(name))
                    continue;

                columnsExist = false;
                break;
            }

            if (!columnsExist)
                return;

            var table = sourceRow.Table;
            var targetColumns = (from n in columnNames
                                 where table.Columns.ContainsKey(n)
                                 select table.Columns[n]).ToList();

            // Copy the value from the source row to the target property
            var sourceCells = from t in targetColumns
                              let cells = sourceRow.Cells
                              where cells.ContainsKey(t.ColumnName)
                              select new KeyValuePair<string, object>(t.ColumnName, cells[t.ColumnName].Value);


            Modify(target, propertyName, sourceCells, sourceRow);
        }

        protected virtual bool CanConvertFrom(IEnumerable<IColumn> sourceColumns, Type propertyType)
        {
            return false;
        }
        protected abstract void Modify(object target, string propertyName,
            IEnumerable<KeyValuePair<string, object>> columnValues, IRow sourceRow);
    }
}
