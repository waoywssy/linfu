using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist
{
    public interface IPropertyMappingRegistry
    {
        bool HasPropertyMapping(Type declaringType, string propertyName, Type propertyType);
        IPropertyMapping GetPropertyMapping(Type declaringType, string propertyName, Type propertyType);
    }
}
