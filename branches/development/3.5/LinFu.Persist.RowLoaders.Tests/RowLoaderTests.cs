using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Simple.IoC;
using System.IO;
using Simple.IoC.Loaders;
using LinFu.Persist.Metadata;
using LinFu.Database;
using System.Data;
namespace LinFu.Persist.RowLoaders.Tests
{
    [TestFixture]
    public class RowLoaderTests : BaseFixture
    {
        private IContainer _container;
        private IList<IRowTaskItem> _rowTaskItems =
            new List<IRowTaskItem>();

        private ITableInfoRepository _tableRepository = null;

        public override void Setup()
        {
            _container = new SimpleContainer();
            var directory = Path.GetDirectoryName(typeof(RowLoaderTests).Assembly.Location);
            Loader loader = new Loader(_container);
            loader.LoadDirectory(directory, "*.dll");

            _tableRepository = _container.GetService<ITableInfoRepository>();
        }

        public override void TearDown()
        {
            _container = null;
            _tableRepository = null;
            base.TearDown();
        }


        
        [Test]
        public void LoadSingleRow()
        {
            IRowLoader rowLoader = _container.GetService<IRowLoader>();
            IEnumerable<IRow> rows = rowLoader.CreateRows(CreateTaskItems("Customers",1));
            Assert.IsNotNull(rows);
            Assert.IsTrue(rows.Count() == 1);
        }

        [Test]
        public void LoadMultipleRows()
        {
            IRowLoader rowLoader = _container.GetService<IRowLoader>();
            IEnumerable<IRow> rows = rowLoader.CreateRows(CreateTaskItems("Customers", 10));
            Assert.IsNotNull(rows);
            Assert.IsTrue(rows.Count() == 10);
        }

        [Test]
        public void LoadMultipleRowsWithCompositeKeys()
        {
            IRowLoader rowLoader = _container.GetService<IRowLoader>();
            IEnumerable<IRow> rows = rowLoader.CreateRows(CreateTaskItems("Order Details", 2000));
            Assert.IsNotNull(rows);
            Assert.IsTrue(rows.Count() == 2000);
        }
        private IEnumerable<IRowTaskItem> CreateTaskItems(string TableName, int maxRows)
        {
            var query = from column in _tableRepository.Tables[TableName].Columns select column.Value.LocalName;
            IList<IRowTaskItem> tasks = new List<IRowTaskItem>();

            IEnumerable<IDictionary<string,object>> primarykeys = GetPrimaryKeys(TableName,maxRows);
            foreach (var keyValues in primarykeys)
            {
                IRowTaskItem task = new RowTaskItem(TableName, query, keyValues);
                tasks.Add(task);
            }
            return tasks;
        }
        
        
        private IEnumerable<IDictionary<string,object>> GetPrimaryKeys(string tableName,int maxRows)
        {               
            IList<IDictionary<string, object>> keys = new List<IDictionary<string, object>>();

            ITableInfo tableInfo = _tableRepository.Tables[tableName];

            string keyList = tableInfo.PrimaryKey.Columns.Select(c => c.ColumnName)
                .Aggregate((current, next) => current + ", " + next);

            string sql = string.Format("SELECT TOP {0} {1} FROM {2}", maxRows, keyList, tableInfo.TableName);
            
            using (IConnection connection = _container.GetService<IConnection>())
            {
                
                IDataReader reader = connection.ExecuteReader(sql);
                try
                {
                    while (reader.Read())
                    {
                        IDictionary<string, object> keyValues = new Dictionary<string, object>();
                        foreach (var primaryKeyColumn in tableInfo.PrimaryKey.Columns)
                        {
                            keyValues.Add(primaryKeyColumn.LocalName, reader[primaryKeyColumn.LocalName]);
                        }
                        keys.Add(keyValues);
                    }
                }
                finally
                {
                    reader.Close();
                }
            }

           return keys;
        }

    }
}
