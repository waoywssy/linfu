using System;
using System.Collections.Generic;

namespace LinFu.DynamicProxy
{
    public struct ProxyCacheEntry
    {
        private readonly int _hashCode;
        public Type BaseType;
        public Type[] Interfaces;

        public ProxyCacheEntry(Type baseType, Type[] interfaces)
        {
            if (baseType == null)
                throw new ArgumentNullException("baseType");

            BaseType = baseType;
            Interfaces = interfaces;
            if (interfaces == null || interfaces.Length == 0)
            {
                _hashCode = baseType.GetHashCode();
                return;
            }

            _hashCode = baseType.GetHashCode();
            foreach (Type type in interfaces)
            {
                if (type == null)
                    continue;

                _hashCode ^= type.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ProxyCacheEntry))
                return false;

            ProxyCacheEntry other = (ProxyCacheEntry)obj;
            return _hashCode == other.GetHashCode();
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }
    }
}
