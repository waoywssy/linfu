using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simple.IoC;
using Simple.IoC.Loaders;

namespace LinFu.Database.Implementation
{
    [Customizer]
    public class ConnectionCustomizer : ICustomizeInstance
    {
        #region ICustomizeInstance Members

        public bool CanCustomize(string serviceName, Type serviceType, IContainer hostContainer)
        {
            return typeof(IConnection).IsAssignableFrom(serviceType);
        }

        public void Customize(string serviceName, Type serviceType, object instance, IContainer hostContainer)
        {
            IConnectionRepository connectionRepository = hostContainer.GetService<IConnectionRepository>();
            IConnectionInfo connectionInfo = null;
            IConnection connection = (IConnection)instance;

            if (string.IsNullOrEmpty(serviceName))
                connectionInfo = connectionRepository.DefaultConnection;
            else
                connectionInfo = connectionRepository[serviceName];

            
            connection.ProviderFactory = connectionInfo.CreateProviderFactory();
            connection.ConnectionString = connectionInfo.ConnectionString;
            try
            {
                connection.BulkLoader = hostContainer.GetService<IBulkLoader>(connectionInfo.ProviderName);
            }
            catch { }
        }

        #endregion
    }
}
