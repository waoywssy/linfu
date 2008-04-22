using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace LinFu.Persist
{
    public interface IFillTable
    {
        void Fill(ITable table, IDbCommand command);
    }
}
