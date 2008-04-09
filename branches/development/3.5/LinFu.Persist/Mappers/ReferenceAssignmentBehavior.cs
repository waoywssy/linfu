using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.Reflection;
using System.Reflection;

namespace LinFu.Persist
{
    public class ReferenceAssignmentBehavior : BasePropertyAssignmentBehavior
    {
        private IRowRegistry _registry;
        private IMapperRegistry _MapperRegistry;
        private DynamicObject _dynamic = new DynamicObject();
        public ReferenceAssignmentBehavior(IRowRegistry registry,
            IMapperRegistry MapperRegistry)
        {
            _registry = registry;
            _MapperRegistry = MapperRegistry;
        }

        public override bool CanModify(PropertyInfo targetProperty, IRow currentRow)
        {
            // The property can only be modified if it:
            // 1) Is a reference type
            var targetTable = currentRow.Table;
            var propertyType = targetProperty.PropertyType;

            if (propertyType.IsValueType || propertyType == typeof(string))
                return false;

            if (!propertyType.IsClass && !propertyType.IsInterface)
                return false;

            if (PropertyMapping == null || _registry == null)
                return false;

            string propertyName = targetProperty.Name;
            string columnName = PropertyMapping.ColumnName;

            // 2) Has a matching table and row that points to its data
            var relations = from r in targetTable.Relations
                            where r.SourceColumn.ColumnName == columnName
                            select r;

            if (relations.Count() == 0)
                return false;

            // 3) Has a corresponding Mapper
            if (!_MapperRegistry.HasMapperFor(propertyType, targetTable.Columns))
                return false;

            return base.CanModify(targetProperty, currentRow);
        }

        protected override void Modify(object target, string propertyName, object propertyValue, IRow sourceRow)
        {
            // Find the actual row that the foreign key is referencing
            ITable sourceTable = sourceRow.Table;

            string sourceTableName = sourceTable.TableName;
            string columnName = PropertyMapping.ColumnName;

            var matchingRelations = (from r in sourceTable.Relations
                                     where r.SourceColumn.ColumnName == columnName
                                     select r).ToList();

            if (matchingRelations.Count == 0)
                return;

            var relation = matchingRelations.First();
            var targetColumn = relation.TargetColumn;
            ITable relatedTable = targetColumn.Table;
            IRow actualRow = _registry.GetRow(relatedTable.TableName, propertyValue);

            if (actualRow == null)
                return;

            IMapper Mapper = _MapperRegistry.GetMapper(PropertyMapping.PropertyType, relatedTable.Columns);
            if (Mapper == null || target == null)
                return;

            // Create the new object instance using the row data
            object instance = Mapper.CreateItem(PropertyMapping.PropertyType, actualRow);

            // Assign it to the target property            
            _dynamic.Target = target;
            _dynamic.Properties[propertyName] = instance;
        }
        protected override bool CanConvertFrom(IColumn sourceColumn, Type sourceType, Type propertyType)
        {
            if (PropertyMapping == null)
                return false;

            return PropertyMapping.ColumnName == sourceColumn.ColumnName &&
                PropertyMapping.ColumnType.IsAssignableFrom(sourceType);
        }
       
    }
}
