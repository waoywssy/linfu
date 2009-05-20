﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using LinFu.Proxy.Interfaces;

namespace LinFu.Proxy
{
    [Serializable]
    public class ProxyObjectReference : IObjectReference, ISerializable
    {
        private readonly Type _baseType;
        private readonly IProxy _proxy;

        protected ProxyObjectReference(SerializationInfo info, StreamingContext context)
        {
            // Deserialize the base type using its assembly qualified name
            string qualifiedName = info.GetString("__baseType");
            _baseType = Type.GetType(qualifiedName, true, false);

            // Rebuild the list of interfaces
            List<Type> interfaceList = new List<Type>();
            int interfaceCount = info.GetInt32("__baseInterfaceCount");
            for (int i = 0; i < interfaceCount; i++)
            {
                string keyName = string.Format("__baseInterface{0}", i);
                string currentQualifiedName = info.GetString(keyName);
                Type interfaceType = Type.GetType(currentQualifiedName, true, false);

                interfaceList.Add(interfaceType);
            }

            // Reconstruct the proxy
            ProxyFactory factory = new ProxyFactory();
            Type proxyType = factory.CreateProxyType(_baseType, interfaceList.ToArray());

            // Initialize the proxy with the deserialized data
            object[] args = new object[] { info, context };
            _proxy = (IProxy)Activator.CreateInstance(proxyType, args);
        }

        public object GetRealObject(StreamingContext context)
        {
            return _proxy;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
        }
    }
}
