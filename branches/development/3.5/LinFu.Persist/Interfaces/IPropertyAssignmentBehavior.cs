using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace LinFu.Persist
{
    public interface IPropertyAssignmentBehavior
    {
        bool CanModify(PropertyInfo targetProperty, IRow currentRow);
        void AssignPropertyValue(object target, PropertyInfo targetProperty, IRow sourceRow);
    }
}
