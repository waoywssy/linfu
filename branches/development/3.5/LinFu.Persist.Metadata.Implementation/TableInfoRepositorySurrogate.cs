?using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simple.IoC;
using Simple.IoC.Loaders;

namespace LinFu.Persist.Metadata.Implementation
{
    [TypeSurrogate]
    public class TableInfoRepositorySurrogate : ITypeSurrogate
    {
        private IDictionary<string, ITableInfoRepository> _repositories =
            new Dictionary<string, ITableInfoRepository>();
        
        #region ITypeSurrogate Members

        public bool CanSurrogate(string serviceName, Type serviceType)
        {
            return typeof(ITableInfoRepository).IsAssignableFrom(serviceType);
        }

        public object ProvideSurrogate(string serviceName, Type serviceType)
        {
         
            if (!_repositories.ContainsKey(serviceName))
                _repositories.Add(serviceName, new TableInfoRepository());

            return _repositories[serviceName];                                    
        }

       
        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simple.IoC;
using Simple.IoC.Loaders;

namespace LinFu.Persist.Metadata.Implementation
{
    [TypeSurrogate]
    public class TableInfoRepositorySurrogate : ITypeSurrogate
    {
        private IDictionary<string, ITableInfoRepository> _repositories =
            new Dictionary<string, ITableInfoRepository>();
        
        #region ITypeSurrogate Members

        public bool CanSurrogate(string serviceName, Type serviceType)
        {
            return typeof(ITableInfoRepository).IsAssignableFrom(serviceType);
        }

        public object ProvideSurrogate(string serviceName, Type serviceType)
        {
         
            if (!_repositories.ContainsKey(serviceName))
                _repositories.Add(serviceName, new TableInfoRepository());

            return _repositories[serviceName];                                    
        }

       
        #endregion
    }
}
