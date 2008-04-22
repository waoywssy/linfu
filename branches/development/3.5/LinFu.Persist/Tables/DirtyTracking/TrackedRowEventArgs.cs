using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist
{
    public class TrackedRowEventArgs : EventArgs
    {
        public ITrackedRow Row { get; internal set; }
    }
}
