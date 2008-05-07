using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Database.Implementation
{
    public class DatabaseException : ApplicationException
    {

        public DatabaseException(string message)
            : base(message)
        {

        } 
        public DatabaseException(string message,Exception innerExeption) : base(message,innerExeption)
        {

        } 
    }
}
