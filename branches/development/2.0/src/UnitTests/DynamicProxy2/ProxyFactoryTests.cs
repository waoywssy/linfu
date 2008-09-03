using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LinFu.AOP;
using LinFu.DynamicProxy2;
using LinFu.DynamicProxy2.Interfaces;
using LinFu.IoC;
using LinFu.IoC.Configuration;
using NUnit.Framework;

namespace LinFu.UnitTests.DynamicProxy2
{
    [TestFixture]
    public class ProxyFactoryTests : BaseTestFixture
    {
        private ServiceContainer container;
        private Loader loader;
        public override void Init()
        {
            loader = new Loader();
            container = new ServiceContainer();

            container.LoadFrom(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
            
            LoadAssemblyUsing(typeof(ProxyFactory));
            LoadAssemblyUsing(typeof (InvocationInfoEmitter));
        }

        private void LoadAssemblyUsing(Type embeddedType)
        {
            var location = embeddedType.Assembly.Location;
            var directory = Path.GetDirectoryName(location);
            var filename = Path.GetFileName(location);
            
            container.LoadFrom(directory, filename);
        }

        public override void Term()
        {
            loader = null;
            container = null;
        }

        [Test]
        public void DefaultProxyFactoryMustExist()
        {
            var factory = container.GetService<IProxyFactory>();
            Assert.IsNotNull(factory);
            Assert.IsTrue(factory.GetType() == typeof (ProxyFactory));
        }

        [Test]
        public void GeneratedProxyTypeMustHaveDefaultConstructor()
        {
            var factory = container.GetService<IProxyFactory>();
            Type proxyType = factory.CreateProxyType(typeof (object), new Type[0]);
            Assert.IsNotNull(proxyType);

            var constructor = proxyType.GetConstructor(new Type[0]);
            Assert.IsTrue(constructor != null);

            var instance = constructor.Invoke(new object[0]);
            Assert.IsNotNull(instance);
        }

        [Test]
        [Ignore("TODO: Implement this")]
        public void GeneratedProxyTypeMustCallInterceptorInstance()
        {
            throw new NotImplementedException();
        }
        [Test]
        [Ignore("TODO: Implement this")]
        public void GeneratedProxyTypeMustImplementIProxy()
        {
            throw new NotImplementedException();
        }

        [Test]
        [Ignore("TODO: Implement this")]
        public void GeneratedProxyMustSupportRefArguments()
        {
            throw new NotImplementedException();
        }

        [Test]
        [Ignore("TODO: Implement this")]
        public void GeneratedProxyMustSupportOutArguments()
        {
            throw new NotImplementedException();
        }

        [Test]
        [Ignore("TODO: Implement this")]
        public void InvocationArgumentsMustMatchMethodCall()
        {
            throw new NotImplementedException();
        }
        [Test]
        [Ignore("TODO: Implement this")]
        public void ProxyFactoryMustGenerateValidIProxyInstance()
        {
            throw new NotImplementedException();
        }

        [Test]
        [Ignore("TODO: Implement this")]
        public void GeneratedProxyTypeMustSupportGenericMethodCalls()
        {
            throw new NotImplementedException();
        }

        [Test]
        [Ignore("TODO: Implement this")]
        public void GeneratedProxyTypeMustImplementGivenInterfaces()
        {
            throw new NotImplementedException();
        }

        [Test]
        [Ignore("TODO: Implement this")]
        public void GeneratedProxyTypeMustSupportSerialization()
        {
            throw new NotImplementedException();
        }
    }
}
