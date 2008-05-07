using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Simple.IoC;
using System.Data.Common;
using System.Reflection;
using Simple.IoC.Loaders;

namespace LinFu.Database.Implementation
{
    [Implements(typeof(IProcedure),LifecycleType.OncePerRequest)]
    public class Procedure : IProcedure, IInitialize
    {


        #region IProcedure Members

        IParameterCache _parameterCache;

        public IConnection Connection {get;set;}

        private IEnumerable<IDataParameter> _parameters;

        public string Name {get;set;}
        

        #endregion

        private void FillParameters(IDbCommand command, params object[] parameterValues)
        {
            

        }

        private IEnumerable<IDataParameter> GetParameters()
        {
            if (_parameters != null)
                return _parameters;
            
            if (!_parameterCache.HasCachedParameters(ParameterKey))
                DeriveParameters();

            return _parameterCache.GetParameters(ParameterKey);
        }

        private string ParameterKey
        {
            get { return string.Format("{0}.{1}", Connection.ConnectionString, Name); }
        }


        private IDbCommand CreateCommand(params object[] parameters)
        {
            IDbCommand command = Connection.CreateCommand();            
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = Name;
                       
            foreach (var item in Parameters)
            {
                item.Value = DBNull.Value;
                command.Parameters.Add(item);
            }

            //Fill the parameters
            int parameterCount = command.Parameters.Count - 1 < parameters.Length ?
                command.Parameters.Count - 1 : parameters.Length;

            for (int i = 1; i <= parameterCount; i++)
            {
                command.Parameters[i] = parameters[i - 1] == null ? DBNull.Value : parameters[i - 1];
            }
            return command;
        }


        public IDataReader ExecuteReader(params object[] parameters)
        {
            return Connection.ExecuteReader(CreateCommand(parameters));            
        }

        public DataTable ExecuteDataTable(params object[] parameters)
        {
            return Connection.ExecuteDataTable(CreateCommand(parameters));
        }

        public int ExecuteNonQuery(params object[] parameters)
        {
            return Connection.ExecuteNonQuery(CreateCommand(parameters));
        }

        public T ExecuteScalar<T>(params object[] parameters)
        {
            return Connection.ExecuteScalar<T>(CreateCommand(parameters));
        }

        

        #region IProcedure Members


        public IEnumerable<IDataParameter> Parameters
        {
            get 
            {
                return GetParameters();
            }
        }

        #endregion

        

        private void DeriveParameters()
        {
            //Create a new connection to perform the parameter derivation.
            IDbConnection connection = Connection.ProviderFactory.CreateConnection();
            connection.ConnectionString = Connection.ConnectionString;
            connection.Open();
            
            //Create a new command object to 
            IDbCommand command = Connection.ProviderFactory.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = Name;
            command.Connection = connection;

            
            DbCommandBuilder commandBuilder = Connection.ProviderFactory.CreateCommandBuilder();
            Type commandBuilderType = commandBuilder.GetType();
            
            //Get a reference to the static "DeriveParameters" method
            MethodInfo methodInfo = commandBuilderType.GetMethod("DeriveParameters");
          
            methodInfo.Invoke(null, new object[1] { command });

            //Cache the parameters for later use
            _parameterCache.CacheParameters(string.Format("{0}.{1}", connection.ConnectionString, command.CommandText),
                command.Parameters.Cast<IDataParameter>());

            connection.Close();
            command.Dispose();
            connection.Dispose();
        }
        

        #region IInitialize Members

        public void Initialize(IContainer container)
        {
            _parameterCache = container.GetService<IParameterCache>();
        }

        #endregion






    }
}