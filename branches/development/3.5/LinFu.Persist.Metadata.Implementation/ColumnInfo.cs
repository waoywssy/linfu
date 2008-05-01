using System;
using LinFu.Persist.Metadata;
using Simple.IoC.Loaders;

namespace LinFu.Persist.Metadata.Implementation
{
    [Serializable]
    [Implements(typeof(IColumnInfo),LifecycleType.OncePerRequest)]
    public class ColumnInfo : IColumnInfo
    {

        #region IColumnInfo Members

        public ITableInfo Table { get; set; }

        public string ColumnName { get; set; }

        public Type DataType { get; set; }

        #endregion

        public override string ToString()
        {
            if (ColumnName != null && DataType != null)
                return string.Format("{0}({1})", ColumnName, DataType.Name);
            
            return base.ToString();
        }
    }
}
