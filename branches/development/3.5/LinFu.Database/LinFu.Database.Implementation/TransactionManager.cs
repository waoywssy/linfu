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


        public Transaction()
        {
            
        }

        public IConnection Connection {get;set;}

        //#region IInvokeWrapper Members

        //public void AfterInvoke(InvocationInfo info, object returnValue)
        //{
        //    //Do nothing
        //}

        //public void BeforeInvoke(InvocationInfo info)
        //{
        //    if (typeof(IDisposable).Equals(info.TargetMethod.DeclaringType))
        //    {
        //        CloseConnection();
        //    }

        //    _isTransactableMethod = (typeof(IExecutor).Equals(info.TargetMethod.DeclaringType)
        //        && typeof(IDbCommand).IsAssignableFrom(info.Arguments.First().GetType()));
        //    if (_isTransactableMethod)
        //        BeginTransaction(info.Arguments.First() as IDbCommand);
        //}

        //public object DoInvoke(InvocationInfo info)
        //{
        //    if (_isTransactableMethod)
        //    {
        //        try
        //        {
        //            return info.TargetMethod.Invoke(_executor, info.Arguments);
        //        }
        //        catch (Exception ex)
        //        {
        //            performRollback = true;
        //            throw new DatabaseException("Transaction is rolled back", ex);
        //        }
        //    }
        //    else
        //    {
        //        return info.TargetMethod.Invoke(_executor, info.Arguments);
        //    }
        //}

        public void BeginTransaction(IDbCommand command)
        {            
            if (_transaction == null)
            {
                _dbConnection = Connection.ProviderFactory.CreateConnection();
                _dbConnection.ConnectionString = Connection.ConnectionString;
                _dbConnection.Open();
                _transaction = _dbConnection.BeginTransaction(Connection.IsolationLevel);
            }
            command.Connection = _dbConnection;
            command.Transaction = _transaction;
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
