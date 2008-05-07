using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simple.IoC.Loaders;
using System.Data.Common;

namespace LinFu.Database.Implementation
{
    [Implements(typeof(IConnectionInfo),LifecycleType.OncePerRequest)]
    public class ConnectionInfo : IConnectionInfo
    {
     
        #region IConnectionInfo Members

        public string Name {get;set;}
        
        public string ProviderName {get;set;}
        
        public string ConnectionString {get;set;}

        public DbProviderFactory CreateProviderFactory()
        {
            return DbProviderFactories.GetFactory(ProviderName);
        }

        #endregion

    }
}
