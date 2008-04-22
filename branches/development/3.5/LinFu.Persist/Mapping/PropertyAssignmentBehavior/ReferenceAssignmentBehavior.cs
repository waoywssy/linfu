using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.Reflection;
using System.Reflection;

namespace LinFu.Persist
{
    //public class ReferenceAssignmentBehavior : BasePropertyAssignmentBehavior
    //{
    //    private IRowRegistry _registry;
    //    private IMapperRegistry _MapperRegistry;
    //    private DynamicObject _dynamic = new DynamicObject();
    //    public ReferenceAssignmentBehavior(IRowRegistry registry,
    //        IMapperRegistry MapperRegistry)
    //    {
    //        _registry = registry;
    //        _MapperRegistry = MapperRegistry;
    //    }

    //    public override bool CanModify(PropertyInfo targetProperty, IRow currentRow)
    //    {
    //        // The property can only be modified if it:
    //        // 1) Is a reference type
    //        var targetTable = currentRow.Table;
    //        var propertyType = targetProperty.PropertyType;

    //        if (propertyType.IsValueType || propertyType == typeof(string))
    //            return false;

    //        if (!propertyType.IsClass && !propertyType.IsInterface)
    //            return false;

    //        if (PropertyMapping == null || _registry == null)
    //            return false;

    //        string propertyName = targetProperty.Name;
    //        var columnNames = from c in PropertyMapping.MappedColumns.Columns
    //                                 select c.ColumnName;

    //        // 2) Has a matching table and row that points to its data            
    //        HashSet<string> columnList = new HashSet<string>(columnNames);
    //        int columnCount = columnList.Count;
    //        if (columnCount == 0)
    //            return false;

    //        bool hasRelation = false;
    //        foreach (var relation in targetTable.Relations)
    //        {
    //            if (relation == null)
    //                continue;

    //            var sourceKey = relation.SourceKey;
    //            if (sourceKey == null)
    //                continue;


    //            var sourceKeyColumns = from c in sourceKey.Columns
    //                                   let columnName = c.ColumnName
    //                                   where columnList.Contains(columnName)
    //                                   select c.ColumnName;

    //            int columnMatches = sourceKeyColumns.Count();
    //            if (columnMatches != columnCount)
    //                continue;
                
    //            hasRelation = true;
    //            break;
    //        }

    //        if (!hasRelation)
    //            return false;

    //        // 3) Has a corresponding Mapper
    //        if (!_MapperRegistry.HasMapperFor(propertyType, targetTable.Columns))
    //            return false;

    //        return base.CanModify(targetProperty, currentRow);
    //    }

    //    protected override void Modify(object target, string propertyName, 
    //        IEnumerable<KeyValuePair<string, object>> columnValues, IRow sourceRow)
    //    {

    //        if (PropertyMapping == null)
    //            return;

    //        // Find the actual row that the foreign key is referencing
    //        ITable sourceTable = sourceRow.Table;

    //        string sourceTableName = sourceTable.TableName;
    //        var columns = PropertyMapping.MappedColumns.Columns;

    //        var columnNames = from c in columns
    //                          where c != null
    //                          select c.ColumnName;

    //        var columnList = new HashSet<string>(columnNames);

    //        //var matchingRelations = from r in sourceTable.Relations
    //        //                        where r.SourceKey != null
                                    

    //        //string columnName = PropertyMapping.ColumnName;

    //        //var matchingRelations = (from r in sourceTable.Relations
    //        //                         where r.SourceKey != null &&
    //        //                         r.SourceKey.Columns.Where(c => c.ColumnName == columnName).Count() > 0
    //        //                         select r).ToList();

    //        //if (matchingRelations.Count == 0)
    //        //    return;

    //        var relation = matchingRelations.First();
    //        var targetColumns = relation.TargetKey.Columns;



    //        if (targetColumns.Count == 0)
    //            return;

    //        ITable relatedTable = targetColumns[0].Table;

    //        Dictionary<string, object> primaryKeys = new Dictionary<string, object>();
    //        foreach (var key in relation.TargetKey.Columns)
    //        {

    //        }

    //        IRow actualRow = _registry.GetRow(relatedTable.TableName, primaryKeys);

    //        if (actualRow == null)
    //            return;

    //        IMapper Mapper = _MapperRegistry.GetMapper(PropertyMapping.PropertyType, relatedTable.Columns);
    //        if (Mapper == null || target == null)
    //            return;

    //        // Create the new object instance using the row data
    //        object instance = Mapper.CreateItem(PropertyMapping.PropertyType, actualRow);

    //        // Assign it to the target property            
    //        _dynamic.Target = target;
    //        _dynamic.Properties[propertyName] = instance;
    //    }
    //    protected override bool CanConvertFrom(IEnumerable<IColumn> sourceColumns, Type propertyType)
    //    {
    //        if (PropertyMapping == null)
    //            return false;

    //        return PropertyMapping.ColumnName == sourceColumn.ColumnName &&
    //            PropertyMapping.ColumnType.IsAssignableFrom(sourceType);
    //    }

    //}
}
