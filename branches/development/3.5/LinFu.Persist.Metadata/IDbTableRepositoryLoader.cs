using System.Data;

namespace LinFu.Persist.Metadata
{
    /// <summary>
    /// Represents a reposiory loader that is capable of loading the structure of an DBMS based datastore.
    /// </summary>
    public interface IDbTableRepositoryLoader : ITableRepositoryLoader
    {
        /// <summary>
        /// Sets or gets the connection that is used to retrieve the structure.
        /// </summary>
        IDbConnection Connection { get; set; }
    }
}
