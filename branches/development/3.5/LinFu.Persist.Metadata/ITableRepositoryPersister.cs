namespace LinFu.Persist.Metadata
{
    /// <summary>
    /// Represensts a "persister" that is capable of storing an already loaded repository.
    /// </summary>
    public interface ITableRepositoryPersister
    {
        /// <summary>
        /// Saves the <see cref="ITableRepository"/>.
        /// </summary>
        /// <param name="repository">The repository to save.</param>
        void Save(ITableRepository repository);
    }
}
