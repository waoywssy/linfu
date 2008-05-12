using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Visitors
{
    public class MetaVisitor<T> : IVisitor<T>
    {
        private List<IVisitor<T>> _cases = new List<IVisitor<T>>();
        private HashSet<T> _visitedItems = new HashSet<T>();
        public IList<IVisitor<T>> PotentialVisitors
        {
            get
            {
                return _cases;
            }
        }
        public bool Matches(T expression)
        {
            return true;
        }
        public void Visit(T expression)
        {
            // Find a visitor that can handle
            // the current expression
            var matches = (from c in _cases
                           where c.Matches(expression)
                           select c).ToList();

            // Make sure that all nodes are visited only once
            if (matches.Count == 0 || _visitedItems.Contains(expression))
                return;

            var firstMatch = matches[0];
            firstMatch.Visit(expression);

            _visitedItems.Add(expression);
        }
    }

}
