using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using LinFu.Reflection;

namespace LinFu.Persist
{
    public class CollectionPropertyAssignmentBehavior : IPropertyAssignmentBehavior
    {
        private DynamicObject _dynamic = new DynamicObject();
        public IMapperRegistry MapperRegistry { get; set; }
        public IRowFinder RowFinder { get; set; }
        public ICollectionMapping CollectionMapping { get; set; }
        public bool CanModify(PropertyInfo targetProperty, IRow currentRow)
        {
            if (CollectionMapping == null)
                return false;

            if (MapperRegistry == null)
                return false;

            if (RowFinder == null)
                return false;

            // The property can be modified if:
            // 1) The property type is a collection
            var propertyType = targetProperty.PropertyType;
            var interfaces = propertyType.GetInterfaces();

            // Only the ICollection<T> interface is supported
            var collectionInterfaces = (from i in interfaces
                                        where i.IsGenericType &&
                                        i.GetGenericTypeDefinition() == typeof(ICollection<>)
                                        select i).ToList();

            if (collectionInterfaces.Count == 0)
                return false;

            // Determine the element type and make sure that
            // it matches the type specified in the CollectionMapping
            var collectionType = collectionInterfaces.First();
            var typeArguments = collectionType.GetGenericArguments();
            var itemType = typeArguments[0];

            if (!CollectionMapping.ItemType.IsAssignableFrom(itemType))
                return false;

            // 2) The collection item type maps to an item that can be
            //    created with an IMapper instance
            if (!MapperRegistry.HasMapperFor(itemType, currentRow.Table.Columns))
                return false;

            // 3) There is a set of related rows mapped to this property name and
            //    primary key
            var propertyName = targetProperty.Name;
            if (CollectionMapping.PropertyName != propertyName)
                return false;

            // Extract the primary key from the source row
            object key = currentRow.Cells[CollectionMapping.SourceIDColumn].Value;

            if (!RowFinder.HasRows(CollectionMapping.TargetTableName, CollectionMapping.TargetIDColumn, key))
                return false;


            return true;
        }

        public void AssignPropertyValue(object target, PropertyInfo targetProperty, IRow sourceRow)
        {
            if (CollectionMapping == null)
                return;

            if (sourceRow == null)
                return;

            object key = sourceRow.Cells[CollectionMapping.SourceIDColumn].Value;
            var relatedRows = RowFinder.GetRows(CollectionMapping.TargetTableName, CollectionMapping.TargetIDColumn, key);

            var itemType = CollectionMapping.ItemType;
            var columns = sourceRow.Table.Columns;

            IMapper mapper = MapperRegistry.GetMapper(CollectionMapping.ItemType, columns);

            var items = mapper.CreateItems(itemType, relatedRows);

            _dynamic.Target = target;
            object targetCollection = _dynamic.Properties[targetProperty.Name];

            //// Reassign the dynamic object to the target collection and
            //// add the items to the target collection
            //_dynamic.Target = targetCollection;

            //foreach (var item in items)
            //{
            //    _dynamic.Methods["Add"](item);
            //}

            MethodInfo addItemDefinition = typeof(CollectionPropertyAssignmentBehavior).GetMethod("AddItems", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo addItemMethod = addItemDefinition.MakeGenericMethod(itemType);
            addItemMethod.Invoke(this, new object[] { targetCollection, items });
        }
        protected void AddItems<T>(object targetCollection, IEnumerable<object> items)
            where T : class
        {
            ICollection<T> collection = targetCollection as ICollection<T>;
            if (collection == null)
                return;

            foreach (var item in items)
            {
                collection.Add((T)item);
            }
        }
    }
}
