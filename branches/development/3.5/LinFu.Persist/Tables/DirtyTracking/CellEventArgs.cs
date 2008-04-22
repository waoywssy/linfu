using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist
{
    public class CellEventArgs : EventArgs
    {
        public string ColumnName { get; internal set; }
        public IRow ParentRow { get; internal set; }
        public ICell TargetCell { get; internal set; }
    }

}
