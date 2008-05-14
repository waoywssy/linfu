using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace LinFu.Database
{
    public class Parameter
    {
        public Parameter()
        {
            Direction = ParameterDirection.InputOutput;
            DbType = DbType.Object;
        }
        
        public string Name { get; set; }
        public object Value { get; set; }
        public ParameterDirection Direction { get; set; }
        public DbType DbType { get; set; }
    }
}
