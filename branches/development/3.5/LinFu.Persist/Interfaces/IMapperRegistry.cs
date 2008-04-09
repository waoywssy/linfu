using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist
{
    public interface IMapperRegistry
    {
        bool HasMapperFor(Type targetType, IDictionary<string, IColumn> rowColumns);
        IMapper GetMapper(Type targetType, IDictionary<string, IColumn> rowColuns);
        void Register(IMapper Mapper, Type targetType, IDictionary<string, IColumn> rowColumns);
    }
}
