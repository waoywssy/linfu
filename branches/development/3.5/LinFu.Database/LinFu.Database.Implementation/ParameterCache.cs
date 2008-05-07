using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simple.IoC.Loaders;
using System.Data;

namespace LinFu.Database.Implementation
{
    [Implements(typeof(IParameterCache),LifecycleType.Singleton)]
    public class ParameterCache : IParameterCache
    {

        #region Private Fields
        
        private IDictionary<string, IEnumerable<IDataParameter>> _cache =
            new Dictionary<string, IEnumerable<IDataParameter>>();

        #endregion 

        #region IParameterCache Members

        public IEnumerable<IDataParameter> GetParameters(string key)
        {
            if (HasCachedParameters(key))
                return CloneParameters(_cache[key]);
            else
                return null;
        }

        public void CacheParameters(string key, IEnumerable<IDataParameter> parameters)
        {
            _cache.Add(key, CloneParameters(parameters));
        }

        
        public bool HasCachedParameters(string key)
        {
            return _cache.ContainsKey(key);
        }

        #endregion

        #region Private Methods

        private static IEnumerable<IDataParameter> CloneParameters(IEnumerable<IDataParameter> parameters)
        {
            List<IDataParameter> clonedParameters = new List<IDataParameter>();
            foreach (IDataParameter parameter in parameters)
            {
                clonedParameters.Add((IDataParameter)((ICloneable)parameter).Clone());
            }
            return clonedParameters;
        }
        #endregion 

    }
}
