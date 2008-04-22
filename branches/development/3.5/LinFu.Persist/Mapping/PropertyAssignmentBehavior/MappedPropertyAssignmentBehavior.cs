using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using LinFu.Reflection;

namespace LinFu.Persist
{
    //public class MappedPropertyAssignmentBehavior : BasePropertyAssignmentBehavior
    //{
    //    private DynamicObject _dynamic = new DynamicObject();
    //    public MappedPropertyAssignmentBehavior(IPropertyMapping mapping)
    //        : base(mapping)
    //    {
    //    }
    //    public virtual IValueConverter ValueConverter
    //    {
    //        get;
    //        set;
    //    }
    //    public override bool CanModify(PropertyInfo targetProperty, IRow currentRow)
    //    {
    //        var targetTable = currentRow.Table;
    //        bool result = base.CanModify(targetProperty, currentRow);

    //        var converter = ValueConverter;

    //        if (converter == null || PropertyMapping == null)
    //            return false;

    //        // The property can only be mapped to a single field 
    //        if (PropertyMapping.ForeignKey.Columns.Count != 1)
    //            return false;

    //        var columns = targetTable.Columns;
    //        string columnName = PropertyMapping.ColumnName;

    //        var targetColumn = columns[columnName];
    //        if (ValueConverter != null && !ValueConverter.CanConvertTo(targetProperty.PropertyType, targetColumn.DataType))
    //            return false;

    //        return result;
    //    }

    //    protected override void Modify(object target, string propertyName, 
    //        IEnumerable<KeyValuePair<string, object>> columnValues, IRow sourceRow)
    //    {
    //        var propertyType = PropertyMapping.PropertyType;
    //        var value = propertyValue;

    //        // Perform any conversion if necessary

    //        if (propertyValue != null)
    //            value = Convert(propertyValue, propertyType, value);

    //        _dynamic.Target = target;
    //        _dynamic.Properties[propertyName] = propertyValue;
    //    }

    //    private object Convert(object propertyValue, Type propertyType, object value)
    //    {
    //        Type valueType = propertyValue.GetType();
    //        if (!propertyType.IsAssignableFrom(valueType) && ValueConverter != null)
    //            value = ValueConverter.ConvertTo(propertyType, propertyValue);
    //        return value;
    //    }
    //}
}
