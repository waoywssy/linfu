using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simple.IoC.Loaders;

namespace LinFu.Persist
{
    [Implements(typeof(IDeriveTableName), LifecycleType.OncePerRequest)]
    public class DeriveTableNameFromAttribute : IDeriveTableName
    {
        private Dictionary<Type, string> _cachedResults = new Dictionary<Type, string>();
        public string GetTableName(Type targetType)
        {
            // Reuse the cached results, if possible
            if (_cachedResults.ContainsKey(targetType))
                return _cachedResults[targetType];

            var matches = (from a in targetType.GetCustomAttributes(typeof(PeristableAttribute), false)
                            where a != null &&
                            a is PeristableAttribute
                            select a as PeristableAttribute).ToList();

            int matchCount = matches.Count;
            string result = string.Empty;

            if (matchCount > 0)
            {
                // Set the default name
                result = string.Format("{0}s", targetType.Name);

                var attribute = matches[0];
                if (!string.IsNullOrEmpty(attribute.TableName))
                    result = attribute.TableName;
            }
            // Save the result
            _cachedResults[targetType] = result;

            return result;
        }
    }
}
