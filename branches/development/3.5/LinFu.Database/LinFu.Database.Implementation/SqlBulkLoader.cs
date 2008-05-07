using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Simple.IoC.Loaders;

namespace LinFu.Database.Implementation
{
    [Implements(typeof(IBulkLoader),LifecycleType.OncePerRequest,ServiceName = "System.Data.SqlClient")]
    public class SqlBulkLoader : IBulkLoader
    {
        #region IBulkLoader Members

        public void Load(DataTable table)
        {
            
        }

        public IConnection Connection {get;set;}
        
        #endregion
    }
}
