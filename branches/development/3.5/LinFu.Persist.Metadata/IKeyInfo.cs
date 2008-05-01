using System.Collections.Generic;

namespace LinFu.Persist.Metadata
{
    /// <summary>
    /// Provides metadata information for a key.
    /// </summary>
    public interface IKeyInfo
    {
        /// <summary>
        /// Gets or sets the <see cref="ITableInfo"/> that is the owner of the key.
        /// </summary>
        ITableInfo Table { get; set; }

        /// <summary>
        /// Gets a collection of columns that belongs to this key.
        /// </summary>
        IList<IColumnInfo> Columns { get; }
    }
}
