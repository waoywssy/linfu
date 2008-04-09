using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace LinFu.Persist
{
    public class CompositePropertyAssignmentBehavior : IPropertyAssignmentBehavior
    {
        private List<IPropertyAssignmentBehavior> _behaviors = new List<IPropertyAssignmentBehavior>();

        public IList<IPropertyAssignmentBehavior> Behaviors
        {
            get { return _behaviors; }
        }

        public bool CanModify(PropertyInfo targetProperty, IRow currentRow)
        {
            var targetTable = currentRow.Table;
            var matches = from b in _behaviors
                          where b.CanModify(targetProperty, currentRow)
                          select b;

            return matches.Count() > 0;
        }

        public void AssignPropertyValue(object target, PropertyInfo targetProperty, IRow sourceRow)
        {
            var propertyName = targetProperty.Name;
            var matches = (from b in _behaviors
                           where b.CanModify(targetProperty, sourceRow)
                           select b).ToList();

            if (matches.Count == 0)
                return;

            var match = matches[0];
            match.AssignPropertyValue(target, targetProperty, sourceRow);
        }
    }
}
