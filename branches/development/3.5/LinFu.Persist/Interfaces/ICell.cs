using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist
{
    public interface ICell
    {
        object Value { get; set; }
    }
}
