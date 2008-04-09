using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist
{
    internal struct RelatedTableKey
    {
        public string ColumnName { get; set; }
        public string SourceTableName { get; set; }
    }
}
