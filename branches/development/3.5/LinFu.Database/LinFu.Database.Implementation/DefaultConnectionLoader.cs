using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simple.IoC;
using Simple.IoC.Loaders;
using System.Configuration;

namespace LinFu.Database.Implementation
{
    [Implements(typeof(IConnectionRepositoryLoader), LifecycleType.OncePerRequest, ServiceName = "DefaultRepositoryLoader")]
    public class DefaultConnectionLoader : IConnectionRepositoryLoader, IInitialize
    {

        #region Private Fields

        private IContainer _container;

        #endregion

        #region IInitialize Members

        public void Initialize(IContainer container)
        {
            _container = container;
        }

        #endregion

        #region IConnectionRepositoryLoader Members

        public void Load(IConnectionRepository repository)
        {
            foreach (ConnectionStringSettings cs in ConfigurationManager.ConnectionStrings)
            {
                IConnectionInfo connectionInfo = _container.GetService<IConnectionInfo>();
                connectionInfo.Name = cs.Name.Split('.').Last();
                connectionInfo.ProviderName = cs.ProviderName;
                connectionInfo.ConnectionString = cs.ConnectionString;
                repository.Add(connectionInfo);
            }
            repository.DefaultConnection = repository.Connections.Last();

        }

        #endregion

    }
}
