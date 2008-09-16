﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using LinFu.IoC;
using LinFu.IoC.Extensions;
using LinFu.IoC.Extensions.Interfaces;
using LinFu.IoC.Interfaces;
using Moq;
using NUnit.Framework;
using SampleLibrary;
using SampleLibrary.IOC;

namespace LinFu.UnitTests.IOC
{
    [TestFixture]
    public class ResolutionTests
    {
        [Test]
        public void ShouldConvertParameterInfoIntoPredicateThatChecksIfParameterTypeExistsAsService()
        {
            // Initialize the container so that it contains
            // an instance of ISampleService
            var mockSampleService = new Mock<ISampleService>();
            var container = new ServiceContainer();

            container.AddService(mockSampleService.Object);

            var constructor = typeof(SampleClassWithSingleArgumentConstructor)
                .GetConstructor(new Type[] { typeof(ISampleService) });

            var parameters = constructor.GetParameters();
            var firstParameter = parameters.First();

            Assert.IsNotNull(firstParameter);

            Func<IServiceContainer, bool> mustExist = firstParameter.ParameterType.MustExistInContainer();
            Assert.IsTrue(mustExist(container));
        }

        [Test]
        public void ShouldConvertTypeIntoPredicateThatChecksIfTypeExistsInContainerAsServiceList()
        {
            var mockSampleService = new Mock<ISampleService>();
            var container = new ServiceContainer();

            // Add a bunch of dummy services
            for (var i = 0; i < 10; i++)
            {
                var serviceName = string.Format("Service{0}", i + 1);
                container.AddService(serviceName, mockSampleService.Object);
            }

            var services = container.GetServices<ISampleService>();
            Assert.IsTrue(services.Count() == 10);

            foreach(var service in services)
            {
                Assert.AreSame(mockSampleService.Object, service);
            }

            Func<IServiceContainer, bool> predicate = typeof (IEnumerable<ISampleService>)
                .ExistsAsServiceList();

            Assert.IsTrue(predicate(container));
        }

        [Test]
        public void ShouldResolveConstructorWithMostResolvableParametersFromContainer()
        {
            var mockSampleService = new Mock<ISampleService>();
            IServiceContainer container = new ServiceContainer();

            // Add an ISampleService instance
            container.AddService(mockSampleService.Object);
            container.AddService<IConstructorResolver>(new ConstructorResolver());
            var resolver = container.GetService<IConstructorResolver>();
            Assert.IsNotNull(resolver);
                        
            // The resolver should return the constructor with two ISampleService parameters
            var expectedConstructor = typeof(SampleClassWithMultipleConstructors).GetConstructor(new [] {typeof(ISampleService), typeof(ISampleService)});
            Assert.IsNotNull(expectedConstructor);

            ConstructorInfo result = resolver.ResolveFrom(typeof(SampleClassWithMultipleConstructors), container);
            Assert.AreSame(expectedConstructor, result);
        }
    }
}
