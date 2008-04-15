using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Visitors
{
    public interface IVisitorPath<T>
    {
        void AddStep(T visitableItem);
        void WalkWith(IVisitor<T> visitor);
        void Clear();
    }
}
