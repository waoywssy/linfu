using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist.Mappers
{
    public class CollectionMapping : ICollectionMapping
    {
        public string PropertyName
        {
            get;
            set;
        }

        public string SourceIDColumn
        {
            get;
            set;
        }

        public string TargetIDColumn
        {
            get;
            set;
        }

        public string TargetTableName
        {
            get;
            set;
        }

        public Type ItemType
        {
            get;
            set;
        }
    }
}
