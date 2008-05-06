using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Simple.IoC.Loaders;
using System.Data.SqlClient;

namespace LinFu.Persist
{
    [Implements(typeof(IBulkLoader),LifecycleType.OncePerRequest,ServiceName = "SqlBulkLoader")]
    public class SqlBulkLoader : IBulkLoader
    {
        #region IBulkLoader Members

        public IDbConnection Connection{get;set;}
        

        public void Load(DataTable dataTable)
        {
            SqlBulkCopy sqlBulkCopy = new SqlBulkCopy((SqlConnection)Connection);
            sqlBulkCopy.DestinationTableName = dataTable.TableName;
            sqlBulkCopy.WriteToServer(dataTable);
        }

        #endregion
    }
}
