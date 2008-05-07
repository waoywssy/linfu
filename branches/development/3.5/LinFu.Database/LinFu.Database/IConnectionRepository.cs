using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Database
{
    
    public interface IConnectionRepository
    {
        void Add(IConnectionInfo connectionInfo);
        IConnectionInfo this[string name] { get; }
        bool Contains(string name);
        IConnectionInfo DefaultConnection { get; set; }
        IEnumerable<IConnectionInfo> Connections { get; }
        bool IsLoaded { get; set; }
    }
}
