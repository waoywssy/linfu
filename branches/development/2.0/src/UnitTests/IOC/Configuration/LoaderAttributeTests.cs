﻿using System;
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
        public void OncePerRequestFactoryMustBeCreatedFromTypeWithImplementsAttribute()
        {
            var implementingType = typeof(OncePerRequestSampleService);
            var serviceType = typeof(ISampleService);
            var converter = new ImplementsAttributeFactoryLoader();

            TestFactoryConverterWith<OncePerRequestFactory<ISampleService>>(string.Empty,
                serviceType, implementingType, converter);
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
        private void TestFactoryConverterWith<TFactory>(string serviceName, Type serviceType, Type implementingType, IFactoryLoader loader)
            where TFactory : IFactory
        {
            // The loader should initialize the container with
            // the particular factory type
            var mockContainer = new Mock<IServiceContainer>();
            mockContainer.Expect(container =>
                container.AddFactory(serviceName, serviceType, It.Is<IFactory>(f => f != null && f is TFactory)));

            var factoryActions = loader.LoadFactoriesFrom(implementingType);
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
