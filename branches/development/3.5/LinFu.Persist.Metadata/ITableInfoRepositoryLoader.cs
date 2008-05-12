namespace LinFu.Persist.Metadata
{
    /// <summary>
    /// Represents a loader that is capable of loading an <see cref="ITableRepository"/>
    /// from an external datastore(e.g a database) or from a cached repository.
    /// </summary>
    public interface ITableInfoRepositoryLoader
    {
        /// <summary>
        /// Loads the repository.
        /// </summary>
        /// <param name="repositoryName">The name of the repository to load.</param>
        /// <returns>A <see cref="ITableRepository"/> describing the structure of the datastore.</returns>
        void Load(string repositoryName, ITableInfoRepository repository);
    }

}