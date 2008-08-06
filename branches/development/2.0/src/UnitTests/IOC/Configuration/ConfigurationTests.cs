using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC;
using LinFu.IoC.Configuration;
using LinFu.IoC.Configuration.Interfaces;
using LinFu.IoC.Factories;
using NUnit.Framework;
using Moq;

namespace LinFu.UnitTests.IOC.Configuration
{
    [TestFixture]
    public class ConfigurationTests
    {
        [Test]        
        public void LoaderMustLoadUnnamedFactoriesWithFactoryAttributeFromAnAssembly()
        {
            var mockContainer = new Mock<IContainer>();
            var containerInstance = mockContainer.Object;

            // If this works, the loader will construct the factory
            // and add it to the container instance
            mockContainer.Expect(container =>
                                 container.AddFactory(typeof (ISampleService), 
                                 It.Is<IFactory>(f=>f != null)));

            // TODO: Fix this
            //var loader = new Loader(containerInstance);
            //loader.Load(typeof (ConfigurationTests).Assembly);
            throw new NotImplementedException();
        }

        [Test]
        public void LoaderMustLoadNamedFactoriesWithFactoryAttributeAnAssembly()
        {
            throw new NotImplementedException();
        }
        [Test]
        public void LoaderMustSignalToPluginsWhenTheLoadBegins()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void LoaderMustSignalToPluginsWhenTheLoadEnds()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void CreatedServicesMustBeAbleToInitializeThemselves()
        {
            throw new NotImplementedException();
        }
        [Test]
        public void OncePerRequestFactoryMustBeCreatedFromTypeWithImplementsAttribute()
        {
            var implementingType = typeof(OncePerRequestSampleService);
            var serviceType = typeof (ISampleService);
            var converter = new ImplementsAttributeConverter();

            TestFactoryConverterWith<OncePerRequestFactory<ISampleService>>(serviceType, implementingType, converter);
        }

        [Test]
        public void OncePerThreadFactoryMustBeCreatedFromTypeWithImplementsAttribute()
        {
            throw new NotImplementedException();
        }
        [Test]
        public void SingletonFactoryMustBeCreatedFromTypeWithImplementsAttribute()
        {
            throw new NotImplementedException();
        }
        private void TestFactoryConverterWith<TFactory>(Type serviceType, Type implementingType, IFactoryConverter converter)
            where TFactory : IFactory
        {
            IEnumerable<IFactory> factories = converter.CreateFactoriesFrom(implementingType);
            Assert.IsNotNull(factories, "The result cannot be null");
            Assert.IsTrue(factories.Count() == 1, "The converter must return at least one result");

            // There must be at least one factory from
            // the result list
            var factory = factories.FirstOrDefault();
            Assert.IsNotNull(factory);
            Assert.IsTrue(factory is TFactory);


            // The service must be compatible with the service type
            // and match the implementing type itself
            var serviceInstance = factory.CreateInstance(null);
            Assert.IsAssignableFrom(implementingType, serviceInstance);
            Assert.IsInstanceOfType(serviceType, serviceInstance);
        }

        [Test]
        public void FactoryMustBeCreatedFromTypeWithNamedImplementsAttribute()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void NamedFactoryMustBeCreatedFromTypeWithFactoryAttribute()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void UnnamedFactoryMustBeCreatedFromTypeWithFactoryAttribute()
        {
            throw new NotImplementedException();
        }
    }    
}
