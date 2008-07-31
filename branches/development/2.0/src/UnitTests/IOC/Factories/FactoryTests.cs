using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using LinFu.IOC;
using LinFu.IOC.Factories;
using Moq;
using NUnit.Framework;

namespace LinFu.UnitTests.IOC
{
    [TestFixture]
    public class FactoryTests
    {
        private Func<IContainer, ISerializable> createInstance;
        [SetUp]
        public void Init()
        {
            // Create a new mock service instance on each
            // factory method call
            createInstance = 
                container => (new Mock<ISerializable>()).Object;
        }
        
        [TearDown]
        public void Term()
        {
            createInstance = null;
        }

        [Test]
        public void OncePerRequestFactoryShouldCreateUniqueInstances()
        {
            var factory = new OncePerRequestFactory<ISerializable>(createInstance);

            var first = factory.CreateInstance(null);
            var second = factory.CreateInstance(null);

            // Both instances must be unique
            Assert.AreNotSame(first, second);
            Assert.IsNotNull(first);
            Assert.IsNotNull(second);
        }

        [Test]
        public void OncePerThreadFactoryShouldCreateUniqueInstancesAmongThreads()
        {
            throw new NotImplementedException();
        }
        [Test]
        public void SingletonFactoryShouldCreateTheSameInstanceOnce()
        {
            var factory = new SingletonFactory<ISerializable>(createInstance);

            var first = factory.CreateInstance(null);
            var second = factory.CreateInstance(null);

            // Both instances must be the same
            Assert.AreSame(first, second);
            Assert.IsNotNull(first);
            Assert.IsNotNull(second);
        }
        [Test]
        public void GenericFactoryAdapterShouldCallUntypedFactoryInstance()
        {
            var container = new SimpleContainer();
            var mockFactory = new Mock<IFactory<ISerializable>>();
            var mockService = new Mock<ISerializable>();
            var adapter = new FactoryAdapter<ISerializable>(mockFactory.Object);

            // The adapter itself should call the container on creation
            mockFactory.Expect(f => f.CreateInstance(container)).Returns(mockService.Object);

            Assert.IsInstanceOfType(typeof(IFactory), adapter);

            adapter.CreateInstance(container);

            mockFactory.VerifyAll();
        }
    }
}
