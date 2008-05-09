namespace LinFu.Persist.Metadata
{
    /// <summary>
    /// Represensts a "persister" that is capable of storing an already loaded repository.
    /// </summary>
    public interface ITableInfoRepositoryPersister
    {
        /// <summary>
        /// Saves the <see cref="ITableRepository"/>.
        /// </summary>
        /// <param name="repository">The repository to save.</param>
        void Save(ITableInfoRepository repository);
    }
}
