using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Simple.IoC;
using System.IO;
using Simple.IoC.Loaders;
using LinFu.Persist.Metadata;
namespace LinFu.Persist.Metadata.Test
{
    [TestFixture]
    public class TableInfoLoaderTests :BaseFixture
    {
        private IContainer _container;
        
        public override void Setup()
        {
            _container = new SimpleContainer();
            var directory = Path.GetDirectoryName(typeof(TableInfoLoaderTests).Assembly.Location);
            Loader loader = new Loader(_container);
            loader.LoadDirectory(directory, "*.dll");
        }

        public override void TearDown()
        {
            _container = null;
            base.TearDown();
        }

        [Test]
        public void DbTableRepositoryLoader()
        {
            ITableRepositoryLoader loader = _container.GetService<IDbTableRepositoryLoader>();            
            ITableRepository repository = loader.Load("Northwind");
            Assert.IsTrue(repository.Tables.Count > 0);
        }

        [Test]
        public void DefaultTableRepositoryPersister()
        {
            ITableRepositoryLoader loader = _container.GetService<IDbTableRepositoryLoader>();
            ITableRepository repository = loader.Load("Northwind");
            Assert.IsTrue(repository.Tables.Count > 0);

            ITableRepositoryPersister persister = _container.GetService<ITableRepositoryPersister>();
            persister.Save(repository);

        }

        [Test]
        public void CreateITableFromRepository()
        {
            ITableRepositoryLoader loader = _container.GetService<IDbTableRepositoryLoader>();
            ITableRepository repository = loader.Load("Northwind");
            Assert.IsTrue(repository.Tables.Count > 0);

            ITableFactory factory = new TableFactory();
            factory.Repository = repository;
            ITable ordersTable = factory.CreateTable("dbo.Orders");

        }

      
    }
}
