using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Linq.Expressions;

namespace LinFu.Persist
{
    public class Query<T> : IQueryable<T>, IQueryable, IEnumerable<T>, IEnumerable, IOrderedQueryable<T>, IOrderedQueryable
    {
        private readonly QueryProvider _provider = new QueryProvider();
        private Expression _expression;

        public Query(IQueryInterpreter interpreter)
        {
            _provider.QueryInterpreter = interpreter;
            _expression = Expression.Constant(this);
        }

        internal Query(IQueryInterpreter interpreter, Expression expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            if (!typeof(IQueryable<T>).IsAssignableFrom(expression.Type))
                throw new ArgumentOutOfRangeException("expression");

            _provider.QueryInterpreter = interpreter;
            _expression = expression;
        }


        public Expression Expression
        {
            get { return _expression; }
        }

        public Type ElementType
        {
            get { return typeof(T); }
        }

        public IQueryProvider Provider
        {
            get { return this._provider; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (_provider == null)
                return null;

            var result = _provider.Execute(_expression);
            var enumerable = (IEnumerable<T>)result;

            return enumerable.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            if (_provider == null)
                return null;

            var result = _provider.Execute(_expression);
            var enumerable = (IEnumerable)result;

            return enumerable.GetEnumerator();
        }
    }
}
