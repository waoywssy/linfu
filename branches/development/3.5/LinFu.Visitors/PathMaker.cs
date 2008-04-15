using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Visitors
{
    public class PathMaker<T>
    {
        private Func<T, IEnumerable<T>> _getChildren;
        public PathMaker(Func<T, IEnumerable<T>> getChildren)
        {
            _getChildren = getChildren;
        }
        public Func<T, IEnumerable<T>> GetChildren
        {
            get
            {
                return _getChildren;
            }
            set
            {
                _getChildren = value;
            }
        }
        public IVisitorPath<T> CreatePath(T expression)
        {
            List<T> items = new List<T>();
            AddItem(expression, items);

            VisitorPath<T> result = new VisitorPath<T>();
            foreach (var item in items)
            {
                result.AddStep(item);
            }

            return result;
        }

        private void AddItem(T currentItem, IList<T> results)
        {
            if (_getChildren == null)
                return;

            if (currentItem == null)
                return;

            results.Add(currentItem);

            foreach (var child in _getChildren(currentItem))
            {
                AddItem(child, results);
            }
        }
    }
}
