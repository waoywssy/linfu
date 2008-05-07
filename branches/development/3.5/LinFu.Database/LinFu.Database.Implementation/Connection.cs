using System;
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

        #endregion

        #region Constructors

        public Connection()
        {
            IsolationLevel = IsolationLevel.ReadCommitted;
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
        }

        #endregion

        #region IConnection Members

        public string ConnectionString{get;set;}
        
        public DbProviderFactory ProviderFactory{get;set;}
        
        public IProcedures Procedures{get;private set;}

        public IsolationLevel IsolationLevel { get; set; }

        public bool SupportsBulkLoading
        {
            get {return BulkLoader == null;}            
        }

        public IBulkLoader BulkLoader { get; set; }

        public DataTable ExecuteDataTable(IDbCommand command)
        {
            _transaction.BeginTransaction(command);
            IDbDataAdapter dataAdapter = ProviderFactory.CreateDataAdapter();
            dataAdapter.SelectCommand = command;
            DataSet dataSet = new DataSet();
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

        public IDbCommand CreateCommand()
        {
            return ProviderFactory.CreateCommand();
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