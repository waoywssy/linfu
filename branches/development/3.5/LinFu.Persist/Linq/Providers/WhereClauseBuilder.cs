using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Linq.TreeMasher;
using LinFu.Visitors;

namespace LinFu.Persist
{
    public class WhereClauseBuilder
    {
        private StringBuilder _builder = new StringBuilder();

        public WhereClauseBuilder(StringBuilder builder)
        {
            _builder = builder;
        }

        public void Visit(IMetaExpression expression)
        {
            MetaVisitor<IMetaExpression> visitor = new MetaVisitor<IMetaExpression>();

            // Handle simple binary expressions
            var simpleComparison = new SimpleLogicalComparison(_builder);
            simpleComparison.PropertyMappingRegistry = new SamplePropertyMappingRegistry();
            visitor.Cases.Add(simpleComparison);

            // Combine those simple binary expressions together using
            // this visitor 
            var compoundExpression = new CompoundExpressionVisitor(_builder);
            compoundExpression.Visitor = visitor;
            visitor.Cases.Add(compoundExpression);

            Func<IMetaExpression, IEnumerable<IMetaExpression>> getChildren = e => e.Children;
            PathMaker<IMetaExpression> pathMaker = new PathMaker<IMetaExpression>(getChildren);

            var path = pathMaker.CreatePath(expression);
            path.WalkWith(visitor);
        }
    }
}
