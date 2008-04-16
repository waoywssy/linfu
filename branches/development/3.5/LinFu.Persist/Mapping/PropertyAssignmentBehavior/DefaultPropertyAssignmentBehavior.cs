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
        public IValueConverter ValueConverter { get; set; }
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

            bool result = propertyType.IsAssignableFrom(columnType);

            if (ValueConverter != null)
            {
                result |= ValueConverter.CanConvertTo(propertyType, columnType);
            }
            return result;
        }

        public virtual void AssignPropertyValue(object target, PropertyInfo targetProperty, IRow sourceRow)
        {
            var propertyName = targetProperty.Name;
            var value = sourceRow.Cells[propertyName].Value;
            var propertyType = targetProperty.PropertyType;

            Type columnType = typeof(object);
            if (value != null)
                columnType = value.GetType();

            // Convert the value if necessary
            if (ValueConverter != null &&
                ValueConverter.CanConvertTo(propertyType, columnType))
                value = ValueConverter.ConvertTo(targetProperty.PropertyType, value);

            _dynamic.Target = target;
            _dynamic.Properties[propertyName] = value;
        }
    }
}
