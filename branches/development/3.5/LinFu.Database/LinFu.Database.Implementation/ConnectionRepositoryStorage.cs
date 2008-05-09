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

        public void Load(IConnectionRepository repository)
        {
            if (repository.IsLoaded)
                return;

            LoadStrategy.Load(repository);
            if (repository.DefaultConnection == null)
                repository.DefaultConnection = repository.Connections.Last();

            repository.IsLoaded = true;
        }

        #endregion
       
    }
}
