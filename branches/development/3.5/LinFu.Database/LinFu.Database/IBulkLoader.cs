using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace LinFu.Database
{
    public interface IBulkLoader
    {
        void Load(DataTable table);
        IConnection Connection { get; set; }
    }
}
