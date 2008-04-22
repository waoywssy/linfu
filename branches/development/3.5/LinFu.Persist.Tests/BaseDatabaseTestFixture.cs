using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using NUnit.Framework;
using System.Data.SqlClient;
using System.Data;

namespace LinFu.Persist.Tests
{
    public abstract class BaseDatabaseTestFixture : BaseFixture 
    {
        private string _connectionString;
        protected BaseDatabaseTestFixture()
        {
            _connectionString = ConfigurationSettings.AppSettings["DbConnectionString"];
        }        
        [Test]
        public void ShouldBeAbleToConnectToDb()
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();
            Assert.IsTrue(connection.State == ConnectionState.Open);
            connection.Close();
        }
        protected string ConnectionString
        {
            get { return _connectionString; }
        }
    }
}
