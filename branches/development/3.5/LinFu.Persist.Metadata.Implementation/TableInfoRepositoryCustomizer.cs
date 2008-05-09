using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simple.IoC;
using Simple.IoC.Loaders;
using LinFu.Database;

namespace LinFu.Persist.Metadata.Implementation
{
    [Customizer]
    public class TableInfoRepositoryCustomizer : ICustomizeInstance
    {
        #region ICustomizeInstance Members

        public bool CanCustomize(string serviceName, Type serviceType, IContainer hostContainer)
        {
            return typeof(ITableInfoRepository).IsAssignableFrom(serviceType);
        }

        public void Customize(string serviceName, Type serviceType, object instance, IContainer hostContainer)
        {
            if (((ITableInfoRepository)instance).Tables.Count() > 0)
                return;

            IConnectionRepository connections = hostContainer.GetService<IConnectionRepository>();                        
            string repositoryName;

            if (string.IsNullOrEmpty(serviceName))
                repositoryName = connections.DefaultConnection.Name;
            else
                repositoryName = connections[serviceName].Name;                        
            
            ITableInfoRepositoryStorage storage = hostContainer.GetService<ITableInfoRepositoryStorage>();                                    
            storage.Load(repositoryName, (ITableInfoRepository)instance);            
        }

        #endregion
    }
}