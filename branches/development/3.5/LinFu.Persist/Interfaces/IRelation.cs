using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist
{
    public interface IRelation
    {
        IColumn SourceColumn { get; set; }
        IColumn TargetColumn { get; set; }
    }
}
