using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.DynamicProxy;

namespace LinFu.Persist
{
    public class TransparentCellWrapper : IInvokeWrapper
    {
        private ProxyFactory _factory = new ProxyFactory();
        private IDictionary<string, ICell> _target;
        private ICellEventHandler _handler;
        private IRow _parentRow;
        public TransparentCellWrapper(IRow parentRow, IDictionary<string, ICell> dictionary, ICellEventHandler handler)
        {
            _parentRow = parentRow;
            _target = dictionary;
            _handler = handler;
        }

        public void AfterInvoke(InvocationInfo info, object returnValue)
        {
        }

        public void BeforeInvoke(InvocationInfo info)
        {
            var targetMethod = info.TargetMethod;

            if (targetMethod.DeclaringType != typeof(IDictionary<string, ICell>))
                return;

            if (targetMethod.Name != "set_Item")
                return;

            if (_handler == null)
                return;

            var arguments = info.Arguments;

            string columnName = (string)arguments[0];
            ICell targetCell = arguments[1] as ICell;

            if (targetCell == null)
                return;

            // Track the cell only once
            if (targetCell is ICellEvents)
                return;

            var watcher = new CellWatcher(targetCell, _parentRow, columnName);
            ICell replacementCell = _factory.CreateProxy<ICell>(watcher, typeof(ICellEvents));

            // Track any changes made to the cell
            ICellEvents events = (ICellEvents)replacementCell;
            _handler.Attach(events);

            // Replace the real cell with the change tracker
            arguments[1] = replacementCell;

            return;
        }

        public object DoInvoke(InvocationInfo info)
        {
            return info.TargetMethod.Invoke(_target, info.Arguments);
        }
    }
}
