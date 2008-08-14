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
using SampleLibrary;

namespace LinFu.UnitTests.IOC.Configuration
{
    [TestFixture]
    public class LoaderAttributeTests
    {
        #region ImplementsAttribute Tests
        [Test]
        public void NamedOncePerRequestFactoryMustBeCreatedFromTypeWithImplementsAttribute()
        {
            var implementingType = typeof(NamedOncePerRequestSampleService);
            var serviceType = typeof(ISampleService);
            var converter = new ImplementsAttributeLoader();

            TestFactoryConverterWith<OncePerRequestFactory<ISampleService>>("MyService",
                serviceType, implementingType, converter);
        }

        [Test]
        public void OncePerRequestFactoryMustBeCreatedFromTypeWithImplementsAttribute()
        {
            var implementingType = typeof(OncePerRequestSampleService);
            var serviceType = typeof(ISampleService);
            var converter = new ImplementsAttributeLoader();

            TestFactoryConverterWith<OncePerRequestFactory<ISampleService>>(string.Empty,
                serviceType, implementingType, converter);
        }

        [Test]
        public void NamedOncePerThreadFactoryMustBeCreatedFromTypeWithImplementsAttribute()
        {
            TestFactoryConverterWith<OncePerThreadFactory<ISampleService>>("MyService",
                typeof(ISampleService), typeof(NamedOncePerThreadSampleService), new ImplementsAttributeLoader());
        }

        [Test]
        public void OncePerThreadFactoryMustBeCreatedFromTypeWithImplementsAttribute()
        {
            TestFactoryConverterWith<OncePerThreadFactory<ISampleService>>(string.Empty,
                typeof(ISampleService), typeof(OncePerThreadSampleService), new ImplementsAttributeLoader());
        }
        [Test]
        public void SingletonFactoryMustBeCreatedFromTypeWithImplementsAttribute()
        {
            TestFactoryConverterWith<SingletonFactory<ISampleService>>(string.Empty,
                typeof(ISampleService), typeof(SingletonSampleService), new ImplementsAttributeLoader());
        }
        [Test]
        public void NamedSingletonFactoryMustBeCreatedFromTypeWithImplementsAttribute()
        {
            TestFactoryConverterWith<SingletonFactory<ISampleService>>("MyService",
                typeof(ISampleService), typeof(NamedSingletonSampleService), new ImplementsAttributeLoader());
        }
        #endregion

        [Test]
        public void FactoryAttributeLoaderMustInjectUnnamedCustomFactoryIntoContainer()
        {
            var mockContainer = new Mock<IServiceContainer>();
            var serviceType = typeof (ISampleService);
            var serviceName = string.Empty;

            // The container should add the expected
            // factory type
            mockContainer.Expect(container => container.AddFactory(serviceName, serviceType,
                                                                   It.IsAny<SampleFactory>()));

            ITypeLoader loader = new FactoryAttributeLoader();
            var actions = loader.LoadContainerFrom(typeof (SampleFactory));

            // The factory loader should return a bunch of actions
            // that will inject that custom factory into the container
            // itself
            foreach(var action in actions)
            {
                action(mockContainer.Object);
            }

            mockContainer.VerifyAll();
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
