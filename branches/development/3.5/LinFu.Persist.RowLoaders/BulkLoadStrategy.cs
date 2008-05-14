using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simple.IoC.Loaders;
using Simple.IoC;
using System.Data;
using LinFu.Database;
using LinFu.Persist.Metadata;

namespace LinFu.Persist
{
    [Implements(typeof(IRowLoadStrategy), LifecycleType.OncePerRequest, ServiceName = "BulkLoadRowStrategy")]
    public class BulkLoadStrategy : BaseRowLoadStrategy
    {
        
        public override IEnumerable<IRow> Load(IEnumerable<IRowTaskItem> tasks)
        {

            IRowTaskItem firstTask = tasks.First();
            ITableInfo tableInfo = Container.GetService<ITableInfoRepository>().Tables[firstTask.TableName];

            
            //Get the name of the table to load
            string tableName = tableInfo.TableName;

            //Get the name of the temporary key table
            string keyTableName = GetKeyTableName(tableName);

            using (IConnection connection = Container.GetService<IConnection>())
            {

                // Create the temporary key table
                CreateKeyTable(connection, tableInfo, keyTableName);

                // Create a datatable to be the source of bulkloading
                DataTable keyTable = CreateDataTable(tableInfo, keyTableName);

                // Add the primary key values to the table
                foreach (var item in tasks)
                {
                    keyTable.Rows.Add(item.PrimaryKeyValues.Select(p => p.Value).ToArray());
                }

                // Bulkload the keys up to the server            
                connection.BulkLoad(keyTable);


                //Get the column info for the requested columns            
                var columns = from columnInfo in tableInfo.Columns
                              join columnName in firstTask.Columns on columnInfo.Value.LocalName equals columnName
                              select string.Format("t0.{0}",columnInfo.Value.ColumnName);

                string columnList = columns.Aggregate((current, next) => current + ", " + next);



                // Create the join argument list 
                string joinArgument = tableInfo.PrimaryKey.Columns.Select(p => string.Format("t0.{0} = {1}.{0}", p.ColumnName, keyTableName))
                    .Aggregate((current, next) => current + " AND " + next);

                // Construct the final sql statement
                string sql = string.Format("SELECT {0} FROM {1} AS t0 INNER JOIN {2} ON {3}", columnList, tableName, keyTableName, joinArgument);

                IDbCommand command = connection.CreateCommand(sql);
                return CreateRows(connection.ExecuteReader(command), tasks.First().Columns);
            }
        }

        private DataTable CreateDataTable(ITableInfo tableInfo, string keyTableName)
        {
            DataTable dataTable = new DataTable(keyTableName);
            foreach (var item in tableInfo.PrimaryKey.Columns)
            {
                dataTable.Columns.Add(item.LocalName,item.DataType);
            }
            return dataTable;
        }

        private void CreateKeyTable(IConnection connection, ITableInfo tableInfo, string keyTableName)
        {
            
            string keyColumns = tableInfo.PrimaryKey.Columns.Select(c => c.ColumnName)
                .Aggregate((current,next) => current + ", "+  next);
            
            

            string sql = string.Format("SELECT {0} INTO {1} FROM {2} WHERE 1 = 2",
                keyColumns, keyTableName, tableInfo.TableName);
            connection.ExecuteNonQuery(sql);
        }

        private static string GetKeyTableName(string tableName)
        {
            return string.Format("#tmp{0}Keys", 
                tableName.Substring(tableName.LastIndexOf(".") + 1).Replace("[", "").Replace("]", "").Replace(" ", ""));
        }
    }
}
