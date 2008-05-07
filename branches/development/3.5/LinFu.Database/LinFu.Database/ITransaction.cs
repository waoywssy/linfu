using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace LinFu.Database
{
    public interface ITransaction
    {
        IConnection Connection {get;set;}
        void BeginTransaction(IDbCommand command);
        void FinalizeTransaction();
    }
}
