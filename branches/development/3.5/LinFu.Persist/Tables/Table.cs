using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simple.IoC.Loaders;

namespace LinFu.Persist
{
    [Implements(typeof(ITable), LifecycleType.OncePerRequest)]
    public class Table : ITable
    {
        private List<IRow> _rows = new List<IRow>();
        private List<IRelation> _relations = new List<IRelation>();
        private Dictionary<string, IColumn> _columns = new Dictionary<string, IColumn>();

        public string TableName { get; set; }
        public IDictionary<string, IColumn> Columns
        {
            get
            {
                return _columns;
            }
        }
        public IList<IRow> Rows
        {
            get { return _rows; }
        }

        public IList<IRelation> Relations
        {
            get { return _relations; }
        }
    }
}
