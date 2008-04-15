using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Visitors
{
    public interface IVisitor<T>
    {
        bool Matches(T item);
        void Visit(T expression);
    }
}
