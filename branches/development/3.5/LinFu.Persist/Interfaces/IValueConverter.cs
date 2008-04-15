using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist
{
    public interface IValueConverter
    {
        bool CanConvertTo(Type targetType, Type sourceType);
        object ConvertTo(Type targetType, object value);
    }
}
