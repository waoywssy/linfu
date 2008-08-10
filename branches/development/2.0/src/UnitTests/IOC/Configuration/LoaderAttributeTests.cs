using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC;
using LinFu.IoC.Configuration;
using LinFu.IoC.Configuration.Interfaces;
using LinFu.IoC.Factories;
using Moq;
using NUnit.Framework;

namespace LinFu.UnitTests.IOC.Configuration
{
    [TestFixture]
    public class LoaderAttributeTests
    {
        [Test]
        public void NamedOncePerRequestFactoryMustBeCreatedFromTypeWithImplementsAttribute()
        {
            var implementingType = typeof(NamedOncePerRequestSampleService);
            var serviceType = typeof(ISampleService);
            var converter = new ImplementsAttributeFactoryLoader();

            TestFactoryConverterWith<OncePerRequestFactory<ISampleService>>("MyService",
                serviceType, implementingType, converter);
        }

        [Test]
        public void OncePerRequestFactoryMustBeCreatedFromTypeWithImplementsAttribute()
        {
            var implementingType = typeof(OncePerRequestSampleService);
            var serviceType = typeof(ISampleService);
            var converter = new ImplementsAttributeFactoryLoader();

            TestFactoryConverterWith<OncePerRequestFactory<ISampleService>>(string.Empty,
                serviceType, implementingType, converter);
        }

        [Test]
        public void NamedOncePerThreadFactoryMustBeCreatedFromTypeWithImplementsAttribute()
        {
            TestFactoryConverterWith<OncePerThreadFactory<ISampleService>>("MyService",
                typeof(ISampleService), typeof(NamedOncePerThreadSampleService), new ImplementsAttributeFactoryLoader());
        }

        [Test]
        public void OncePerThreadFactoryMustBeCreatedFromTypeWithImplementsAttribute()
        {
            TestFactoryConverterWith<OncePerThreadFactory<ISampleService>>(string.Empty,
                typeof(ISampleService), typeof(OncePerThreadSampleService), new ImplementsAttributeFactoryLoader());
        }
        [Test]
        public void SingletonFactoryMustBeCreatedFromTypeWithImplementsAttribute()
        {
            TestFactoryConverterWith<SingletonFactory<ISampleService>>(string.Empty,
                typeof(ISampleService), typeof(SingletonSampleService), new ImplementsAttributeFactoryLoader());
        }
        [Test]
        public void NamedSingletonFactoryMustBeCreatedFromTypeWithImplementsAttribute()
        {
            TestFactoryConverterWith<SingletonFactory<ISampleService>>("MyService",
                typeof(ISampleService), typeof(NamedSingletonSampleService), new ImplementsAttributeFactoryLoader());
        }
        private static void TestFactoryConverterWith<TFactory>(string serviceName, Type serviceType, Type implementingType, ITypeLoader loader)
            where TFactory : IFactory
        {
            // The loader should initialize the container with
            // the particular factory type
            var mockContainer = new Mock<IServiceContainer>();
            mockContainer.Expect(container =>
                container.AddFactory(serviceName, serviceType, It.Is<IFactory>(f => f != null && f is TFactory)));

            var factoryActions = loader.LoadContainerFrom(implementingType);
            Assert.IsNotNull(factoryActions, "The result cannot be null");
            Assert.IsTrue(factoryActions.Count() == 1, "There must be at least at least one result");

            // There must be at least one factory from
            // the result list
            var firstResult = factoryActions.FirstOrDefault();
            Assert.IsNotNull(firstResult);


            firstResult(mockContainer.Object);

            mockContainer.VerifyAll();
        }
    }
}
