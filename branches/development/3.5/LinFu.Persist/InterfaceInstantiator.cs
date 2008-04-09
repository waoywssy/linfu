using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist
{
    public class InterfaceInstantiator : DefaultInstantiator
    {
        private Dictionary<Type, Type> _implementations = new Dictionary<Type, Type>();
        public void AddImplementation(Type baseType, Type implementingType)
        {
            _implementations[baseType] = implementingType;
        }
        public override object CreateNew(Type targetType)
        {
            if (targetType.IsInterface && _implementations.ContainsKey(targetType))
                return Activator.CreateInstance(_implementations[targetType]);

            return base.CreateNew(targetType);
        }
    }
}
