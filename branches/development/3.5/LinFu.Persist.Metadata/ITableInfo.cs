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
        /// Get the table name.
        /// </summary>
        /// <remarks>In order to make the tablename unique, 
        /// the tablename is normally a concatenation of schema and tablename.</remarks>
        string TableName { get; set; }

        /// <summary>
        /// Describes the primary key.
        /// </summary>
        IKeyInfo PrimaryKey { get; set; }

        /// <summary>
        /// Gets a collection o relations that belongs to this <see cref="ITableInfo"/>.
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
