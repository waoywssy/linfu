using System.Collections.Generic;

namespace LinFu.Persist.Metadata
{
    /// <summary>
    /// Represents a storeage location for table repositories.
    /// </summary>
    public interface ITableInfoRepositoryStorage 
    {
        /// <summary>
        /// Gets a list of loaders (<see cref="ITableRepositoryLoader"/>) that should be used to load the repository.
        /// </summary>
        IList<ITableInfoRepositoryLoader> LoadStrategies { get; }
        
        /// <summary>
        /// Sets or gets the <see cref="ITableRepositoryPersister"/> that will be used to persist the repository.
        /// </summary>
        ITableInfoRepositoryPersister PersistStrategy { get; set; }        

        /// <summary>
        /// Stores the repository.
        /// </summary>
        /// <param name="repository"></param>
        void Store(ITableInfoRepository repository);
        
        /// <summary>
        /// Retrieves the repository using the applies loadstrategies.
        /// </summary>
        /// <remarks>Implementors should return the repository once sucessfully loaded by one of the load stragegies.
        /// The last strategy would naturally be the one that actually retrieves the structure from the datastore.</remarks>
        /// <param name="repositoryName">The name of the repository to retrieve.</param>
        /// <returns>A <see cref="ITableRepository"/> describing the datastore as identified by <paramref name="repositoryName"/></returns>
        void Load(string repositoryName,ITableInfoRepository repository);        
    }
}