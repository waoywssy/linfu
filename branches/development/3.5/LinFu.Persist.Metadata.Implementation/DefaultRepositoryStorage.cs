using System.Collections.Generic;
using System.Linq;
using LinFu.Persist.Metadata;
using Simple.IoC.Loaders;
using Simple.IoC;
namespace LinFu.Persist.Metadata.Implementation
{
    /// <summary>
    /// Represents a default implementation of the <see cref="ITableRepositoryStorage"/> interface.
    /// The basic rule of thump here is that we retrieve the structure from the database, store it and 
    /// the used the stored/cached version for any subsequent loading of the repository. 
    /// </summary>
    [Implements(typeof(ITableRepositoryStorage),LifecycleType.Singleton)]
    public class DefaultRepositoryStorage : ITableRepositoryStorage, IInitialize
    {
        private readonly IList<ITableRepositoryLoader> _loadStrategies = new List<ITableRepositoryLoader>();
        private ITableRepositoryPersister _persistStrategy;
              
        #region IInitialize Members

        public void Initialize(IContainer container)
        {
            _loadStrategies.Add(container.GetService<ITableRepositoryLoader>("DefaultTableRepositoryLoader"));
            _loadStrategies.Add(container.GetService<ITableRepositoryLoader>("DbTableRepositoryLoader"));
            _persistStrategy = container.GetService<ITableRepositoryPersister>();
        }

        #endregion

        #region ITableRepositoryStorage Members

        public IList<ITableRepositoryLoader> LoadStrategies
        {
            get { return _loadStrategies; }
        }

        public ITableRepositoryPersister PersistStrategy {get;set;}
        
        public void Store(ITableRepository repository)
        {
            _persistStrategy.Save(repository);
        }

        public ITableRepository Retrieve(string repositoryName)
        {
            ITableRepository repository = null;
            
            foreach (var loadStrategy in _loadStrategies)
            {
                repository = loadStrategy.Load(repositoryName);
                if (repository != null)
                {
                    //We assume that the last loader is the loader that 
                    //actually retrieves the schema from the datastore
                    if (loadStrategy == _loadStrategies.Last())
                    {
                        //Save it for later
                        _persistStrategy.Save(repository);
                    }
                    break;
                }                    
            }
            return repository;
        }

        #endregion
    }
}
