using System;
using System.Collections.Generic;
using System.Text;
using LinFu.Persist.Metadata;
using Simple.IoC.Loaders;

namespace LinFu.Persist.Metadata.Implementation
{
    [Serializable]
    [Implements(typeof(IKeyInfo),LifecycleType.OncePerRequest)]
    public class KeyInfo : IKeyInfo
    {
        private readonly IList<IColumnInfo> _columns = new List<IColumnInfo>();
        
        #region IKeyInfo Members

        public ITableInfo Table{get;set;}
        

        public IList<IColumnInfo> Columns
        {
            get { return _columns; }
        }


        public override string ToString()
        {
            if (Columns == null || Columns.Count == 0)
                return base.ToString();
            
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0} (", Table);
            
            foreach (var item in Columns)
            {
                sb.AppendFormat("{0}, ",item.ColumnName);
            }
            sb.Length -= 2;
            sb.Append(")");
            return sb.ToString();
        }
        #endregion
    }
}
