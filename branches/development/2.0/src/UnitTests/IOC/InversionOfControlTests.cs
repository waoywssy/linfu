using System;
using System.Runtime.Serialization;
using LinFu.IoC;
using LinFu.IoC.Configuration;
using LinFu.IoC.Interfaces;
using Moq;
using NUnit.Framework;
using SampleLibrary;

namespace LinFu.UnitTests.IOC
{
    [TestFixture]
    public class InversionOfControlTests
    {
        [Test]
        [Ignore("TODO: Implement this")]
        public void ContainerMustAllowServicesToBeIntercepted()
        {
            // TODO: Wrap a decorator around an IContainer
            // instance
            throw new NotImplementedException();
        }

        [Test]
        public void ContainerMustAllowSurrogatesForNonExistentServiceInstances()
        {
            var container = new ServiceContainer();
            var mockService = new Mock<ISampleService>();
            var surrogate = mockService.Object;
            container.Inject<ISampleService>().Using(f => surrogate).OncePerRequest();

            var result = container.GetService<ISampleService>();
            Assert.IsNotNull(result);
            Assert.AreSame(surrogate, result);
        }

        [Test]        
        public void InitializerShouldOnlyBeCalledOncePerLifetime()
        {
            var container = new ServiceContainer();
            var mockService = new Mock<IInitialize>();

            VerifyInitializeCall(container, mockService);
            VerifyInitializeCall(container, new Mock<IInitialize>());
        }

        private static void VerifyInitializeCall(ServiceContainer container, Mock<IInitialize> mockService)
        {
            container.LoadFrom(AppDomain.CurrentDomain.BaseDirectory, "*.dll");                        
            container.AddService(mockService.Object);


            mockService.Expect(i => i.Initialize(container)).AtMostOnce();

            // The container should return the same instance
            var firstResult = container.GetService<IInitialize>();
            var secondResult = container.GetService<IInitialize>();
            Assert.AreSame(firstResult, secondResult);

            // The Initialize() method should only be called once
            mockService.Verify();
        }

        [Test]
        public void ContainerMustBeAbleToAddExistingServiceInstances()
        {
            var container = new ServiceContainer();
            var mockService = new Mock<ISerializable>();
            container.AddService(mockService.Object);

            var result = container.GetService<ISerializable>();
            Assert.AreSame(result, mockService.Object);
        }

        [Test]
        [ExpectedException(typeof (NamedServiceNotFoundException))]
        public void ContainerMustBeAbleToSuppressNamedServiceNotFoundErrors()
        {
            var container = new ServiceContainer();
            object instance = container.GetService("MyService", typeof (ISerializable));
            Assert.IsNull(instance, "The container is supposed to return a null instance");
        }

        [Test]
        public void ContainerMustBeAbleToSupressServiceNotFoundErrors()
        {
            var container = new ServiceContainer();
            container.SuppressErrors = true;

            object instance = container.GetService(typeof (ISerializable));
            Assert.IsNull(instance, "The container is supposed to return a null instance");
        }

        [Test]
        public void ContainerMustCallPostProcessorDuringARequest()
        {
            var mockPostProcessor = new Mock<IPostProcessor>();
            var container = new ServiceContainer();
            container.PostProcessors.Add(mockPostProcessor.Object);

            mockPostProcessor.Expect(p =>
                                     p.PostProcess(It.Is<IServiceRequestResult>(result => result != null)));

            container.SuppressErrors = true;
            container.GetService<ISerializable>();

            mockPostProcessor.VerifyAll();
        }

        [Test]
        public void ContainerMustHoldAnonymousFactoryInstance()
        {
            var mockFactory = new Mock<IFactory>();
            var container = new ServiceContainer();

            // Give it a random service interface type
            Type serviceType = typeof (IDisposable);

            // Manually add the factory instance
            container.AddFactory(serviceType, mockFactory.Object);
            Assert.IsTrue(container.Contains(serviceType), "The container needs to have a factory for service type '{0}'", serviceType);
        }

        [Test]
        public void ContainerMustHoldNamedFactoryInstance()
        {
            var mockFactory = new Mock<IFactory>();
            var container = new ServiceContainer();

            // Randomly assign an interface type
            // NOTE: The actual interface type doesn't matter
            Type serviceType = typeof (ISerializable);

            container.AddFactory("MyService", serviceType, mockFactory.Object);
            Assert.IsTrue(container.Contains("MyService", serviceType), "The container is supposed to contain a service named 'MyService'");

            var instance = new object();
            mockFactory.Expect(f => f.CreateInstance(serviceType, container)).Returns(instance);

            Assert.AreSame(instance, container.GetService("MyService", serviceType));
        }

        [Test]
        public void ContainerMustReturnServiceInstance()
        {
            var mockFactory = new Mock<IFactory>();
            var container = new ServiceContainer();

            Type serviceType = typeof (ISerializable);
            var instance = new object();

            container.AddFactory(serviceType, mockFactory.Object);

            // The container must call the IFactory.CreateInstance method
            mockFactory.Expect(f => f.CreateInstance(serviceType, container)).Returns(instance);

            object result = container.GetService(serviceType);
            Assert.IsNotNull(result, "The container failed to return the given service instance");
            Assert.AreSame(instance, result, "The service instance returned does not match the given instance");

            mockFactory.VerifyAll();
        }

        [Test]
        public void ContainerMustSupportGenericAddFactoryMethod()
        {
            var container = new ServiceContainer();
            var mockFactory = new Mock<IFactory<ISerializable>>();
            var mockService = new Mock<ISerializable>();

            container.AddFactory(mockFactory.Object);
            mockFactory.Expect(f => f.CreateInstance(container)).Returns(mockService.Object);

            Assert.IsNotNull(container.GetService<ISerializable>());
        }

