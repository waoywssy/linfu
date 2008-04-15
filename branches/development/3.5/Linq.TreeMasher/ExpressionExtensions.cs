using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Linq.TreeMasher;

namespace Linq.TreeMasher
{
    public static class ExpressionExtensions
    {
        private static readonly HashSet<ExpressionType> logicalComparisonTypes = new HashSet<ExpressionType>();
        private static readonly HashSet<ExpressionType> logicalNodeTypes = new HashSet<ExpressionType>();
        static ExpressionExtensions()
        {
            ExpressionType[] comparisonTypes = new ExpressionType[]
                {
                    ExpressionType.Equal,
                    ExpressionType.GreaterThan,
                    ExpressionType.GreaterThanOrEqual,
                    ExpressionType.LessThan,
                    ExpressionType.LessThanOrEqual,                    
                };

            ExpressionType[] nodeTypes = new ExpressionType[] 
                {
                    ExpressionType.Not,
                    ExpressionType.And, 
                    ExpressionType.AndAlso,
                    ExpressionType.Or,
                    ExpressionType.OrElse,
                    ExpressionType.ExclusiveOr,
                };

            foreach (var item in comparisonTypes)
            {
                logicalComparisonTypes.Add(item);
            }

            foreach (var item in nodeTypes)
            {
                logicalNodeTypes.Add(item);
            }

        }
        public static bool IsLogicalComparison(this Expression expression)
        {
            return logicalComparisonTypes.Contains(expression.NodeType);
        }
        public static bool IsCompoundExpression(this Expression expression)
        {
            return logicalNodeTypes.Contains(expression.NodeType);
        }
        public static IList<IMetaExpression> Mash(this Expression expression)
        {
            return TreeMasher.Mash(expression);
        }
    }
}
