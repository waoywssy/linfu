using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist
{
    // TODO: Remove this from production builds
    public class SamplePropertyMappingRegistry : IPropertyMappingRegistry
    {
        public bool HasPropertyMapping(Type declaringType, string propertyName, Type propertyType)
        {
            if (declaringType.Name != "Order")
                return false;

            if (propertyName == "CustomerID" || propertyName == "OrderID")
                return true;

            throw new NotImplementedException();
        }

        public IPropertyMapping GetPropertyMapping(Type declaringType, string propertyName, Type propertyType)
        {
            if (declaringType.Name != "Order")
                return null;

            if (propertyName == "CustomerID")
            {

                var mapping = new PropertyMapping()
                {
                    PropertyName = "CustomerID",
                    PropertyType = typeof(string),
                    ColumnName = "CustomerID",
                    ColumnType = typeof(string)
                };

                return mapping;
            }

            if (propertyName == "OrderID")
            {
                var mapping = new PropertyMapping()
                {
                    PropertyName = "OrderID",
                    PropertyType = typeof(int),
                    ColumnName = "OrderID",
                    ColumnType = typeof(int)
                };

                return mapping;
            }

            return null;
        }
    }
}
