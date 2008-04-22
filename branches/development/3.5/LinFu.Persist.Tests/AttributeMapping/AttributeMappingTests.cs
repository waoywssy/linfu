using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Core;
using NUnit.Framework;
using LinFu.Persist.Tests.AttributeMapping.SampleDomain;
using Simple.IoC;
using Simple.IoC.Loaders;
using System.IO;

namespace LinFu.Persist.Tests
{
    [TestFixture]
    public class AttributeMappingTests : BaseFixture
    {
        private SimpleContainer container;
        
        public override void Setup()
        {
            container = new SimpleContainer();
            var directory = Path.GetDirectoryName(typeof(AttributeMappingTests).Assembly.Location);            
            Loader loader = new Loader(container);
            loader.LoadDirectory(directory, "*.dll");
        }        
        public override void TearDown()
        {
            container = null;
        }
        [Test]
        public void ShouldDeriveDefaultTableNameFromPersistableType()
        {
            Type targetType = typeof(Order);
            IDeriveTableName derive = container.GetService<IDeriveTableName>();            
            Assert.IsNotNull(derive);
            string tableName = derive.GetTableName(targetType);
            Assert.AreEqual("Orders", tableName);
        }
        [Test]
        public void ShouldReturnBlankTableNameFromNonPersistableType()
        {
            Type targetType = typeof(NonPersistableOrder);
            IDeriveTableName derive = container.GetService<IDeriveTableName>();
            Assert.IsNotNull(derive);
            string tableName = derive.GetTableName(targetType);
            Assert.IsEmpty(tableName);
        }
        [Test]
        public void ShouldDeriveTableNameFromPersistableType()
        {
            Type targetType = typeof(OrderMappedToSpecificTable);
            IDeriveTableName derive = container.GetService<IDeriveTableName>();
            Assert.IsNotNull(derive);
            string tableName = derive.GetTableName(targetType);
            Assert.AreEqual("AnotherOrdersTable", tableName);            
        }
    }
}
