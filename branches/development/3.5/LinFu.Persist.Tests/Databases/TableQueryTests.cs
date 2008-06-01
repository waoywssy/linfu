using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using Simple.IoC;
using Simple.IoC.Loaders;

namespace LinFu.Persist.Tests.Databases
{
    [TestFixture]
    public class TableQueryTests : BaseDatabaseTestFixture 
    {
        private IContainer _container;
        private SqlConnection _connection;
        public override void Setup()
        {
            _connection = new SqlConnection();
            _connection.ConnectionString = ConfigurationSettings.AppSettings["NorthwindConnectionString"];

            _container = new SimpleContainer();
            Loader loader = new Loader(_container);
            loader.LoadDirectory(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
        }
        public override void TearDown()
        {
            if (_connection != null)
            {
                if (_connection.State != ConnectionState.Closed)
                    _connection.Close();

                _connection.Dispose();
            }
            _connection = null;
            _container = null;
        }
        
    }
}
