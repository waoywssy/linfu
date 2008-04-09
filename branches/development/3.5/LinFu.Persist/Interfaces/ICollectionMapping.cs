using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist
{
    public interface ICollectionMapping
    {
        string PropertyName { get; set; }
        string SourceIDColumn { get; set; }
        string TargetIDColumn { get; set; }
        string TargetTableName { get; set; }
        Type ItemType { get; set; }

    }
}
