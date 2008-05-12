using System.Collections.Generic;

namespace LinFu.Persist.Metadata
{
    /// <summary>
    /// Represents a repository of <see cref="ITableInfo"/> describing the structure of an external datastore.
    /// </summary>
    public interface ITableInfoRepository
    {
        /// <summary>
        /// Sets or gets the name of the repository.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets a collection of tables that belongs to this repository.
        /// </summary>
        IDictionary<string, ITableInfo> Tables { get; }

    }
}
