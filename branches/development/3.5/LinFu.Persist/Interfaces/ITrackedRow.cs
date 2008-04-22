using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist
{
    public interface ITrackedRow : IRow
    {
        event EventHandler<CellEventArgs> CellModified;
        IEnumerable<string> ModifiedColumns { get; }
        bool IsDirty { get; }
        bool TrackingEnabled { get; set; }
        void Reset();
    }
}
