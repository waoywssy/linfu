using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using LinFu.Reflection;

namespace LinFu.Persist
{
    public class DefaultPropertyAssignmentBehavior : IPropertyAssignmentBehavior
    {
        private DynamicObject _dynamic = new DynamicObject();

        public virtual bool CanModify(PropertyInfo targetProperty, IRow currentRow)
        {
            var targetTable = currentRow.Table;
            var propertyName = targetProperty.Name;
            var propertyType = targetProperty.PropertyType;

            if (!targetTable.Columns.HasColumn(propertyName))
                return false;

            // The property types must be compatible
            // with the source column type
            var columnType = targetTable.Columns[propertyName].DataType;

            return propertyType.IsAssignableFrom(columnType);
        }

        public virtual void AssignPropertyValue(object target, PropertyInfo targetProperty, IRow sourceRow)
        {
            var propertyName = targetProperty.Name;
            _dynamic.Target = target;
            _dynamic.Properties[propertyName] = sourceRow.Cells[propertyName].Value;
        }
    }
}
