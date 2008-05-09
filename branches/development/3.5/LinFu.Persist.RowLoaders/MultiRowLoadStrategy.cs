using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simple.IoC.Loaders;
using System.Data;
using LinFu.Database;

namespace LinFu.Persist.RowLoaders
{
    [Implements(typeof(IRowLoadStrategy), LifecycleType.OncePerRequest, ServiceName = "MultiRowLoadStrategy")]
    public class MultiRowLoadStrategy : BaseRowLoadStrategy
    {
        #region IRowLoadStrategy Members

        public override IEnumerable<IRow> Load(IEnumerable<IRowTaskItem> tasks)
        {
            using (IConnection connection = Container.GetService<IConnection>())
            {
                IDbCommand command = connection.CreateCommand();
                int parameterCount = 0;
                StringBuilder whereClause = new StringBuilder(string.Format(" WHERE {0} IN( ", tasks.First().PrimaryKeyValues.First().Key));
                string columnList = tasks.First().Columns.Aggregate((current, next) => current + ", " + next);
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

                string sql = string.Format("SELECT {0} FROM {1} {2}", columnList, tasks.First().TableName, whereClause);
                command.CommandText = sql;

                return CreateRows(connection.ExecuteReader(command), tasks.First().Columns);
            }

        }


        #endregion
    }
}
