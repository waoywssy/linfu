using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace LinFu.Persist
{
    public interface IBulkLoader
    {
        IDbConnection Connection { get; set; }
        void Load(DataTable dataTable);
    }
}
