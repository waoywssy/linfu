using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using LinFu.Reflection;

namespace Linq.TreeMasher
{
    public class TreeMasher
    {
        private IList<IMetaExpression> _results;
        private int _currentDepth = 0;
        private static int _sequence = 0;
        private IMetaExpression _parent;

        public TreeMasher()
        {
            _results = new List<IMetaExpression>();

        }
        public TreeMasher(IList<IMetaExpression> results)
        {
            _results = results;
        }
        public static List<IMetaExpression> Mash(Expression expression)
        {
            List<IMetaExpression> results = new List<IMetaExpression>();

            TreeMasher masher = new TreeMasher(results);
            masher.MashExpression(expression);

            return results;
        }

        private void MashExpression(Expression expression)
        {
            DoVisit(expression);
        }
        private IMetaExpression DoVisit(Expression expression)
        {
            DynamicObject dynamic = new DynamicObject(new TreeMasher() { _currentDepth = this._currentDepth + 1, _results = this._results });
            return dynamic.Methods["Visit"](expression) as IMetaExpression;
        }
        private IMetaExpression DoVisit(IMetaExpression parent, Expression child)
        {
            TreeMasher masher = new TreeMasher()
            {
                _currentDepth = this._currentDepth + 1,
                _results = this._results,
                _parent = parent
            };
            DynamicObject dynamic = new DynamicObject(masher);
            IMetaExpression result = (IMetaExpression)dynamic.Methods["Visit"](child);

            masher._currentDepth = this._currentDepth;

            return result;
        }

        #region Expression nodes that have child nodes
        public MetaExpression Visit(MethodCallExpression expression)
        {
            MetaExpression meta = CreateMetaData(_parent, expression);
            _results.Add(meta);

            foreach (Expression child in expression.Arguments)
            {
                AddChild(meta, child);
            }

            return meta;
        }
        public MetaExpression Visit(InvocationExpression expression)
        {
            MetaExpression data = CreateMetaData(_parent, expression);
            _results.Add(data);

            AddChild(data, expression.Expression);
            foreach (Expression current in expression.Arguments)
            {
                AddChild(data, current);
            }

            return data;
        }
        public MetaExpression Visit(TypeBinaryExpression expression)
        {
            MetaExpression data = CreateMetaData(_parent, expression);
            _results.Add(data);

            AddChild(data, expression.Expression);

            return data;
        }
        public MetaExpression Visit(UnaryExpression expression)
        {
            MetaExpression data = CreateMetaData(_parent, expression);
            _results.Add(data);

            AddChild(data, expression.Operand);
            return data;
        }
        public MetaExpression Visit(LambdaExpression expression)
        {
            MetaExpression data = CreateMetaData(_parent, expression);
            _results.Add(data);

            data.Children.Add(DoVisit(data, expression.Body));
            foreach (Expression child in expression.Parameters)
            {
                AddChild(data, child);
            }
            return data;
        }
        public IMetaExpression Visit(MemberInitExpression expression)
        {
            MetaExpression data = CreateMetaData(_parent, expression);
            _results.Add(data);

            AddChild(data, expression.NewExpression);

            return data;
        }
        public MetaExpression Visit(BinaryExpression expression)
        {
            MetaExpression data = CreateMetaData(_parent, expression);

            AddChild(data, expression.Left);
            AddChild(data, expression.Right);

            _results.Add(data);
            return data;
        }
        public IMetaExpression Visit(ConditionalExpression expression)
        {
            MetaExpression data = CreateMetaData(_parent, expression);
            _results.Add(data);

            AddChild(data, expression.IfTrue);
            AddChild(data, expression.IfFalse);
            AddChild(data, expression.Test);

            return data;
        }
        public IMetaExpression Visit(NewExpression expression)
        {
            MetaExpression data = CreateMetaData(_parent, expression);
            return data;
        }
        public IMetaExpression Visit(ListInitExpression expression)
        {
            MetaExpression data = CreateMetaData(_parent, expression);
            _results.Add(data);

            AddChild(data, expression.NewExpression);

            return data;
        }
        public IMetaExpression Visit(NewArrayExpression expression)
        {
            MetaExpression data = CreateMetaData(_parent, expression);
            _results.Add(data);

            foreach (Expression child in expression.Expressions)
            {
                AddChild(data, child);
            }
            return data;
        }
        #endregion
        #region Expression nodes that have no child nodes
        public MetaExpression Visit(ConstantExpression expression)
        {
            MetaExpression data = CreateMetaData(_parent, expression);
            _results.Add(data);

            return data;
        }
        public MetaExpression Visit(ParameterExpression expression)
        {
            MetaExpression data = CreateMetaData(_parent, expression);
            _results.Add(data);

            return data;
        }

        public IMetaExpression Visit(MemberExpression expression)
        {
            MetaExpression data = CreateMetaData(_parent, expression);
            _results.Add(data);
            return data;
        }


        #endregion
        private void AddChild(IMetaExpression parent, Expression expression)
        {
            parent.Children.Add(DoVisit(parent, expression));
        }
        private MetaExpression CreateMetaData(IMetaExpression parent, Expression child)
        {
            _sequence++;
            MetaExpression result = new MetaExpression() { Depth = _currentDepth, TargetExpression = child, Parent = parent, Sequence=_sequence};

            return result;
        }

    }
}
