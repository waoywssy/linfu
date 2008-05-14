using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simple.IoC.Loaders;
using System.Data;
using LinFu.Database;
using LinFu.Persist.Metadata;

namespace LinFu.Persist.RowLoaders
{
    [Implements(typeof(IRowLoadStrategy), LifecycleType.OncePerRequest, ServiceName = "MultiRowLoadStrategy")]
    public class MultiRowLoadStrategy : BaseRowLoadStrategy
    {
        #region IRowLoadStrategy Members

        public override IEnumerable<IRow> Load(IEnumerable<IRowTaskItem> tasks)
        {
            if (tasks.Count() == 0)
                return null;
            
            IRowTaskItem firstTask = tasks.First();            
            ITableInfo tableInfo = Container.GetService<ITableInfoRepository>().Tables[firstTask.TableName];

            //Get the column info for the requested columns            
            var columns = from columnInfo in tableInfo.Columns
                          join columnName in firstTask.Columns on columnInfo.Value.LocalName equals columnName
                          select columnInfo.Value.ColumnName;

            string columnList = columns.Aggregate((current, next) => current + ", " + next);


            using (IConnection connection = Container.GetService<IConnection>())
            {
                IDbCommand command = connection.CreateCommand();
                int parameterCount = 0;
                StringBuilder whereClause = new StringBuilder(string.Format(" WHERE {0} IN( ", tableInfo.PrimaryKey.Columns.First().ColumnName));                
                foreach (var task in tasks)
                {
                    string parameterName = string.Format("@p{0}", parameterCount);
                    IDataParameter parameter = command.CreateParameter();
                    parameter.ParameterName = parameterName;
                    parameter.Value = task.PrimaryKeyValues.First().Value;
                    command.Parameters.Add(parameter);
                    whereClause.AppendFormat("{0}, ", parameterName);
                    parameterCount++;
                }
                whereClause.Length -= 2;
                whereClause.Append(")");

                string sql = string.Format("SELECT {0} FROM {1} {2}", columnList, tableInfo.TableName, whereClause);
                command.CommandText = sql;

                return CreateRows(connection.ExecuteReader(command), tasks.First().Columns);
            }

        }


        #endregion
    }
}
