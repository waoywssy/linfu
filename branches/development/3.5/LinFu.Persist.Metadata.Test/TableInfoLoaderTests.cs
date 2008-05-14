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
        public void DefaultRepository()
        {            
            
            ITableInfoRepository defaultRepository = _container.GetService<ITableInfoRepository>();
            Assert.IsNotNull(defaultRepository);

            var tableInfo = defaultRepository.Tables.Values.First();
            Assert.IsNotNull(tableInfo.LocalName);
            Assert.IsNotEmpty(tableInfo.LocalName);
        }
    }
}
