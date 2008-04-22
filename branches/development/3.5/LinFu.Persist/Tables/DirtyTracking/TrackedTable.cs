using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.DynamicProxy;

namespace LinFu.Persist
{
    public class TrackedTable : ITrackedTable
    {
        private static readonly ProxyFactory _factory = new ProxyFactory();
        private Dictionary<string, IColumn> _columns = new Dictionary<string, IColumn>();

        private IList<IRow> _rows;
        private HashSet<ITrackedRow> _modifiedRows = new HashSet<ITrackedRow>();
        private HashSet<ITrackedRow> _deletedRows = new HashSet<ITrackedRow>();
        private HashSet<ITrackedRow> _addedRows = new HashSet<ITrackedRow>();
        public TrackedTable()
        {
            var rowList = new List<IRow>();
            CollectionWatcher<IRow> watcher = new CollectionWatcher<IRow>(rowList);

            // Monitor the collection for any changes
            IList<IRow> rows = _factory.CreateProxy<IList<IRow>>(watcher, typeof(ICollectionEvents<IRow>));

            // Transparently wrap each row with a change tracker
            var wrapper = new TransparentRowCollectionWrapper(rows);
            rows = _factory.CreateProxy<IList<IRow>>(wrapper, typeof(ICollectionEvents<IRow>));
            _rows = rows;

            ICollectionEvents<IRow> rowEvents = rows as ICollectionEvents<IRow>;
            if (rowEvents == null)
                return;

            rowEvents.ItemAdded += new EventHandler<EventArgs<IRow>>(rowEvents_ItemAdded);
            rowEvents.ItemRemoved += new EventHandler<EventArgs<IRow>>(rowEvents_ItemRemoved);
        }
        public IEnumerable<ITrackedRow> AddedRows
        {
            get { return _addedRows; }
        }
        public IEnumerable<ITrackedRow> ModifiedRows
        {
            get
            {
                return _modifiedRows;
            }
        }
        public IEnumerable<ITrackedRow> DeletedRows
        {
            get { return _deletedRows; }
        }
        void rowEvents_ItemRemoved(object sender, EventArgs<IRow> e)
        {
            var row = e.Item;

            var trackedRow = row as ITrackedRow;
            if (trackedRow == null)
                return;

            // Keep track of each deleted item
            _deletedRows.Add(trackedRow);

            // Unsubscribe from this row's events
            trackedRow.CellModified -= this.trackedRow_CellModified;

            if (RowDeleted != null)
                RowDeleted(this, new EventArgs<ITrackedRow>() { Item = trackedRow });
        }

        void rowEvents_ItemAdded(object sender, EventArgs<IRow> e)
        {
            var row = e.Item;
            var trackedRow = row as ITrackedRow;
            if (trackedRow == null)
                return;

            trackedRow.CellModified += new EventHandler<CellEventArgs>(trackedRow_CellModified);
            if (!_addedRows.Contains(trackedRow))
                _addedRows.Add(trackedRow);

            if (RowAdded != null)
                RowAdded(this, new EventArgs<ITrackedRow>() { Item = trackedRow });
        }

        void trackedRow_CellModified(object sender, CellEventArgs e)
        {
            var row = e.ParentRow as ITrackedRow;
            if (row == null)
                return;

            _modifiedRows.Add(row);

            if (RowModified != null)
                RowModified(this, new EventArgs<ITrackedRow>() { Item = row });
        }
        public IDictionary<string, IColumn> Columns
        {
            get
            {
                return _columns;
            }
        }

        public IList<IRow> Rows
        {
            get
            {
                return _rows;
            }
        }

        public string TableName
        {
            get;
            set;
        }
        public void Reset()
        {
            var resetList = new List<ITrackedRow>();
            resetList.AddRange(_addedRows);
            resetList.AddRange(_modifiedRows);

            // Reset every new and modified row
            resetList.ForEach(r =>
            {
                if (r.IsDirty)
                    r.Reset();
            }
            );

            _addedRows.Clear();
            _modifiedRows.Clear();
            _deletedRows.Clear();
        }



        public event EventHandler<EventArgs<ITrackedRow>> RowAdded;
        public event EventHandler<EventArgs<ITrackedRow>> RowModified;
        public event EventHandler<EventArgs<ITrackedRow>> RowDeleted;
    }
}
