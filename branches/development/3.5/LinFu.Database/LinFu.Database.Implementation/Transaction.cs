using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.DynamicProxy;
using System.Data;
using Simple.IoC.Loaders;
using System.Diagnostics;

namespace LinFu.Database.Implementation
{
    [Implements(typeof(ITransaction),LifecycleType.OncePerRequest)]
    public class Transaction : ITransaction
    {        
        private IDbTransaction _transaction;
        private IDbConnection _dbConnection;
        private bool _isTransactableMethod = false;
        private bool performRollback = false;       

        public IConnection Connection {get;set;}

       
        public void BeginTransaction(IDbCommand command)
        {
            BeginTransaction();
            command.Connection = _dbConnection;
            command.Transaction = _transaction;
        }

        public void BeginTransaction(IBulkLoader bulkLoader)
        {
            BeginTransaction();
            bulkLoader.Connection = _dbConnection;
            bulkLoader.Transaction = _transaction;
        }

        private void BeginTransaction()
        {
            if (_transaction == null)
            {
                _dbConnection = Connection.ProviderFactory.CreateConnection();
                _dbConnection.ConnectionString = Connection.ConnectionString;
                _dbConnection.Open();
                _transaction = _dbConnection.BeginTransaction(Connection.IsolationLevel);
            }
        }

        public void FinalizeTransaction()
        {
            Console.WriteLine("FinalizeTransaction");
            if (performRollback)
                RollbackTransaction();
            else
                CommitTransaction();
            if (_dbConnection != null)
            {
                _dbConnection.Close();
                _dbConnection.Dispose();
            }
        }


        private void CommitTransaction()
        {
            if (_transaction != null)
            {
                _transaction.Commit();
                _transaction.Dispose();
                _transaction = null;
            }
            
            
        }

        private void RollbackTransaction()
        {
            if (_transaction != null)
            {
                _transaction.Rollback();
                _transaction.Dispose();
                _transaction = null;
            }            
        }

        private void CloseConnection()
        {            
           
        }
    }
}