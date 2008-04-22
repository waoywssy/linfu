using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist
{
    public interface ITrackedTable : ITable
    {
        event EventHandler<EventArgs<ITrackedRow>> RowAdded;
        event EventHandler<EventArgs<ITrackedRow>> RowModified;
        event EventHandler<EventArgs<ITrackedRow>> RowDeleted;

        IEnumerable<ITrackedRow> ModifiedRows { get; }
        IEnumerable<ITrackedRow> DeletedRows { get; }
        IEnumerable<ITrackedRow> AddedRows { get; }

        void Reset();
    }
}
