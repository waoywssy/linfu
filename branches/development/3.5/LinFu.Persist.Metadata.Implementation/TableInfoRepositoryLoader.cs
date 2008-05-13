using System.Collections.Generic;
using System.Linq;
using LinFu.Persist.Metadata;
using Simple.IoC.Loaders;
using Simple.IoC;
using System.Data;
using System.Data.Common;
using System.Resources;
using System.Reflection;
using LinFu.Database;

namespace LinFu.Persist.Metadata.Implementation
{
    /// <summary>
    /// Represents an implementation of the <see cref="IDbTableRepositoryLoader"/> interface that is capable of loading 
    /// the structure from the database as represented by the <see cref="IDbTableRepositoryLoader.Connection"/> property.
    /// </summary>
    /// <remarks>
    /// This class is dependant upon the INFORMATION_SCHEMA views witch is part of the SQL-92 standard.
    /// The usability of this class would depend on each DBMS vendors support and implementation.
    /// </remarks>
    [Implements(typeof(ITableInfoRepositoryLoader), LifecycleType.OncePerRequest, ServiceName = "TableInfoRepositoryLoader")]
    public class TableInfoRepositoryLoader : ITableInfoRepositoryLoader, IInitialize
    {
        private IContainer _container;

        private ITypeMapper _typeMapper;
        private readonly ResourceManager _resourceManager = new ResourceManager(string.Format("{0}.Sql", typeof(TableInfoRepositoryLoader).Namespace), Assembly.GetExecutingAssembly());

        #region IInitialize Members

        public void Initialize(IContainer container)
        {
            _container = container;
            _typeMapper = container.GetService<ITypeMapper>();
        }

        #endregion



        #region ITableInfoLoader Members

        /// <summary>
        /// Loads the structure of the database.
        /// </summary>
        /// <param name="repositoryName">A name that is to be used to identify the loaded repository.</param>
        /// <returns>A <see cref="ITableRepository"/> that describes the structure of the database.</returns>
        public void Load(string repositoryName, ITableInfoRepository repository)
        {

            IConnection connection = _container.GetService<IConnection>(repositoryName);

            DbConnection dbConnection = connection.ProviderFactory.CreateConnection();
            dbConnection.ConnectionString = connection.ConnectionString;

            repository.Name = repositoryName;

            dbConnection.Open();

            // Get all the tables.
            // NOTE :Views are not returned here
            var tableRows = dbConnection.GetSchema("Tables").Rows.Cast<DataRow>()
               .Where(r => (string)r["TABLE_TYPE"] == "BASE TABLE");

            // Get all the columns
            var columnRows = dbConnection.GetSchema("Columns").Rows.Cast<DataRow>();

            dbConnection.Close();

            foreach (DataRow row in tableRows)
            {
                string schema = row["TABLE_SCHEMA"] as string ?? string.Empty;
                string tablename = row["TABLE_NAME"] as string ?? string.Empty;

                if (schema == string.Empty || tablename == string.Empty)
                    continue;

                TableInfo tableInfo = new TableInfo { TableName = tablename };
                repository.Tables.Add(tableInfo.TableName, tableInfo);

                var tableColumns = from r in columnRows
                                   let currentTableName = r["TABLE_NAME"] as string
                                   let currentSchema = r["TABLE_SCHEMA"] as string
                                   where currentSchema != null && currentTableName != null
                                   select r;

                tableColumns = tableColumns.OrderBy(o =>
                    {
                        int ordinal = 0;
                        object value = o["ORDINAL_POSITION"];

                        if (value is int)
                            ordinal = (int)value;

                        return ordinal;
                    });


                tableInfo.SchemaName = schema;
                CreateColumns(tableInfo, tableColumns);
            }

            // Now that we have all the tables and columns loaded, 
            // we are ready to set up the keys and relationships
            CreatePrimaryKeys(repository.Tables, connection);
            CreateRelations(repository.Tables, connection);

        }

        private void CreateColumns(ITableInfo tableInfo, IEnumerable<DataRow> tableColumns)
        {
            foreach (var item in tableColumns)
            {
                IColumnInfo columnInfo = _container.GetService<IColumnInfo>();
                columnInfo.ColumnName = (string)item["COLUMN_NAME"];
                columnInfo.DataType = _typeMapper.MapType((string)item["DATA_TYPE"], (string)item["IS_NULLABLE"] == "YES");
                columnInfo.Table = tableInfo;

                if (tableInfo.Columns.ContainsKey(columnInfo.ColumnName))
                    continue;

                tableInfo.Columns.Add(columnInfo.ColumnName, columnInfo);
            }
        }

        private void CreatePrimaryKeys(IDictionary<string, ITableInfo> tables, IConnection connection)
        {
            IDataReader reader = connection.ExecuteReader(_resourceManager.GetString("PrimaryKeys"));
            while (reader.Read())
            {
                string tableName = string.Format("{0}.[{1}]", reader["TABLE_SCHEMA"], reader["TABLE_NAME"]);
                string columnName = (string)reader["COLUMN_NAME"];

                IKeyInfo key = tables[tableName].PrimaryKey;
                if (key == null)
                {
                    key = _container.GetService<IKeyInfo>();
                    key.Table = tables[tableName];
                    tables[tableName].PrimaryKey = key;
                }

                var matchingColumns = tables[tableName].Columns.Values.Where(c => c.ColumnName == columnName);
                var firstMatch = matchingColumns.FirstOrDefault();
                if (firstMatch == null)
                    continue;

                key.Columns.Add(firstMatch);
            }
            reader.Close();
        }

        private void CreateRelations(IDictionary<string, ITableInfo> tables, IConnection connection)
        {
            IDictionary<string, IRelationInfo> relations = new Dictionary<string, IRelationInfo>();

            IDataReader reader = connection.ExecuteReader(_resourceManager.GetString("ForeignKeys"));

            while (reader.Read())
            {
                string foreignTableName = string.Format("{0}.[{1}]", reader["FK_TABLE_SCHEMA"], reader["FK_TABLE_NAME"]);
                string foreignColumnName = (string)reader["FK_COLUMN_NAME"];
                string constraintName = (string)reader["FK_CONSTRAINT_NAME"];
                string primaryTableName = string.Format("{0}.[{1}]", reader["UQ_TABLE_SCHEMA"], reader["UQ_TABLE_NAME"]);

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




        #endregion

    }
}