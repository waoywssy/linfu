using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist
{
    public class DefaultInstantiator : IInstantiator
    {
        public virtual object CreateNew(Type targetType)
        {
            return Activator.CreateInstance(targetType);
        }
    }
}
