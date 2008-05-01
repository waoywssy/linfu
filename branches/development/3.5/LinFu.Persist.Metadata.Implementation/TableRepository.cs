using System;
using System.Collections.Generic;
using LinFu.Persist.Metadata;
using Simple.IoC.Loaders;
namespace LinFu.Persist.Metadata.Implementation
{
    [Serializable]
    [Implements(typeof(ITableRepository),LifecycleType.OncePerRequest)]
    public class TableRepository : ITableRepository
    {
        private readonly IDictionary<string, ITableInfo> _tables = new Dictionary<string, ITableInfo>();

        #region ITableRepository Members

        public string Name { get; set; }

        public IDictionary<string, ITableInfo> Tables
        {
            get { return _tables; }
        }

        #endregion

      
    }
}
