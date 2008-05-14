using System;

namespace LinFu.Persist.Metadata
{
    /// <summary>
    /// Provides metadata information about a column in the datastore
    /// </summary>
    public interface IColumnInfo
    {
        /// <summary>
        /// Sets or gets the <see cref="ITableInfo"/> that is the owner of this column.
        /// </summary>
        ITableInfo Table { get; set; }

        /// The local name of the <see cref="IColumnInfo"/> without the datasource specific naming scheme.
        /// </summary>        
        string LocalName { get; set; }

        /// <summary>
        /// Sets or gets the name of the column.
        /// </summary>
        string ColumnName { get; set; }

        /// <summary>
        /// Sets or gets the .Net datatype that should be used to represent the column value.
        /// </summary>
        Type DataType { get; set; }
    }
}
