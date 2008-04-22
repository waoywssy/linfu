using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist
{
    public interface IPropertyMapping
    {
        Type PropertyType { get; set; }
        IKey MappedColumns { get; set; }
        string PropertyName { get; set; }
    }
}
