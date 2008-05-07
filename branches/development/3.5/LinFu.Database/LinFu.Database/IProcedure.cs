using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace LinFu.Database
{
    public interface IProcedure
    {
        IConnection Connection { get; set; }
        
        IEnumerable<IDataParameter> Parameters { get; } 
        string Name { get; set; }
        DataTable ExecuteDataTable(params object[] parameters);
        IDataReader ExecuteReader(params object[] parameters);
        int ExecuteNonQuery(params object[] parameters);
        T ExecuteScalar<T>(params object[] parameters);
    }
}
