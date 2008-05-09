using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simple.IoC;
using Simple.IoC.Loaders;

namespace LinFu.Database.Implementation
{
    [Customizer]
    public class ConnectionRepositoryCustomizer : ICustomizeInstance
    {
        #region ICustomizeInstance Members

        public bool CanCustomize(string serviceName, Type serviceType, IContainer hostContainer)
        {
            return typeof(IConnectionRepository).IsAssignableFrom(serviceType);
        }

        public void Customize(string serviceName, Type serviceType, object instance, IContainer hostContainer)
        {
            IConnectionRepositoryStorage storage = hostContainer.GetService<IConnectionRepositoryStorage>();
            storage.Load((IConnectionRepository)instance);
        }

        #endregion
    }
}
