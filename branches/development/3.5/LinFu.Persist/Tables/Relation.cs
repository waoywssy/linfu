using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist
{
    public class Relation : IRelation
    {

        public IKey SourceKey
        {
            get;
            set;
        }

        public IKey TargetKey
        {
            get;
            set;
        }
    }
}
