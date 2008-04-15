using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Linq.TreeMasher
{
    public interface IMetaExpression
    {
        int Depth { get; set; }
        int Sequence { get; set; }
        IMetaExpression Parent { get; set; }
        Expression TargetExpression { get; set; }
        Type ExpressionType { get; }
        IList<IMetaExpression> Children { get; }
    }
}
