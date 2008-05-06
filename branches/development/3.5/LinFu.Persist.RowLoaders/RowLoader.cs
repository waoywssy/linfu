using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Simple.IoC.Loaders;
using Simple.IoC;

namespace LinFu.Persist
{
    [Implements(typeof(IRowLoader),LifecycleType.OncePerRequest)]
    public class RowLoader : IRowLoader, IInitialize
    {
        private IDbConnection _connection = new SqlConnection("Data Source=.;Initial Catalog=RBUTV;Integrated Security=True");
        private IContainer _container = null;
        private IRowLoadStrategy _singleLoadStrategy = null;
        private IRowLoadStrategy _multiRowStrategy = null;

        #region IInitialize Members

        public void Initialize(IContainer container)
        {
            _container = container;
            _singleLoadStrategy = container.GetService<IRowLoadStrategy>("SingleRowLoadStrategy");
            _multiRowStrategy = container.GetService<IRowLoadStrategy>("MultiRowLoadStrategy");
        }

        #endregion


        #region IRowLoader Members

        public IEnumerable<IRow> CreateRows(IEnumerable<IRowTaskItem> tasks)
        {
            IRowLoadStrategy loadStrategy = null;

            if (tasks.Count() == 0)
                return null;

            if (tasks.Count() == 1)
                //Load a single row
                loadStrategy = _singleLoadStrategy;
            else
                //Load multible rows
                loadStrategy = _multiRowStrategy;

            return loadStrategy.Load(tasks);
            
        }

        public IEnumerable<IRow> CreateRowsUsingBatchQueries(IEnumerable<IRowTaskItem> tasks)
        {
            StringBuilder sb = new StringBuilder();




            var query = from task in tasks.GroupBy(t => t.TableName) select task;

            foreach (var item in tasks)
            {
                
                string columns = CreateColumnList(item.Columns);

                string sql = string.Format("SELECT {0} FROM {1} WHERE {2};",CreateColumnList(item.Columns),item.TableName,CreateWhereClause(item.PrimaryKeyValues));
                sb.AppendLine(sql);
            }

            _connection.Open();
            IDbCommand command =  _connection.CreateCommand();
            command.CommandText = sb.ToString();
            IDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                
            }


            _connection.Close();
            return null;
        }


        public IEnumerable<IRow> CreateRowsUsingTempTable(IEnumerable<IRowTaskItem> tasks)
        {

            _connection.Open();
            IDbCommand tmpCommand = _connection.CreateCommand();
            tmpCommand.CommandText = "CREATE TABLE #tmp (StockGUID UNIQUEIDENTIFIER )";
            tmpCommand.ExecuteNonQuery();        
            

            IDbCommand insertCommand = _connection.CreateCommand();
            
            IDbDataParameter parameter =  insertCommand.CreateParameter();
            parameter.ParameterName = "@StockGUID";
                        
            insertCommand.CommandText = "INSERT INTO #tmp VALUES(@StockGUID)";
            insertCommand.Parameters.Add(parameter);
            

            foreach (var item in tasks)
            {
                ((IDataParameter)insertCommand.Parameters["@StockGUID"]).Value = item.PrimaryKeyValues.First().Value;
                insertCommand.ExecuteNonQuery();                
            }

            
            return null;
        }


        public IEnumerable<IRow> CreateRowsUsingBulkLoad(IEnumerable<IRowTaskItem> tasks)
        {

            _connection.Open();
            IDbCommand tmpCommand = _connection.CreateCommand();
            tmpCommand.CommandText = "CREATE TABLE #tmp (StockGUID UNIQUEIDENTIFIER )";
            tmpCommand.ExecuteNonQuery();


            DataTable dataTable = new DataTable("#tmp");
            dataTable.Columns.Add("StockGUID", typeof(Guid));






            Stopwatch w = new Stopwatch();
            w.Start();
            foreach (var item in tasks)
            {
                dataTable.Rows.Add((Guid)item.PrimaryKeyValues.First().Value);                                
            }

            
            SqlBulkCopy sqlBulkCopy = new SqlBulkCopy((SqlConnection)_connection);
            sqlBulkCopy.DestinationTableName = "#tmp";
            sqlBulkCopy.WriteToServer(dataTable);

            w.Stop();
            Console.WriteLine(w.ElapsedMilliseconds.ToString());
            IDbCommand command = _connection.CreateCommand();
            command.CommandText = "SELECT * FROM Stock INNER JOIN #tmp ON Stock.StockGUID = #tmp.StockGUID";
            IDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                IRow row = new Row();
                //foreach (var item in collection)
                //{
                    
                //}
            }

            return null;
        }

        private static string CreateWhereClause(IEnumerable<KeyValuePair<string, object>> primaryKeys)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in primaryKeys)
            {
                sb.AppendFormat("{0} = '{1}' AND",item.Key,item.Value);
            }
            sb.Length -= 4;
            return sb.ToString();
        }


        private static string CreateColumnList(IEnumerable<string> columns)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in columns)
	        {
                sb.AppendFormat("{0}, ", item);
	        }
            sb.Length -= 2;

            return sb.ToString();
        }


        #endregion

        
    }
}
