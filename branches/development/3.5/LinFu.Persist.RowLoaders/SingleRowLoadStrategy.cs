﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simple.IoC.Loaders;
using System.Data;

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

            IDbCommand command = CreateCommand(tasks.Single());      
             
            return CreateRows(command, tasks.Single().Columns);
        }


        #endregion

        #region Private Methods
        
        private IDbCommand CreateCommand(IRowTaskItem task)
        {
            IDbCommand command = Connection.CreateCommand();
            command.CommandText = CreateSelectStatement(task);
            foreach (var primaryKey in task.PrimaryKeyValues)
            {
                IDbDataParameter parameter = command.CreateParameter();
                parameter.ParameterName = string.Format("@{0}",primaryKey.Key);                
                parameter.Value = primaryKey.Value;
                command.Parameters.Add(parameter);
            }
            return command;
        }

        private static string CreateSelectStatement(IRowTaskItem task)
        {
            string columnList = task.Columns.Aggregate((current,next) => current + ", " + next) ;
          
            string whereClause = task.PrimaryKeyValues.Select(p => string.Format("{0} = @{0}", p.Key))
                .Aggregate((current, next) => current + " AND " + next);
            
            return string.Format("SELECT {0} FROM {1} WHERE {2}", columnList, 
                task.TableName, whereClause);
        }

       

        
        #endregion

    }
}
