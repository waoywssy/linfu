using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace LinFu.Database
{
    public interface IParameterCache
    {
        IEnumerable<IDataParameter> GetParameters(string key);
        void CacheParameters(string key, IEnumerable<IDataParameter> parameters);
        bool HasCachedParameters(string key);
    }
}
