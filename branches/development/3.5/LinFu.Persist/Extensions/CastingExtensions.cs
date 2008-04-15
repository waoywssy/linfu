using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist
{
    public static class CastingExtensions
    {
        public static T As<T>(this object target)
            where T : class
        {
            if (target == null)
                return null;

            T result = target as T;
            return result;
        }
    }
}
