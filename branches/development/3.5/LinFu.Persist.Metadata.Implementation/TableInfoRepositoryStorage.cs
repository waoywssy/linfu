?using System.Collections.Generic;
using System.Linq;
using LinFu.Persist.Metadata;
using Simple.IoC.Loaders;
using Simple.IoC;
namespace LinFu.Persist.Metadata.Implementation
{
    /// <summary>
    /// Represents a default implementation of the <see cref="ITableRepositoryStorage"/> interface.
    /// The basic rule of thump here is that we retrieve the structure from the database, store it and 
    /// then use the stored/cached version for any subsequent loading of the repository. 
    /// </summary>
    [Implements(typeof(ITableInfoRepositoryStorage),LifecycleType.Singleton)]
    public class TableInfoRepositoryStorage : ITableInfoRepositoryStorage, IInitialize
    {
        private readonly IList<ITableInfoRepositoryLoader> _loadStrategies = new List<ITableInfoRepositoryLoader>();
        private ITableInfoRepositoryPersister _persistStrategy;
              
        #region IInitialize Members

        public void Initialize(IContainer container)
        {
            _loadStrategies.Add(container.GetService<ITableInfoRepositoryLoader>("CachedTableInfoRepositoryLoader"));
            _loadStrategies.Add(container.GetService<ITableInfoRepositoryLoader>("TableInfoRepositoryLoader"));            
            _persistStrategy = container.GetService<ITableInfoRepositoryPersister>();
        }

        #endregion

        #region ITableRepositoryStorage Members

        public IList<ITableInfoRepositoryLoader> LoadStrategies
        {
            get { return _loadStrategies; }
        }

        public ITableInfoRepositoryPersister PersistStrategy {get;set;}
        
        public void Store(ITableInfoRepository repository)
        {
            _persistStrategy.Save(repository);
        }

        public void Load(string repositoryName,ITableInfoRepository repository)
        {
            
            
            foreach (var loadStrategy in _loadStrategies)
            {
                loadStrategy.Load(repositoryName,repository);
                if (repository.Tables.Count != 0)
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
        }

        #endregion
    }
}
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
    /// then use the stored/cached version for any subsequent loading of the repository. 
    /// </summary>
    [Implements(typeof(ITableInfoRepositoryStorage),LifecycleType.Singleton)]
    public class TableInfoRepositoryStorage : ITableInfoRepositoryStorage, IInitialize
    {
        private readonly IList<ITableInfoRepositoryLoader> _loadStrategies = new List<ITableInfoRepositoryLoader>();
        private ITableInfoRepositoryPersister _persistStrategy;
              
        #region IInitialize Members

        public void Initialize(IContainer container)
        {
            _loadStrategies.Add(container.GetService<ITableInfoRepositoryLoader>("CachedTableInfoRepositoryLoader"));
            _loadStrategies.Add(container.GetService<ITableInfoRepositoryLoader>("TableInfoRepositoryLoader"));            
            _persistStrategy = container.GetService<ITableInfoRepositoryPersister>();
        }

        #endregion

        #region ITableRepositoryStorage Members

        public IList<ITableInfoRepositoryLoader> LoadStrategies
        {
            get { return _loadStrategies; }
        }

        public ITableInfoRepositoryPersister PersistStrategy {get;set;}
        
        public void Store(ITableInfoRepository repository)
        {
            _persistStrategy.Save(repository);
        }

        public void Load(string repositoryName,ITableInfoRepository repository)
        {
            
            
            foreach (var loadStrategy in _loadStrategies)
            {
                loadStrategy.Load(repositoryName,repository);
                if (repository.Tables.Count != 0)
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
        }

        #endregion
    }
}
