﻿using System;
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
using SampleLibrary;

namespace LinFu.UnitTests.IOC.Configuration
{
    [TestFixture]
    public class ConfigurationTests
    {
        [Test]
        [Ignore("TODO: Implement this")]
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
            var mockListing = new Mock<IDirectoryListing>();

            var loader = new Loader
            {
                Container = mockContainer.Object,
                DirectoryLister = mockListing.Object
            };

            var filename = "input.dll";
            mockListing.Expect(listing => listing.GetFiles(It.IsAny<string>(), filename))
                .Returns(new []{filename});
            
            loader.ContainerLoaders.Add(mockLoader.Object);
            // The container should call the load method
            // with the given filename
            string path = string.Empty;

            var emptyActions = new Action<IServiceContainer>[] { };
            mockLoader.Expect(l => l.CanLoad(filename)).Returns(true);
            mockLoader.Expect(l => l.Load(filename)).Returns(emptyActions);

            loader.LoadDirectory(path, filename);

            mockLoader.VerifyAll();
            mockListing.VerifyAll();
        }

        [Test]
        public void AssemblyLoaderMustLoadTargetAssemblyFromDisk()
        {            
            IAssemblyLoader loader = new AssemblyLoader();

            // The loader should return a valid assembly
            var result = loader.Load(typeof (SampleClass).Assembly.Location);
            Assert.IsNotNull(result);
        }

        [Test]
        public void TypeExtractorMustListTypesFromGivenAssembly()
        {
            var targetAssembly = typeof (SampleClass).Assembly;
            
            ITypeExtractor extractor = new TypeExtractor();
            var results = extractor.GetTypes(targetAssembly);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.Count() > 0);
        }

        
        [Test]
        [Ignore("TODO: Implement this")]
        public void LoaderMustLoadContainerLoadersMarkedWithContainerLoaderAttribute()
        {
            throw new NotImplementedException();
        }
        [Test]
        [Ignore("TODO: Implement this")]
        public void LoaderMustLoadNamedFactoriesWithFactoryAttributeAnAssembly()
        {
            throw new NotImplementedException();
        }
        [Test]
        [Ignore("TODO: Implement this")]
        public void LoaderMustSignalToPluginsWhenTheLoadBegins()
        {
            throw new NotImplementedException();
        }

        [Test]
        [Ignore("TODO: Implement this")]
        public void LoaderMustSignalToPluginsWhenTheLoadEnds()
        {
            throw new NotImplementedException();
        }

        [Test]
        [Ignore("TODO: Implement this")]
        public void CreatedServicesMustBeAbleToInitializeThemselves()
        {
            throw new NotImplementedException();
        }


        [Test]
        [Ignore("TODO: Implement this")]
        public void FactoryMustBeCreatedFromTypeWithNamedImplementsAttribute()
        {
            throw new NotImplementedException();
        }

        [Test]
        [Ignore("TODO: Implement this")]
        public void NamedFactoryMustBeCreatedFromTypeWithFactoryAttribute()
        {
            throw new NotImplementedException();
        }
        
        [Test]
        [Ignore("TODO: Implement this")]
        public void BootstrapTypeLoaderMustLoadOtherTypeLoadersWithTypeLoaderAttribute()
        {
            throw new NotImplementedException();
        }
        [Test]
        [Ignore("TODO: Implement this")]
        public void UnnamedFactoryMustBeCreatedFromTypeWithFactoryAttribute()
        {
            throw new NotImplementedException();
        }
    }
}
