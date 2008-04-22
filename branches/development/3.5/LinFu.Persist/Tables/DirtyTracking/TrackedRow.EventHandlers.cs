using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist
{
    public partial class TrackedRow
    {
        void cells_Cleared(object sender, EventArgs e)
        {
            // Unsubscribe the handler from the list of cells
            foreach (var cell in _cellEvents.Values)
            {
                cell.Modified -= this.events_Modified;
            }

            _cellEvents.Clear();
            _modifiedList.Clear();
        }
        void cells_ItemRemoved(object sender, DictionaryEventArgs<string, ICell> e)
        {
            var events = e.Item as ICellEvents;
            var columnName = e.Key;

            // Mark the column as modified
            SetModified(columnName);

            if (events == null)
                return;



            if (!_cellEvents.ContainsKey(columnName))
                return;

            events.Modified -= events_Modified;
            _cellEvents.Remove(columnName);
        }
        void cells_ItemModified(object sender, DictionaryEventArgs<string, ICell> e)
        {
            var columnName = e.Key;
            SetModified(columnName);
        }
        void cells_ItemAdded(object sender, DictionaryEventArgs<string, ICell> e)
        {
            var events = e.Item as ICellEvents;
            var columnName = e.Key;

            // Mark the column as modified
            SetModified(columnName);
            if (events == null)
                return;

            if (_cellEvents.ContainsKey(columnName))
                return;

            // Watch the added cell for any changes
            events.Modified += events_Modified;

            // Keep track of the list of cells currently being monitored
            _cellEvents[columnName] = events;
        }
        void events_Modified(object sender, CellEventArgs e)
        {
            var columnName = e.ColumnName;
            SetModified(columnName);
        }
    }
}
