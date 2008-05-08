﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using LinFu.Database.Implementation;
using System.Data;
using Simple.IoC;
using Simple.IoC.Loaders;
using System.IO;
namespace LinFu.Database.Tests
{
    [TestFixture]
    public class DatabaseTests :BaseFixture
    {
        private IContainer _container; 
        
        public override void Setup()
        {
            _container = new SimpleContainer();
            var directory = Path.GetDirectoryName(typeof(DatabaseTests).Assembly.Location);
            Loader loader = new Loader(_container);
            loader.LoadDirectory(directory, "*.dll");
        }

        public override void TearDown()
        {            
            _container = null;
        }

        [Test]
        public void ConnectionRepositoryReader()
        {
            IConnectionRepositoryStorage reader = _container.GetService<IConnectionRepositoryStorage>();
            IConnectionRepository repository = reader.Retrieve();
            repository.DefaultConnection = repository["Northwind"];
            Assert.IsNotNull(repository);            
        }

        [Test]
        public void GetDefaultConnection()
        {
            IConnectionRepositoryStorage reader = _container.GetService<IConnectionRepositoryStorage>();
            IConnectionRepository repository = reader.Retrieve();
            repository.DefaultConnection = repository["Northwind"];
            IConnection connection = _container.GetService<IConnection>();
            Assert.IsNotNull(connection);            
        }
        
        [Test]
        public void AutoLoadConnectionRepository()
        {
            IConnection connection = _container.GetService<IConnection>();
            Assert.IsNotNull(connection);
            
            IConnection connection2 = _container.GetService<IConnection>("Northwind");
            Assert.IsNotNull(connection2);
        }

        [Test]
        public void GetNamedConnection()
        {
            IConnectionRepositoryStorage reader = _container.GetService<IConnectionRepositoryStorage>();
            IConnectionRepository repository = reader.Retrieve();                        
            IConnection connection = _container.GetService<IConnection>("Northwind");
            Assert.IsNotNull(connection);
        }


        [Test]
        public void ExecuteDataTable()
        {
            IConnectionRepositoryStorage reader = _container.GetService<IConnectionRepositoryStorage>();
            IConnectionRepository repository = reader.Retrieve();
            repository.DefaultConnection = repository["Northwind"];

            IConnection connection = _container.GetService<IConnection>();
            DataTable datatable = connection.ExecuteDataTable("SELECT * FROM Customers");
            Assert.IsNotNull(datatable);
            Assert.IsTrue(datatable.Rows.Count > 0);

        }
        
        [Test]
        public void ExecuteScalar()
        {
            using (IConnection connection = _container.GetService<IConnection>())
            {
                var count = connection.ExecuteScalar<int>("SELECT Count(*) FROM Customers");
                Assert.IsTrue(count > 0);
            }
        }


        [Test]
        public void ExecuteReader()
        {
            using (IConnection connection = _container.GetService<IConnection>())
            {
                IDataReader reader = connection.ExecuteReader("SELECT * FROM Customers");
                Assert.IsNotNull(reader);
                while (reader.Read())
                {
                    Console.WriteLine(reader[0]);
                }
                reader.Close();
            }
        }

        [Test]
        public void DeriveParameters()
        {
            using (IConnection connection = _container.GetService<IConnection>())
            {
                IEnumerable<IDataParameter> parameters =  connection.Procedures["Ten Most Expensive Products"].Parameters;
                Assert.IsNotNull(parameters);
                Assert.IsTrue(parameters.Count() > 0);
            }
        }

        [Test]
        public void ExecuteDataTableUsingProcedure()
        {
            using (IConnection connection = _container.GetService<IConnection>())
            {
                DataTable datatable = connection.Procedures["Ten Most Expensive Products"].ExecuteDataTable();
                Assert.IsNotNull(datatable);
                Assert.IsTrue(datatable.Rows.Count > 0);
            }
        }
       
    }
}