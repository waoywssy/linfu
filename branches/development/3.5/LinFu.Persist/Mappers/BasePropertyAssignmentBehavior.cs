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
            var columnName = _mapping.ColumnName;
            var columns = targetTable.Columns;

            // The property can be modified if:
            // 1) The target table has the specified source column
            if (!columns.ContainsKey(columnName))
                return false;

            // 2) The target property maps to the source column
            if (targetProperty.Name != _mapping.PropertyName)
                return false;

            // 3) The target property type is compatible with the source column type OR
            //    there is a user-defined conversion between the two types

            var targetColumn = columns[columnName];
            if (targetProperty.PropertyType.IsAssignableFrom(targetColumn.DataType))
                return true;

            if (CanConvertFrom(targetColumn, targetColumn.DataType, targetProperty.PropertyType))
                return true;

            return false;
        }

        public void AssignPropertyValue(object target, PropertyInfo targetProperty, IRow sourceRow)
        {

            var columnName = _mapping.ColumnName;
            var propertyName = targetProperty.Name;

            // The column must exist
            if (!sourceRow.Cells.ContainsKey(columnName))
                return;

            var table = sourceRow.Table;
            var targetColumn = table.Columns[columnName];

            // Copy the value from the source row to the target property
            var sourceCell = sourceRow.Cells[columnName];
            object propertyValue = sourceCell.Value;

            Modify(target, propertyName, propertyValue, sourceRow);
        }

        protected virtual bool CanConvertFrom(IColumn sourceColumn, Type sourceType, Type propertyType)
        {
            return false;
        }
        protected abstract void Modify(object target, string propertyName, object propertyValue, IRow sourceRow);
    }
}
