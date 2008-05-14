using System;
using System.Collections.Generic;
using LinFu.Persist.Metadata;
namespace LinFu.Persist.Metadata.Implementation
{
    [Serializable]
    public class TableInfo : ITableInfo
    {
        private readonly IDictionary<string, IColumnInfo> _columns = new Dictionary<string, IColumnInfo>();
        private readonly IList<IRelationInfo> _relations = new List<IRelationInfo>();

        #region ITableInfo Members

        public string TableName { get; set; }
        public string LocalName { get; set; }
        public string SchemaName { get; set; }
        public IKeyInfo PrimaryKey { get; set; }

        public IList<IRelationInfo> Relations
        {
            get { return _relations; }
        }       

        public IDictionary<string, IColumnInfo> Columns
        {
            get { return _columns; }
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(LocalName))
                return base.ToString();

            return TableName;
        }
        
        #endregion
    }
}
