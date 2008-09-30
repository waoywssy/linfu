﻿using System;
using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;

namespace SampleLibrary
{
    [Factory(typeof (ISampleGenericService<>))]
    public class SampleOpenGenericFactory : IFactory
    {
        #region IFactory Members

        public object CreateInstance(Type serviceType, IContainer container, params object[] additionalArguments)
        {
            Type typeArgument = serviceType.GetGenericArguments()[0];
            var resultType = typeof (SampleGenericImplementation<>).MakeGenericType(typeArgument);

            return Activator.CreateInstance(resultType);
        }

        #endregion
    }
}