using System;
using System.Runtime.Serialization;
using LinFu.IOC;
using Moq;
using NUnit.Framework;

namespace LinFu.UnitTests
{
	[TestFixture]
	public class InversionOfControlTests
	{
	    [Test]
	    public void ContainerMustHoldAnonymousFactoryInstance()
	    {
	        var mockFactory = new Mock<IFactory>();
	        var container = new SimpleContainer();

            // Give it a random service interface type
	        var serviceType = typeof (IDisposable);

            // Manually add the factory instance
	        container.AddFactory(serviceType, mockFactory.Object);
	        Assert.IsTrue(container.Contains(serviceType), "The container needs to have a factory for service type '{0}'", serviceType);
	    }

	    [Test]
	    public void ContainerMustReturnServiceInstance()
	    {
            var mockFactory = new Mock<IFactory>();
            var container = new SimpleContainer();

	        var serviceType = typeof (ISerializable);
	        var instance = new object();

	        container.AddFactory(serviceType, mockFactory.Object);

            // The container must call the IFactory.CreateInstance method
	        mockFactory.Expect(f => f.CreateInstance(container)).Returns(instance);
	        
            var result = container.GetService(serviceType);
	        Assert.IsNotNull(result, "The container failed to return the given service instance");
	        Assert.AreSame(instance, result, "The service instance returned does not match the given instance");

	        mockFactory.VerifyAll();
	    }

	    [Test]
	    public void ContainerMustBeAbleToSupressServiceNotFoundErrors()
	    {            
            var container = new SimpleContainer();
	        container.SuppressErrors = true;

            var instance = container.GetService(typeof(ISerializable));
            Assert.IsNull(instance, "The container is supposed to return a null instance");
	    }
	    [Test]
        [ExpectedException(typeof(ServiceNotFoundException))]
	    public void ContainerMustThrowErrorIfServiceNotFound()
	    {
            var container = new SimpleContainer();
            var instance = container.GetService(typeof(ISerializable));
            Assert.IsNull(instance, "The container is supposed to return a null instance");
	    }
	    [Test]
        public void ContainerMustHoldNamedFactoryInstance()
	    {
	        var mockFactory = new Mock<IFactory>();
            var container = new SimpleContainer();

            // Randomly assign an interface type
            // NOTE: The actual interface type doesn't matter
	        var serviceType = typeof (ISerializable);

	        container.AddFactory("MyService", serviceType, mockFactory.Object);
	        Assert.IsTrue(container.Contains("MyService", serviceType), "The container is supposed to contain a service named 'MyService'");

	        var instance = new object();
	        mockFactory.Expect(f => f.CreateInstance(container)).Returns(instance);

	        Assert.AreSame(instance, container.GetService("MyService", serviceType));
	    }

	    [Test]
        [ExpectedException(typeof(NamedServiceNotFoundException))]
	    public void ContainerMustBeAbleToSuppressNamedServiceNotFoundErrors()
	    {
            var container = new SimpleContainer();
            var instance = container.GetService("MyService", typeof(ISerializable));
            Assert.IsNull(instance, "The container is supposed to return a null instance");
	    }

	    [Test]
	    public void ContainerMustSupportGenericGetServiceMethod()
	    {
	        var mockService = new Mock<ISerializable>();
	        var mockFactory = new Mock<IFactory>();
	        var container = new SimpleContainer();

	        container.AddFactory(typeof (ISerializable), mockFactory.Object);
	        container.AddFactory("MyService", typeof (ISerializable), mockFactory.Object);

            // Return the mock ISerializable instance
	        mockFactory.Expect(f => f.CreateInstance(container)).Returns(mockService.Object);

            // Test the syntax
	        ISerializable result = container.GetService<ISerializable>();
	        Assert.AreSame(mockService.Object, result);

	        result = container.GetService<ISerializable>("MyService");
            Assert.AreSame(mockService.Object, result);
	    }

	    [Test]
	    public void ContainerMustSupportGenericAddFactoryMethod()
	    {
            var container = new SimpleContainer();
	        var mockFactory = new Mock<IFactory<ISerializable>>();
	        var mockService = new Mock<ISerializable>();

	        container.AddFactory(mockFactory.Object);
	        mockFactory.Expect(f => f.CreateInstance(container)).Returns(mockService.Object);

	        Assert.IsNotNull(container.GetService<ISerializable>());
	    }

	    [Test]
	    public void ContainerMustSupportNamedGenericAddFactoryMethod()
	    {
            var container = new SimpleContainer();
            var mockFactory = new Mock<IFactory<ISerializable>>();
            var mockService = new Mock<ISerializable>();

            container.AddFactory("MyService", mockFactory.Object);
            mockFactory.Expect(f => f.CreateInstance(container)).Returns(mockService.Object);

            Assert.IsNotNull(container.GetService<ISerializable>("MyService"));
	    }
	    [Test]
	    public void ContainerMustBeAbleToAddExistingServiceInstances()
	    {
            var container = new SimpleContainer();
            var mockService = new Mock<ISerializable>();
	        container.AddService<ISerializable>(mockService.Object);

	        var result = container.GetService<ISerializable>();
	        Assert.AreSame(result, mockService.Object);
	    }

	    [Test]
        [Ignore("TODO: Implement this")]
	    public void ContainerMustAllowServicesToBeIntercepted()
	    {
            // TODO: Wrap a decorator around an IContainer
            // instance
	        throw new NotImplementedException();
	    }

	    [Test]
        [Ignore("TODO: Implement this")]
	    public void ContainerMustAllowSurrogatesForNonExistentServiceInstances()
	    {
	        throw new NotImplementedException();
	    }
	}
}

