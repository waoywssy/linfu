using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using LinFu.Delegates;
using LinFu.Reflection;
using LinFu.Lazy;

namespace LinFu.Persist
{
    public class DefaultMapper : IMapper
    {
        public DefaultMapper()
        {
            Instantiator = new DefaultInstantiator();
        }
        public IInstantiator Instantiator { get; set; }
        public IPropertyAssignmentBehavior PropertyAssignmentBehavior { get; set; }

        public virtual object CreateItem(Type targetType, IRow currentRow)
        {
            var table = currentRow.Table;

            if (PropertyAssignmentBehavior == null)
                throw new NullReferenceException("The PropertyAssignmentBehavior property cannot be null");

            var columns = table.Columns;

            // Create the target object
            object newItem = Instantiator.CreateNew(targetType);

            // TODO: Delegate the GetValidProperties call to an interface
            var validProperties = GetValidProperties(targetType, currentRow);
            foreach (var property in validProperties)
            {
                string propertyName = property.Name;
                string columnName = property.Name;
                Type propertyType = property.PropertyType;


                PropertyAssignmentBehavior.AssignPropertyValue(newItem, property, currentRow);
            }
            return newItem;
        }

        protected virtual IList<PropertyInfo> GetValidProperties(Type targetType, IRow currentRow)
        {
            if (PropertyAssignmentBehavior == null)
                return new List<PropertyInfo>();

            // TODO: Cache the method output

            // Determine which properties can be mapped
            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            var validProperties = (from p in targetType.GetProperties(flags)
                                   where PropertyAssignmentBehavior.CanModify(p, currentRow)
                                   select p).ToList();

            return validProperties;
        }


        public virtual bool CanCreateWith(IEnumerable<IColumn> columns)
        {
            return true;
        }


        public virtual IEnumerable<object> CreateItems(Type itemType, IEnumerable<IRow> rows)
        {
            MethodInfo createItemMethodDefinition = typeof(DefaultMapper).GetMethod("CreateItemList", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo createItemMethod = createItemMethodDefinition.MakeGenericMethod(itemType);

            var result = (IEnumerable<object>)createItemMethod.Invoke(this, new object[] { rows });
            
            return result;
        }
        protected virtual IEnumerable<object> CreateItemList<T>(IEnumerable<IRow> rows)
            where T : class
        {
            
            var itemType = typeof(T);
            Func<IList<T>> createItems = () =>
                {
                    IList<T> list = new List<T>();
                    foreach (var row in rows)
                    {
                        T newItem = (T)CreateItem(itemType, row);
                        list.Add(newItem);
                    }
                    return list;
                };

            // Cascade load each item by default
            int itemCount = rows.Count();
            LazyList<T> lazyList = new LazyList<T>(createItems);            
            List<object> results = new List<object>();
            for (int i = 0; i < itemCount; i++)
            {
                var index = i;
                // Access the item at the specified list index
                // when the actual item is invoked                
                results.Add(lazyList.CreateListItem(list => list[index]));
            }
            return results;
        }

    }    
}
