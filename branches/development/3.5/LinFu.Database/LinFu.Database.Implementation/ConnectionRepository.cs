using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simple.IoC.Loaders;

namespace LinFu.Database.Implementation
{
    [Implements(typeof(IConnectionRepository), LifecycleType.Singleton)]
    public class ConnectionRepository : IConnectionRepository
    {

        #region Private Fields

        private IDictionary<string, IConnectionInfo> _connections =
            new Dictionary<string, IConnectionInfo>();

        #endregion

        #region IConnectionRepository Members

        public void Add(IConnectionInfo connectionInfo)
        {
            _connections.Add(connectionInfo.Name, connectionInfo);
        }

        public IConnectionInfo this[string name]
        {
            get { return _connections[name]; }
        }

        public bool Contains(string name)
        {
            return _connections.ContainsKey(name);
        }

        public IConnectionInfo DefaultConnection { get; set; }

        public IEnumerable<IConnectionInfo> Connections
        {
            get { return _connections.Values; }
        }

        public bool IsLoaded { get; set; }

        #endregion

    }
}
