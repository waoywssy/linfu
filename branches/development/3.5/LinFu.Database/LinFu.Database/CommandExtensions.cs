using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace LinFu.Database
{
    public static class CommandExtensions
    {

        public static void AddParameter(this IDbCommand command, Parameter parameter)
        {
            AddParameter(command, parameter.Name, parameter.Value,parameter.DbType, parameter.Direction);
        }

       
        public static void AddParameter(this IDbCommand command, string name, object value, DbType dbType, ParameterDirection direction)
        {
            IDataParameter parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value ?? DBNull.Value;
            parameter.Direction = direction;
            parameter.DbType = dbType;
            command.Parameters.Add(parameter);            
        }

        public static void AddParameters(this IDbCommand command, IEnumerable<Parameter> parameters)
        {
            foreach (var item in parameters)
            {
                AddParameter(command, item);
            }
        }


        

    }
}
