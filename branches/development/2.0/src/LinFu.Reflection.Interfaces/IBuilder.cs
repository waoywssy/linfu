using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Reflection
{
    public interface IBuilder<TContext, TTarget>
    {
        void Construct(TContext context, TTarget target);
    }
}
