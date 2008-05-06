using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simple.IoC.Loaders;
using Simple.IoC;
using System.Data;

namespace LinFu.Persist
{
    [Implements(typeof(IRowLoadStrategy), LifecycleType.OncePerRequest, ServiceName = "MultiRowLoadStrategy")]
    public class MultiRowLoadStrategy : BaseRowLoadStrategy, IInitialize
    {
        private IBulkLoader bulkLoader = null;

        public void Initialize(IContainer container)
        {
            //HACK This service should be provided by the ADO.NET api wrapper
            bulkLoader = container.GetService<IBulkLoader>("SqlBulkLoader");
        }

        public override IEnumerable<IRow> Load(IEnumerable<IRowTaskItem> tasks)
        {
            if (Connection.State != ConnectionState.Open)
                Connection.Open();

            //Get the name of the table to load
            string tableName = tasks.First().TableName;

            //Get the name of the temporary key table
            string keyTableName = GetKeyTableName(tasks.First());

            //Create the temporary key table
            CreateKeyTable(tasks.First(), keyTableName);

            //Create a datatable to be the source of bulkloading
            DataTable keyTable = CreateDataTable(tasks.First(), keyTableName);

            //Add the primary key values to the table
            foreach (var item in tasks)
                keyTable.Rows.Add(item.PrimaryKeyValues.Select(p => p.Value).ToArray());

            //Bulkload the keys up to the server
            bulkLoader.Connection = Connection;
            bulkLoader.Load(keyTable);


            //Create the projection list
            string columnList = tasks.First().Columns.Select(c => string.Format("t0.{0}", c))
                .Aggregate((current, next) => current + ", " + next);

            //Create the join argument list 
            string joinArgument = tasks.First().PrimaryKeyValues.Select(p => string.Format("t0.{0} = {1}.{0}", p.Key, keyTableName))
                .Aggregate((current, next) => current + "AND" + next);

            //Construct the final sql statement
            string sql = string.Format("SELECT {0} FROM {1} AS t0 INNER JOIN {2} ON {3}", columnList, tableName, keyTableName, joinArgument);

            IDbCommand command = Connection.CreateCommand();
            command.CommandText = sql;
            return CreateRows(command, tasks.First().Columns);
        }

        private DataTable CreateDataTable(IRowTaskItem task, string keyTableName)
        {
            DataTable dataTable = new DataTable(keyTableName);
            foreach (var item in task.PrimaryKeyValues)
            {
                dataTable.Columns.Add(item.Key, item.Value.GetType());
            }
            return dataTable;
        }

        private void CreateKeyTable(IRowTaskItem task, string keyTableName)
        {
            var primaryKeyColumns = from column in task.PrimaryKeyValues select column.Key;
            string columnList = task.PrimaryKeyValues.Select(p => p.Key).Aggregate((current, next) => current + ", " + next);

            string sql = string.Format("SELECT {0} INTO {1} FROM {2} WHERE 1 = 2",
                columnList, keyTableName, task.TableName);
            IDbCommand command = Connection.CreateCommand();
            command.CommandText = sql;
            command.ExecuteNonQuery();
            command.Dispose();
        }

        private static string GetKeyTableName(IRowTaskItem task)
        {
            string tableName = task.TableName.Substring(task.TableName.LastIndexOf(".") + 1)
                .Replace("[", "").Replace("]", "");
            return string.Format("#tmp{0}Keys", tableName);
        }
    }
}
