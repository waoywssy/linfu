using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;

namespace LinFu.Persist
{
    internal sealed class QueryProvider : IQueryProvider
    {
        public QueryProvider() { }
        public QueryProvider(IQueryInterpreter interpreter)
        {
            QueryInterpreter = interpreter;
        }
        public IQueryInterpreter QueryInterpreter
        {
            get;
            set;
        }
        IQueryable<S> IQueryProvider.CreateQuery<S>(Expression expression)
        {
            return new Query<S>(this.QueryInterpreter, expression);
        }

        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {

            Type elementType = TypeSystem.GetElementType(expression.Type);

            IQueryable result = null;

            try
            {
                Type queryType = typeof(Query<>).MakeGenericType(elementType);

                result = (IQueryable)Activator.CreateInstance(queryType,
                    new object[] { this, expression });
            }

            catch (TargetInvocationException tie)
            {

                throw tie.InnerException;

            }
            return result;
        }
        S IQueryProvider.Execute<S>(Expression expression)
        {
            return (S)this.Execute(expression);
        }
        object IQueryProvider.Execute(Expression expression)
        {
            return Execute(expression);
        }
        public object Execute(Expression expression)
        {
            if (QueryInterpreter == null)
                throw new NotImplementedException("The QueryInterpreter property cannot be null");

            return QueryInterpreter.Execute(expression);
        }
    }
}
