using System.Collections.Generic;
using System.Linq;
using LinFu.Persist.Metadata;
using Simple.IoC.Loaders;
using Simple.IoC;
using System.Data;
using System.Data.Common;
using System.Resources;
using System.Reflection;

namespace LinFu.Persist.Metadata.Implementation
{
    /// <summary>
    /// Represents an implementation of the <see cref="IDbTableRepositoryLoader"/> interface that is capable of loading 
    /// the structure from the database as represented by the <see cref="IDbTableRepositoryLoader.Connection"/> property.
    /// </summary>
    /// <remarks>
    /// This class is dependant upon the INFORMATION_SCHEMA views witch is part of the SQL-92 standard.
    /// The usablity of this class would depend on each DBMS vendors support and implementation.
    /// </remarks>
    [Implements(typeof(IDbTableRepositoryLoader), LifecycleType.OncePerRequest)]
    public class DbTableRepositoryLoader : IDbTableRepositoryLoader, IInitialize
    {
        private IContainer _container;
        
        private ITypeMapper _typeMapper;
        private readonly ResourceManager _resourceManager = new ResourceManager(string.Format("{0}.Sql",typeof(DbTableRepositoryLoader).Namespace) , Assembly.GetExecutingAssembly());

        #region IInitialize Members

        public void Initialize(IContainer container)
        {
            _container = container;
            _typeMapper = container.GetService<ITypeMapper>("DbTypeMapper");
        }

        #endregion

        #region IDbTableInfoLoader Members

        /// <summary>
        /// Sets or gets the connection that is used to retrieve the structure.
        /// </summary>
        public IDbConnection Connection {get;set;}
        

        #endregion

        #region ITableInfoLoader Members

        /// <summary>
        /// Loads the structure of the database.
        /// </summary>
        /// <param name="repositoryName">A name that is to be used to identify the loaded repository.</param>
        /// <returns>A <see cref="ITableRepository"/> that describes the structure of the database.</returns>
        public ITableRepository Load(string repositoryName)
        {                                    
            DbConnection connection = (Connection as DbConnection);

            if (connection == null)
                return null;

            ITableRepository repository = _container.GetService<ITableRepository>();
            repository.Name = repositoryName;

            connection.Open();
            
            //Get all the tables.
            //NOTE :Views are not returned here
            var tableRows = connection.GetSchema("Tables").Rows.Cast<DataRow>()
               .Where(r => (string)r["TABLE_TYPE"] == "BASE TABLE");

            //Get all the columns
            var columnRows = connection.GetSchema("Columns").Rows.Cast<DataRow>();

            foreach (DataRow row in tableRows)
            {
                ITableInfo tableInfo = new TableInfo { TableName = string.Format("{0}.{1}" ,row["TABLE_SCHEMA"],row["TABLE_NAME"]) };
                repository.Tables.Add(tableInfo.TableName, tableInfo);

                var tableColumns = columnRows.Where(r => (string)r["TABLE_NAME"] == (string)row["TABLE_NAME"] & (string)r["TABLE_SCHEMA"] == (string)row["TABLE_SCHEMA"])
                                        .OrderBy(o => (int)o["ORDINAL_POSITION"]);
                
                CreateColumns(tableInfo,tableColumns);              
            }

            //Now that we have all the tables and columns loaded, we are ready to set up the keys and relationships

            CreatePrimaryKeys(repository.Tables);
            CreateRelations(repository.Tables);
            connection.Close();

            return repository;
        }

        private void CreateColumns(ITableInfo tableInfo,IEnumerable<DataRow> tableColumns)
        {
            foreach (var item in tableColumns)
            {
                IColumnInfo columnInfo = _container.GetService<IColumnInfo>();
                columnInfo.ColumnName = (string)item["COLUMN_NAME"];
                columnInfo.DataType = _typeMapper.MapType((string)item["DATA_TYPE"], (string)item["IS_NULLABLE"] == "YES");
                columnInfo.Table = tableInfo;
                tableInfo.Columns.Add(columnInfo.ColumnName, columnInfo);
            }
        }

        private void CreatePrimaryKeys(IDictionary<string,ITableInfo> tables)
        {            
            IDataReader reader = CreateReader("PrimaryKeys");
            while (reader.Read())
            {                 
                string tableName = string.Format("{0}.{1}", reader["TABLE_SCHEMA"], reader["TABLE_NAME"]);
                string columnName = (string)reader["COLUMN_NAME"];

                IKeyInfo key = tables[tableName].PrimaryKey;
                if (key == null)
                {
                    key = _container.GetService<IKeyInfo>();
                    key.Table = tables[tableName];
                    tables[tableName].PrimaryKey = key;
                }
                
                key.Columns.Add(tables[tableName].Columns.Values.Where(c => c.ColumnName == columnName).First());
            }
            reader.Close();            
        }

        private void CreateRelations(IDictionary<string, ITableInfo> tables)
        {                                   
            IDictionary<string, IRelationInfo> relations = new Dictionary<string, IRelationInfo>();

            IDataReader reader = CreateReader("ForeignKeys");
            while (reader.Read())
            {
                string foreignTableName = string.Format("{0}.{1}", reader["FK_TABLE_SCHEMA"], reader["FK_TABLE_NAME"]);
                string foreignColumnName = (string)reader["FK_COLUMN_NAME"];
                string constraintName = (string)reader["FK_CONSTRAINT_NAME"];
                string primaryTableName = string.Format("{0}.{1}", reader["UQ_TABLE_SCHEMA"], reader["UQ_TABLE_NAME"]);

                if (!relations.ContainsKey(constraintName))
                {
                    IKeyInfo foreignkey = _container.GetService<IKeyInfo>();
                    foreignkey.Table = tables[foreignTableName];

                    IRelationInfo relationInfo = _container.GetService<IRelationInfo>();
                    relationInfo.PrimaryKey = tables[primaryTableName].PrimaryKey;
                    relationInfo.ForeignKey = foreignkey;
                    tables[foreignTableName].Relations.Add(relationInfo);
                    tables[primaryTableName].Relations.Add(relationInfo);
                   
                    relations.Add(constraintName, relationInfo);
                }

                relations[constraintName].ForeignKey.Columns.Add(tables[foreignTableName].Columns[foreignColumnName]);
                
            }            
        }

        private IDataReader CreateReader(string namedSqlResource)
        {
            IDbCommand command = Connection.CreateCommand();
            command.CommandText = _resourceManager.GetString(namedSqlResource);
            command.CommandType = CommandType.Text;
            return command.ExecuteReader();
        }


        #endregion
        
    }
}
