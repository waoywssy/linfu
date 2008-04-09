using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist
{
    public class Relation : IRelation
    {
        public IColumn SourceColumn
        {
            get;
            set;
        }

        public IColumn TargetColumn
        {
            get;
            set;
        }
    }
}
