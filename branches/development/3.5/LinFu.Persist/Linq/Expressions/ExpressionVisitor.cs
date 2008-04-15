using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.Visitors;
using Linq.TreeMasher;

namespace LinFu.Persist
{
    public abstract class ExpressionVisitor : IVisitor<IMetaExpression>
    {
        public MetaVisitor<IMetaExpression> Visitor { get; set; }

        public abstract void Visit(IMetaExpression expression);
        public abstract bool Matches(IMetaExpression expression);
    }
}
