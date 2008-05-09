using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Simple.IoC.Loaders;
using System.Data.SqlClient;

namespace LinFu.Database.Implementation
{
    [Implements(typeof(IBulkLoader),LifecycleType.OncePerRequest,ServiceName = "System.Data.SqlClient")]
    public class SqlBulkLoader : IBulkLoader
    {

        #region IBulkLoader Members

        public void Load(DataTable table)
        {
            SqlBulkCopyOptions copyOptions = SqlBulkCopyOptions.Default;

            //Remove the UseInternalTransaction flag since this is running within a transaction.
            copyOptions &= ~SqlBulkCopyOptions.UseInternalTransaction;
            
            SqlBulkCopy sqlBulkCopy = new SqlBulkCopy((SqlConnection)Connection,copyOptions,(SqlTransaction)Transaction);
            sqlBulkCopy.DestinationTableName = table.TableName;            
            sqlBulkCopy.WriteToServer(table);         
        }

        public IDbConnection Connection {get;set;}
        public IDbTransaction Transaction { get; set; }        
        #endregion
    }
}
