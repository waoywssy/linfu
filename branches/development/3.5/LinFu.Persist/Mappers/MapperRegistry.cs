using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist
{
    public class MapperRegistry : IMapperRegistry
    {
        private Dictionary<Type, IMapper> _entries = new Dictionary<Type, IMapper>();
        public MapperRegistry()
        {
            MatchCovariantTypes = true;
        }
        public bool MatchCovariantTypes { get; set; }
        public bool HasMapperFor(Type targetType, IDictionary<string, IColumn> rowColumns)
        {
            bool hasExactMatch = _entries.ContainsKey(targetType);

            IMapper bestMatch = null;

            if (hasExactMatch)
                bestMatch = _entries[targetType];

            if (!hasExactMatch && MatchCovariantTypes)
            {
                var matchingEntries = (from k in _entries.Keys
                                       where k.IsAssignableFrom(targetType)
                                       select k).ToList();

                if (matchingEntries.Count > 0)
                    bestMatch = _entries[matchingEntries[0]];
            }

            return bestMatch != null && bestMatch.CanCreateWith(rowColumns.Values);
        }

        public IMapper GetMapper(Type targetType, IDictionary<string, IColumn> rowColumns)
        {
            if (!_entries.ContainsKey(targetType) || !_entries[targetType].CanCreateWith(rowColumns.Values))
                return null;

            if (!MatchCovariantTypes)
                return _entries[targetType];

            // Use the first possible match
            var matchingEntries = (from k in _entries.Keys
                                   where k.IsAssignableFrom(targetType)
                                   select k).ToList();

            if (matchingEntries.Count > 0)
            {
                var key = matchingEntries[0];
                return _entries[key];
            }

            return null;
        }

        public void Register(IMapper Mapper, Type targetType, IDictionary<string, IColumn> rowColumns)
        {
            _entries[targetType] = Mapper;
        }
    }
}
