using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simple.IoC.Loaders;
using System.Configuration;
using Simple.IoC;
namespace LinFu.Database.Implementation
{
    [Implements(typeof(IConnectionRepositoryStorage),LifecycleType.OncePerRequest)]
    public class ConnectionRepositoryStorage : IConnectionRepositoryStorage, IInitialize
    {

        #region Private Fields
        
        private IContainer _container;

        #endregion

        #region IInitialize Members

        public void Initialize(IContainer container)
        {
            _container = container;
            LoadStrategy = container.GetService<IConnectionRepositoryLoader>("DefaultRepositoryLoader");
        }

        #endregion

        #region IConnectionRepositoryStorage Members

        public IConnectionRepositoryLoader LoadStrategy { get; set; }

        public IConnectionRepository Retrieve()
        {
            IConnectionRepository repository = _container.GetService<IConnectionRepository>();
            if (!repository.IsLoaded)
            {
                LoadStrategy.Load(repository);
                repository.IsLoaded = true;
            }
                
            IFactory<IConnection> factory = new ConnectionFactory();
            foreach (var item in repository.Connections)
            {
                if (!_container.NamedFactoryStorage.ContainsFactory<IConnection>(item.Name))    
                    _container.NamedFactoryStorage.Store<IConnection>(item.Name, factory);
            }

            return repository;
        }

        #endregion
       
    }
}
