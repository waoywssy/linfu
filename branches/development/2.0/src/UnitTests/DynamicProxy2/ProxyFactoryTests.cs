using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LinFu.AOP;
using LinFu.AOP.Interfaces;
using LinFu.DynamicProxy2;
using LinFu.DynamicProxy2.Interfaces;
using LinFu.IoC;
using LinFu.IoC.Configuration;
using LinFu.UnitTests.Tools;
using Moq;
using NUnit.Framework;
using SampleLibrary;
using SampleLibrary.DynamicProxy2;

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
            LoadAssemblyUsing(typeof(InvocationInfoEmitter));

            // Add the PEVerifier to the proxy generation process
            container.AddService<IVerifier>(new PEVerifier());
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
            Assert.IsTrue(factory.GetType() == typeof(ProxyFactory));
        }

        [Test]
        public void GeneratedProxyTypeMustHaveDefaultConstructor()
        {
            var factory = container.GetService<IProxyFactory>();
            Type proxyType = factory.CreateProxyType(typeof(object), new Type[0]);
            Assert.IsNotNull(proxyType);

            var constructor = proxyType.GetConstructor(new Type[0]);
            Assert.IsTrue(constructor != null);

            var instance = constructor.Invoke(new object[0]);
            Assert.IsNotNull(instance);
        }

        [Test]
        public void GeneratedProxyTypeMustCallInterceptorInstance()
        {
            var factory = container.GetService<IProxyFactory>();
            var mockInterceptor = new MockInterceptor(i => null);

            // Create the proxy instance and then make the call
            var proxyInstance = (IMoqTrigger)factory.CreateProxy(typeof(object), mockInterceptor, typeof(IMoqTrigger));
            proxyInstance.Execute();

            // The interceptor must be called
            Assert.IsTrue(mockInterceptor.Called);
        }
        [Test]
        public void GeneratedProxyTypeMustImplementIProxy()
        {
            var factory = container.GetService<IProxyFactory>();
            var proxyType = factory.CreateProxyType(typeof(object), new Type[] { typeof(ISampleService) });

            var instance = Activator.CreateInstance(proxyType);
            Assert.IsTrue(instance is IProxy);
            Assert.IsTrue(instance is ISampleService);
        }

        [Test]
        public void GeneratedProxyMustSupportRefArguments()
        {
            var factory = container.GetService<IProxyFactory>();

            // Assign the ref/out value for the int argument
            Func<IInvocationInfo, object> implementation = info =>
                                                               {
                                                                   info.Arguments[0] = 54321;
                                                                   return null;
                                                               };

            var interceptor = new MockInterceptor(implementation);
            var proxy = factory.CreateProxy<ClassWithVirtualByRefMethod>(interceptor);

            int value = 0;
            proxy.ByRefMethod(ref value);

            // The two given arguments should match
            Assert.AreEqual(54321, value);
        }

        [Test]
        public void GeneratedProxyMustSupportOutArguments()
        {
            var factory = container.GetService<IProxyFactory>();

            // Assign the ref/out value for the int argument
            Func<IInvocationInfo, object> implementation = info =>
            {
                info.Arguments[0] = 54321;
                return null;
            };

            var interceptor = new MockInterceptor(implementation);
            var proxy = factory.CreateProxy<ClassWithVirtualMethodWithOutParameter>(interceptor);

            int value;
            proxy.DoSomething(out value);

            // The two given arguments should match
            Assert.AreEqual(54321, value);
        }

        [Test]
        [Ignore("TODO: Implement this")]
        public void InvocationArgumentsMustMatchMethodCall()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void GeneratedProxyTypeMustSupportGenericMethodCalls()
        {
            var factory = container.GetService<IProxyFactory>();
            
            var genericParameterType = typeof(int);
            

            // The type arguments in the IInvocationInfo must match the
            // type arguments given in the method call

            Func<IInvocationInfo, object> methodBody = info =>
                                                           {
                                                               var method = info.TargetMethod;
                                                               Assert.IsTrue(info.TypeArguments.Contains(genericParameterType));
                                                               return null;
                                                           };

            var interceptor = new MockInterceptor(methodBody);
            var proxy = factory.CreateProxy<ClassWithGenericMethod>(interceptor);


            proxy.DoSomething<int>();
        }


        [Test]
        public void GeneratedProxyTypeMustSupportSubclassingFromGenericTypes()
        {
            var factory = container.GetService<IProxyFactory>();
            var actualList = new List<int>();

            Func<IInvocationInfo, object> implementation = info =>
                                                               {
                                                                   IList<int> list = actualList;
                                                                   return info.Proceed(list);
                                                               };
            var interceptor = new MockInterceptor(implementation);
            var proxy = factory.CreateProxy<IList<int>>(interceptor);

            // Any item added to the proxy list should be added to the 
            // actual list
            proxy.Add(12345);

            Assert.IsTrue(interceptor.Called);
            Assert.IsTrue(actualList.Count > 0);
            Assert.IsTrue(actualList[0] == 12345);
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
