using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simple.IoC;

namespace LinFu.Database.Implementation
{
    [Factory(typeof(IConnection))]
    public class ConnectionFactory :IFactory<IConnection>
    {

        #region IFactory<IConnection> Members

        public IConnection CreateInstance(IContainer container)
        {
            if (!container.GetService<IConnectionRepository>().IsLoaded)            
                container.GetService<IConnectionRepositoryStorage>().Retrieve();            
            
            return new Connection();
        }

        #endregion
    }
}
