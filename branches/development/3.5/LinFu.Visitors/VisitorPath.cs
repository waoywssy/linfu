using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Visitors
{
    public class VisitorPath<T> : IVisitorPath<T>
    {
        private IList<T> _steps = new List<T>();
        public void WalkWith(IVisitor<T> visitor)
        {
            if (visitor == null)
                throw new ArgumentNullException("visitor");

            foreach (var step in _steps)
            {
                visitor.Visit(step);
            }
        }

        public void AddStep(T visitableItem)
        {
            _steps.Add(visitableItem);
        }

        public void Clear()
        {
            _steps.Clear();
        }
    }
}