        [Test]
        public void ContainerMustSupportGenericGetServiceMethod()
        {
            var mockService = new Mock<ISerializable>();
            var mockFactory = new Mock<IFactory>();
            var container = new ServiceContainer();

            container.AddFactory(typeof (ISerializable), mockFactory.Object);
            container.AddFactory("MyService", typeof (ISerializable), mockFactory.Object);

            // Return the mock ISerializable instance
            mockFactory.Expect(f => f.CreateInstance(typeof (ISerializable), container)).Returns(mockService.Object);

            // Test the syntax
            var result = container.GetService<ISerializable>();
            Assert.AreSame(mockService.Object, result);

            result = container.GetService<ISerializable>("MyService");
            Assert.AreSame(mockService.Object, result);
        }

        [Test]
        public void ContainerMustSupportNamedGenericAddFactoryMethod()
        {
            var container = new ServiceContainer();
            var mockFactory = new Mock<IFactory<ISerializable>>();
            var mockService = new Mock<ISerializable>();

            container.AddFactory("MyService", mockFactory.Object);
            mockFactory.Expect(f => f.CreateInstance(container)).Returns(mockService.Object);

            Assert.IsNotNull(container.GetService<ISerializable>("MyService"));
        }

        [Test]
        [ExpectedException(typeof (ServiceNotFoundException))]
        public void ContainerMustThrowErrorIfServiceNotFound()
        {
            var container = new ServiceContainer();
            object instance = container.GetService(typeof (ISerializable));
            Assert.IsNull(instance, "The container is supposed to return a null instance");
        }

        [Test]
        public void ContainerMustUseUnnamedAddFactoryMethodIfNameIsEmpty()
        {
            var mockFactory = new Mock<IFactory>();
            var mockService = new Mock<ISerializable>();

            var container = new ServiceContainer();

            Type serviceType = typeof (ISerializable);

            // Add the service using a blank name;
            // the container should register this factory
            // as if it had no name
            container.AddFactory(string.Empty, serviceType, mockFactory.Object);
            mockFactory.Expect(f => f.CreateInstance(serviceType, container)).Returns(mockService.Object);

            // Verify the result
            var result = container.GetService<ISerializable>();
            Assert.AreSame(mockService.Object, result);
        }

        [Test]
        public void ContainerMustUseUnnamedContainsMethodIfNameIsEmpty()
        {
            var mockFactory = new Mock<IFactory>();
            var mockService = new Mock<ISerializable>();
            var container = new ServiceContainer();

            Type serviceType = typeof (ISerializable);

            // Use unnamed AddFactory method
            container.AddFactory(serviceType, mockFactory.Object);

            // The container should use the
            // IContainer.Contains(Type) method instead of the
            // IContainer.Contains(string, Type) method if the
            // service name is blank
            Assert.IsTrue(container.Contains(string.Empty, typeof (ISerializable)));
        }

        [Test]
        public void ContainerMustUseUnnamedGetServiceMethodIfNameIsEmpty()
        {
            var mockFactory = new Mock<IFactory>();
            var mockService = new Mock<ISerializable>();
            var container = new ServiceContainer();


            Type serviceType = typeof (ISerializable);
            mockFactory.Expect(f => f.CreateInstance(It.IsAny<Type>(), container)).Returns(mockService.Object);
            container.AddFactory(serviceType, mockFactory.Object);

            object result = container.GetService(string.Empty, serviceType);

            Assert.AreSame(mockService.Object, result);
        }
        [Test]
        public void ContainerMustAllowInjectingCustomFactoriesForOpenGenericTypeDefinitions()
        {
            var container = new ServiceContainer();
            var factory = new SampleOpenGenericFactory();

            container.AddFactory(typeof(ISampleGenericService<>), factory);

            // The container must report that it *can* create
            // the generic service type 
            Assert.IsTrue(container.Contains(typeof(ISampleGenericService<int>)));

            var result = container.GetService<ISampleGenericService<int>>();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.GetType() == typeof(SampleGenericImplementation<int>));
        }

        [Test]
        public void ContainerMustAllowInjectingCustomFactoriesForNamedOpenGenericTypeDefinitions()
        {
            var container = new ServiceContainer();
            var factory = new SampleOpenGenericFactory();
            var serviceName = "MyService";

            container.AddFactory(serviceName, typeof(ISampleGenericService<>), factory);

            // The container must report that it *can* create
            // the generic service type 
            Assert.IsTrue(container.Contains(serviceName, typeof(ISampleGenericService<int>)));

            var result = container.GetService<ISampleGenericService<int>>(serviceName);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.GetType() == typeof(SampleGenericImplementation<int>));
        }

        [Test]
        public void ContainerMustCallIInitializeOnServicesCreatedFromCustomFactory()
        {
            var mockFactory = new Mock<IFactory>();
            var mockInitialize = new Mock<IInitialize>();
            
            mockFactory.Expect(f => f.CreateInstance(It.IsAny<Type>(), It.IsAny<IServiceContainer>()))
                .Returns(mockInitialize.Object);

            // The IInitialize instance must be called once it
            // leaves the custom factory
            mockInitialize.Expect(i=>i.Initialize(It.IsAny<IServiceContainer>()));

            var container = new ServiceContainer();
            container.LoadFrom(AppDomain.CurrentDomain.BaseDirectory, "LinFu*.dll");
            container.AddFactory(typeof(IInitialize), mockFactory.Object);

            var result = container.GetService<IInitialize>();
            
            mockFactory.VerifyAll();
            mockInitialize.VerifyAll();
        }
    }
}