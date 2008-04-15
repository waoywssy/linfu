using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist
{
    internal static class TypeSystem
    {
        internal static Type GetElementType(Type seqType)
        {
            Type enumerableType = FindEnumerableType(seqType);
            if (enumerableType == null)
                return seqType;

            return enumerableType.GetGenericArguments()[0];
        }

        private static Type FindEnumerableType(Type seqType)
        {
            if (seqType == null || seqType == typeof(string))
                return null;

            if (seqType.IsArray)
                return typeof(IEnumerable<>).MakeGenericType(seqType.GetElementType());

            if (seqType.IsGenericType)
            {
                foreach (Type arg in seqType.GetGenericArguments())
                {
                    Type ienum = typeof(IEnumerable<>).MakeGenericType(arg);

                    if (ienum.IsAssignableFrom(seqType))
                        return ienum;
                }
            }

            var interfaces = seqType.GetInterfaces();
            if (interfaces != null && interfaces.Length > 0)
            {
                Type enumerableType = null;
                foreach (var currentType in interfaces)
                {
                    enumerableType = FindEnumerableType(currentType);
                    if (enumerableType != null)
                        return enumerableType;
                }
            }

            if (seqType.BaseType == null || seqType.BaseType == typeof(object))
                return null;


            return FindEnumerableType(seqType.BaseType);
        }

    }
}
