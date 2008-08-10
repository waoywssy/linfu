using System;
using System.Collections.Generic;
using System.IO;
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
                                 container.AddFactory(typeof(ISampleService),
                                 It.Is<IFactory>(f => f != null)));

            // TODO: Fix this
            //var loader = new Loader(containerInstance);
            //loader.Load(typeof (ConfigurationTests).Assembly);
            throw new NotImplementedException();
        }

        [Test]
        public void LoaderMustPassFilenameToContainerLoaders()
        {
            var mockContainer = new Mock<IServiceContainer>();
            var mockLoader = new Mock<IContainerLoader>(MockBehavior.Loose);

            var loader = new Loader { Container = mockContainer.Object };
            loader.ContainerLoaders.Add(mockLoader.Object);

            // The container should call the load method
            // with the given filename
            var location = typeof(ConfigurationTests).Assembly.Location;
            string path = Path.GetDirectoryName(Path.GetFullPath(location));

            var emptyActions = new Action<IServiceContainer>[] { };
            mockLoader.Expect(l => l.CanLoad(It.Is<string>(f => File.Exists(f)))).Returns(true);
            mockLoader.Expect(l => l.Load(It.Is<string>(f => File.Exists(f)))).Returns(emptyActions);

            loader.LoadDirectory(path, "LinFu.UnitTests.dll");

            mockLoader.VerifyAll();
        }

        [Test]
        public void AssemblyLoaderMustLoadTargetAssemblyFromDisk()
        {
            // TODO: Mock out the IAssemblyLoader interface
            throw new NotImplementedException();
        }
        [Test]
        public void LoaderMustLoadContainerLoadersMarkedWithContainerLoaderAttribute()
        {
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
