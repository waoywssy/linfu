using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.DynamicProxy;

namespace LinFu.Persist
{
    public partial class TrackedRow : ITrackedRow, ICellEventHandler
    {
        private static readonly ProxyFactory _factory = new ProxyFactory();
        private IDictionary<string, ICell> _cells;
        private IDictionary<string, ICellEvents> _cellEvents = new Dictionary<string, ICellEvents>();
        private HashSet<string> _modifiedList = new HashSet<string>();
        private IRow _realRow;

        public TrackedRow(IRow actualRow)
        {
            if (actualRow == null)
                throw new ArgumentNullException("actualRow");

            TrackingEnabled = true;
            _realRow = actualRow;
            #region Initialize the dictionary
            var cells = _realRow.Cells;
            var watcher = new DictionaryWatcher<string, ICell>(cells);


            var trackedDictionary = _factory.CreateProxy<IDictionary<string, ICell>>(watcher,
                typeof(IDictionaryEvents<string, ICell>));


            var wrapper = new TransparentCellWrapper(this, trackedDictionary, this);
            _cells = _factory.CreateProxy<IDictionary<string, ICell>>(wrapper, typeof(IDictionaryEvents<string, ICell>));
            #endregion


            IDictionaryEvents<string, ICell> dictionary = trackedDictionary as IDictionaryEvents<string, ICell>;
            if (dictionary == null)
                return;

            // Keep track of which items have been modified 
            dictionary.ItemAdded += cells_ItemAdded;
            dictionary.ItemModified += cells_ItemModified;
            dictionary.ItemRemoved += cells_ItemRemoved;
            dictionary.ItemsCleared += cells_Cleared;
        }

        public bool TrackingEnabled
        {
            get;
            set;
        }
        public bool IsDirty
        {
            get { return _modifiedList.Count > 0; }
        }
        public override bool Equals(object obj)
        {
            bool isRow = obj is IRow;
            if (!isRow)
                return base.Equals(obj);

            // HACK: Compare the actual row with the other row
            return object.ReferenceEquals(obj, _realRow);
        }
        public override int GetHashCode()
        {
            if (_realRow == null)
                return base.GetHashCode();

            return _realRow.GetHashCode();
        }
        private void SetModified(string columnName)
        {
            if (!TrackingEnabled)
                return;

            if (CellModified != null)
            {
                var args = new CellEventArgs()
                {
                    ColumnName = columnName,
                    ParentRow = this,
                    TargetCell = _cells[columnName]
                };

                CellModified(this, args);
            }

            if (_modifiedList.Contains(columnName))
                return;

            _modifiedList.Add(columnName);
        }

        public void Reset()
        {
            _modifiedList.Clear();
        }

        public IDictionary<string, ICell> Cells
        {
            get { return _cells; }
        }

        public ITable Table
        {
            get
            {
                return _realRow.Table;
            }
            set
            {
                _realRow.Table = value;
            }
        }

        public event EventHandler<CellEventArgs> CellModified;

        public IEnumerable<string> ModifiedColumns
        {
            get
            {
                return _modifiedList;
            }
        }

        public void Attach(ICellEvents events)
        {
            events.Modified += this.events_Modified;
        }


        public object this[string columnName]
        {
            get
            {
                var targetCell = _cells[columnName];
                return targetCell.Value;
            }
            set
            {
                var targetCell = _cells[columnName];
                targetCell.Value = value;
            }
        }
    }
}
