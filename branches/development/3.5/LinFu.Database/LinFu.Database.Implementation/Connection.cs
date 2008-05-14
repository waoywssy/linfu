﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;
using LinFu.DynamicProxy;
using Simple.IoC;
using Simple.IoC.Loaders;

namespace LinFu.Database.Implementation
{
    [Implements(typeof(IConnection), LifecycleType.OncePerRequest)]
    public class Connection : IConnection,IInitialize
    {

        #region Private Fields
        
        private bool _disposed = false;
        private ITransaction _transaction;        
        private IContainer _container;

        #endregion

        #region Private Methods
        private static void PostProcessParameterizedQuery(IDbCommand command,IEnumerable<Parameter> parameters)
        {
            var outParameters = parameters.Where(p => p.Direction == ParameterDirection.Output || p.Direction == ParameterDirection.InputOutput);
            foreach (var parameter in outParameters)
            {
                parameter.Value = ((IDataParameter)command.Parameters[parameter.Name]).Value;
            }            
        }


        #endregion

        #region Constructors

        public Connection()
        {
            IsolationLevel = IsolationLevel.ReadCommitted;
            CommandTimeout = 30;
        }

        #endregion

        #region IInitialize Members

        public void Initialize(IContainer container)
        {
            _transaction = container.GetService<ITransaction>();
            _transaction.Connection = this;
            Procedures = container.GetService<IProcedures>();
            Procedures.Connection = this;
            Procedures.Transaction = _transaction;
            _container = container;
        }

        #endregion

        #region IConnection Members

        public string ConnectionString{get;set;}

        public string ProviderName { get; set; }

        public DbProviderFactory ProviderFactory{get;set;}
        
        public IProcedures Procedures{get;private set;}

        public IsolationLevel IsolationLevel { get; set; }

        public int CommandTimeout { get; set; }

        public bool FillSchema { get; set; }

        public bool SupportsBulkLoading
        {
            get 
            {
                try
                {
                    _container.GetService<IBulkLoader>(ProviderName);
                }
                catch(ServiceNotFoundException)
                {
                    return false;
                }
                return true;
            }            
        }      

        public DataTable ExecuteDataTable(IDbCommand command)
        {
            _transaction.BeginTransaction(command);
            IDbDataAdapter dataAdapter = ProviderFactory.CreateDataAdapter();          
            dataAdapter.SelectCommand = command;
            DataSet dataSet = new DataSet();
            if (FillSchema)
                dataAdapter.FillSchema(dataSet, SchemaType.Source);
            dataAdapter.Fill(dataSet);
            return dataSet.Tables[0];
        }

        public IDataReader ExecuteReader(IDbCommand command)
        {
            _transaction.BeginTransaction(command);
            return command.ExecuteReader();
        }

        public int ExecuteNonQuery(IDbCommand command)
        {
            _transaction.BeginTransaction(command);
            return command.ExecuteNonQuery();
        }

        public T ExecuteScalar<T>(IDbCommand command)
        {
            _transaction.BeginTransaction(command);
            return (T)command.ExecuteScalar();
        }

        public DataTable ExecuteDataTable(string commandText)
        {
            return ExecuteDataTable(CreateCommand(commandText));
        }

        public IDataReader ExecuteReader(string commandText)
        {
            return ExecuteReader(CreateCommand(commandText));
        }       

        public int ExecuteNonQuery(string commandText)
        {
            return ExecuteNonQuery(CreateCommand(commandText));
        }

        public T ExecuteScalar<T>(string commandText)
        {
            return ExecuteScalar<T>(CreateCommand(commandText));
        }


        public DataTable ExecuteDataTable(string commandText, params Parameter[] parameters)
        {
            IDbCommand command = CreateCommand(commandText);
            command.AddParameters(parameters);
            DataTable result = ExecuteDataTable(command);
            PostProcessParameterizedQuery(command, parameters);
            return result;
        }

        public IDataReader ExecuteReader(string commandText, params Parameter[] parameters)
        {
            IDbCommand command = CreateCommand(commandText);
            command.AddParameters(parameters);
            IDataReader result = ExecuteReader(command);
            PostProcessParameterizedQuery(command, parameters);
            return result;
        }

        public int ExecuteNonQuery(string commandText, params Parameter[] parameters)
        {
            IDbCommand command = CreateCommand(commandText);
            command.AddParameters(parameters);
            int result = ExecuteNonQuery(command);
            PostProcessParameterizedQuery(command, parameters);
            return result;
        }

        public T ExecuteScalar<T>(string commandText,params Parameter[] parameters)
        {
            IDbCommand command = CreateCommand(commandText);
            command.AddParameters(parameters);
            T result = ExecuteScalar<T>(command);
            PostProcessParameterizedQuery(command, parameters);
            return result;
        }

        public void BulkLoad(DataTable dataTable)
        {
            IBulkLoader bulkLoader = null;
            try
            {
                bulkLoader = _container.GetService<IBulkLoader>(ProviderName);
            }
            catch (ServiceNotFoundException)
            {
                throw new DatabaseException(string.Format("Bulkload is not supported by this provider ({0})",ProviderName));
            }                                                        
            _transaction.BeginTransaction(bulkLoader);
            bulkLoader.Load(dataTable);
        }

        public IDbCommand CreateCommand()
        {
            IDbCommand command = ProviderFactory.CreateCommand();
            command.CommandTimeout = CommandTimeout;
            return command;
        }

        public IDbCommand CreateCommand(string commandText)
        {
            IDbCommand command = CreateCommand();
            command.CommandText = commandText;
            return command;
        }

        #endregion

        #region IDisposable Members

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _transaction.FinalizeTransaction();
                    _disposed = true;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Connection()
        {
            Dispose(false);
        }

        #endregion

    }
}
