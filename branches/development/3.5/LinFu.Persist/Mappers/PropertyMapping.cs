using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist
{
    public class PropertyMapping : IPropertyMapping
    {
        public Type ColumnType
        {
            get;
            set;
        }
        public Type PropertyType
        {
            get;
            set;
        }

        public string ColumnName
        {
            get;
            set;
        }

        public string PropertyName
        {
            get;
            set;
        }
    }
}
