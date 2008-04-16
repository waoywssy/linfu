using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simple.IoC.Loaders;

namespace LinFu.Persist
{
    [Implements(typeof(IPropertyMapping), LifecycleType.OncePerRequest)]
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
