using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace LinFu.Persist
{
    public interface IQueryInterpreter
    {
        object Execute(Expression expression);
    }
}
