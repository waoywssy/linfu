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
