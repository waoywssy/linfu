using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Linq.TreeMasher
{
    public class MetaExpression : IMetaExpression
    {
        private List<IMetaExpression> _children = new List<IMetaExpression>();
        #region IMetaExpression Members

        public int Depth
        {
            get;
            set;
        }

        public IMetaExpression Parent
        {
            get;
            set;
        }

        public Expression TargetExpression
        {
            get;
            set;
        }

        public IList<IMetaExpression> Children
        {
            get
            {
                return _children;
            }
        }

        public Type ExpressionType
        {
            get
            {
                return TargetExpression.GetType();
            }
        }

        public int Sequence
        {
            get;
            set;
        }

        #endregion
        public override string ToString()
        {
            string result = base.ToString();
            if (TargetExpression != null)
                result = string.Format("[Depth: {0}, Sequence: {1}] {2}", Depth, Sequence, this.TargetExpression.ToString());

            return result;
        }
    }
}
