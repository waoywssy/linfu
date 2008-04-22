using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.DynamicProxy;

namespace LinFu.Persist
{
    public class CellWatcher : IInvokeWrapper, ICellEvents
    {
        private ICell _cell;
        private IRow _parentRow;
        private string _columnName;
        public CellWatcher(ICell cell, IRow parentRow, string columnName)
        {
            _cell = cell;
            _columnName = columnName;
            _parentRow = parentRow;
        }
        public void AfterInvoke(InvocationInfo info, object returnValue)
        {
            var targetMethod = info.TargetMethod;
            if (targetMethod.DeclaringType != typeof(ICell))
                return;

            if (targetMethod.Name != "set_Value")
                return;

            if (Modified == null)
                return;

            CellEventArgs args = new CellEventArgs()
            {
                ColumnName = _columnName,
                TargetCell = _cell,
                ParentRow = _parentRow
            };

            Modified(_cell, args);
        }

        public void BeforeInvoke(InvocationInfo info)
        {
        }

        public object DoInvoke(InvocationInfo info)
        {
            var targetMethod = info.TargetMethod;
            object target = _cell;

            // Redirect all ICellEvent calls to this interceptor
            if (targetMethod.DeclaringType == typeof(ICellEvents))
                target = this;

            return targetMethod.Invoke(target, info.Arguments);
        }

        public event EventHandler<CellEventArgs> Modified;
    }
}
