using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist
{
    public interface IMapper
    {
        object CreateItem(Type targetType, IRow currentRow);
        bool CanCreateWith(IEnumerable<IColumn> columns);

        IPropertyAssignmentBehavior PropertyAssignmentBehavior { get; set; }

        IEnumerable<object> CreateItems(Type itemType, IEnumerable<IRow> rows);
    }
}
