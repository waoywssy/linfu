using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist.Metadata
{
    /// <summary>
    /// Provides metadata information about a table in the datastore.
    /// </summary>
    public interface ITableInfo
    {
        /// <summary>
        /// Gets or sets the name of the <see cref="ITableInfo"/> as formatted by the datasource spesific naming scheme.
        /// </summary>        
        string TableName { get; set; }

        /// <summary>
        /// The local name of the <see cref="ITableInfo"/> without the datasource specific naming scheme.
        /// </summary>        
        string LocalName { get; set; }

        /// <summary>
        /// Describes the schema that owns the current table.
        /// </summary>
        string SchemaName { get; set; }

        /// <summary>
        /// Describes the primary key.
        /// </summary>
        IKeyInfo PrimaryKey { get; set; }

        /// <summary>
        /// Gets a collection of relations that belongs to this <see cref="ITableInfo"/>.
        /// </summary>
        /// <remarks>This list will not only include foreign key relations, but also relations from other tables
        /// that has this table as the primary table.</remarks>
        IList<IRelationInfo> Relations { get; }

        /// <summary>
        /// Gets a collection of columns that belongs to this <see cref="ITableInfo"/>.
        /// </summary>
        IDictionary<string, IColumnInfo> Columns { get; }
    }

}
