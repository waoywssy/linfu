using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Linq.TreeMasher;

namespace LinFu.Persist
{
    public class CompoundExpressionVisitor : ExpressionVisitor
    {
        private static Dictionary<ExpressionType, string> _expressionMap = new Dictionary<ExpressionType, string>();
        static CompoundExpressionVisitor()
        {

            _expressionMap[ExpressionType.And] = "AND";
            _expressionMap[ExpressionType.AndAlso] = "AND";
            _expressionMap[ExpressionType.Or] = "OR";
            _expressionMap[ExpressionType.OrElse] = "OR";
        }
        private StringBuilder _builder;
        public CompoundExpressionVisitor(StringBuilder builder)
        {
            _builder = builder;
        }
        public override void Visit(IMetaExpression expression)
        {
            if (_builder == null || Visitor == null)
                return;

            BinaryExpression compoundExpression = expression.TargetExpression as BinaryExpression;
            if (compoundExpression == null)
                return;

            if (!_expressionMap.ContainsKey(compoundExpression.NodeType))
                return;

            _builder.Append("(");

            // Visit the left child            
            VisitChildExpression(expression.Children[0]);

            // Print the operator
            _builder.AppendFormat(" {0} ", _expressionMap[compoundExpression.NodeType]);

            // Visit the right child
            VisitChildExpression(expression.Children[1]);
            _builder.Append(")");
        }

        private void VisitChildExpression(IMetaExpression child)
        {
            // Print the opening outer parentheses
            _builder.Append("(");

            Visitor.Visit(child);

            // Print the closing outer parentheses
            _builder.Append(")");
        }

        public override bool Matches(IMetaExpression expression)
        {
            if (_builder == null || Visitor == null)
                return false;

            // Unary expressions are not supported
            var targetExpression = expression.TargetExpression;
            var nodeType = targetExpression.NodeType;

            if (targetExpression.NodeType == ExpressionType.Not)
                return false;

            // This must be a compound statement
            if (!_expressionMap.ContainsKey(nodeType))
                return false;

            if (!targetExpression.IsCompoundExpression())
                return false;

            if (expression.ExpressionType != typeof(BinaryExpression))
                return false;

            if (expression.Children.Count != 2)
                return false;

            return true;
        }

    }
}
