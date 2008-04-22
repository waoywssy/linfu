using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.DynamicProxy;

namespace LinFu.Persist
{
    public class TransparentRowCollectionWrapper : IInvokeWrapper
    {
        private ICollection<IRow> _target;
        public TransparentRowCollectionWrapper(ICollection<IRow> target)
        {
            _target = target;
        }
        public void AfterInvoke(InvocationInfo info, object returnValue)
        {
        }

        public void BeforeInvoke(InvocationInfo info)
        {
            var targetMethod = info.TargetMethod;
            var declaringType = targetMethod.DeclaringType;
            var arguments = info.Arguments;
            var argCount = arguments == null ? 0 : arguments.Length;

            if (declaringType != typeof(ICollection<IRow>))
                return;

            if (targetMethod.Name != "Add")
                return;

            if (argCount != 1)
                return;

            var row = arguments[0] as IRow;
            if (row == null)
                return;

            // Replace the real row with a tracked row
            IRow replacementRow = new TrackedRow(row);
            arguments[0] = replacementRow;
        }

        public object DoInvoke(InvocationInfo info)
        {
            return info.TargetMethod.Invoke(_target, info.Arguments);
        }
    }
}
