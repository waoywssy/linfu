using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simple.IoC.Loaders;
using System.Data;
using LinFu.Database;
using LinFu.Persist.Metadata;

namespace LinFu.Persist
{
    [Implements(typeof(IRowLoadStrategy),LifecycleType.OncePerRequest,ServiceName="SingleRowLoadStrategy")]
    public class SingleRowLoadStrategy : BaseRowLoadStrategy
    {

        #region IRowLoadStrategy Members

        
        
        public override IEnumerable<IRow> Load(IEnumerable<IRowTaskItem> tasks)
        {
            if (tasks.Count() != 1)
                return null;

            using(IConnection connection = Container.GetService<IConnection>())
            {
                IDbCommand command = CreateCommand(connection,tasks.Single());      
                
                return CreateRows(connection.ExecuteReader(command), tasks.Single().Columns);
            }
        }


        #endregion

        #region Private Methods
        
        private IDbCommand CreateCommand(IConnection connection, IRowTaskItem task)
        {
            ITableInfo tableInfo = Container.GetService<ITableInfoRepository>().Tables[task.TableName];
            
            IDbCommand command = connection.CreateCommand();
            command.CommandText = CreateSelectStatement(task,tableInfo);
            foreach (var primaryKey in task.PrimaryKeyValues)
            {
                IDbDataParameter parameter = command.CreateParameter();
                parameter.ParameterName = string.Format("@{0}",primaryKey.Key);                
                parameter.Value = primaryKey.Value;
                command.Parameters.Add(parameter);
            }
            return command;
        }

        private static string CreateSelectStatement(IRowTaskItem task, ITableInfo tableInfo)
        {
            //Get the column info for the requested columns            
            var columns = from columnInfo in tableInfo.Columns
                          join columnName in task.Columns on columnInfo.Value.LocalName equals columnName
                          select columnInfo.Value.ColumnName;

            string columnList = columns.Aggregate((current,next) => current + ", " + next) ;

            string whereClause = tableInfo.PrimaryKey.Columns.Select(c => string.Format("{0} = @{1}",c.ColumnName,c.LocalName))
                .Aggregate((current,next) => current + " AND " + next);
                        
            return string.Format("SELECT {0} FROM {1} WHERE {2}", columnList, 
                tableInfo.TableName, whereClause);
        }

       

        
        #endregion

    }
}
