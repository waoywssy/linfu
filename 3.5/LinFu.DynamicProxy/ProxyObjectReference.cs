using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Reflection;

namespace LinFu.DynamicProxy
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
            var interfaceList = new List<Type>();
            int interfaceCount = info.GetInt32("__baseInterfaceCount");
            for(int i = 0; i < interfaceCount; i++)
            {
                var keyName = string.Format("__baseInterface{0}", i);
                var currentQualifiedName = info.GetString(keyName);
                Type interfaceType = Type.GetType(currentQualifiedName, true, false);

                interfaceList.Add(interfaceType);
            }

            // Reconstruct the proxy
            var factory = new ProxyFactory();
            var proxyType = factory.CreateProxyType(_baseType, interfaceList.ToArray());

            // Initialize the proxy with the deserialized data
            var args = new object[] { info, context };
            _proxy = (IProxy)Activator.CreateInstance(proxyType, args);
        }
        #region IObjectReference Members

        public object GetRealObject(StreamingContext context)
        {
            return _proxy;
        }

        #endregion

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {

        }

        #endregion
    }
}
