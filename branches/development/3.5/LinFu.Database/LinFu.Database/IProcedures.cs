using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Database
{
    public interface IProcedures
    {
        ITransaction Transaction { get; set; }
        IConnection Connection { get; set; }
        IProcedure this[string procedureName] { get; }
    }
}
