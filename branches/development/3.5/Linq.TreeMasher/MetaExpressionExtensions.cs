using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Linq.TreeMasher;
using System.Linq.Expressions;

namespace LinFu.Persist
{
    public static class MetaExpressionExtensions
    {
        public static IMetaExpression FindMetaExpression(this IList<IMetaExpression> list,
            Expression target)
        {
            var query = from m in list
                        where m.TargetExpression == target
                        select m;

            var results = query.ToList();
            if (results.Count == 0)
                return null;

            return results[0];
        }
        public static bool IsDescendantOf(this IMetaExpression meta, IMetaExpression other)
        {
            if (meta == other)
                return false;
            IList<IMetaExpression> parents = new List<IMetaExpression>();

            IMetaExpression parent = meta.Parent;
            while (parent != null)
            {
                parents.Add(parent);
                parent = parent.Parent;
            }

            return parents.Contains(other);
        }
        public static IEnumerable<IMetaExpression> GetAncestors(this IMetaExpression meta)
        {
            IList<IMetaExpression> parents = new List<IMetaExpression>();
            IMetaExpression parent = meta.Parent;
            while (parent != null)
            {
                parents.Add(parent);
                parent = parent.Parent;
            }
            return parents;
        }
        public static IEnumerable<IMetaExpression> GetDescendants<TChild>(this IMetaExpression meta)
        {
            var query = from m in meta.GetDescendants()
                        where m.TargetExpression is TChild
                        select m;

            return query.ToList();
        }
        public static IEnumerable<IMetaExpression> GetDescendants(this IMetaExpression meta)
        {
            IList<IMetaExpression> results = new List<IMetaExpression>();

            foreach (var child in meta.Children)
            {
                AddChildren(child, results);
            }

            return results;
        }
        private static void AddChildren(IMetaExpression current, IList<IMetaExpression> results)
        {
            results.Add(current);
            if (current.Children.Count == 0)
                return;

            foreach (var child in current.Children)
            {
                AddChildren(child, results);
            }
        }
    }
}
