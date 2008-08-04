using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.IoC.Factories
{
    public abstract class BaseFactory<T> : IFactory<T>, IFactory
    {       
        public abstract T CreateInstance(IContainer container);
        
        object IFactory.CreateInstance(IContainer container)
        {
            return (T) CreateInstance(container);
        }
    }
}
