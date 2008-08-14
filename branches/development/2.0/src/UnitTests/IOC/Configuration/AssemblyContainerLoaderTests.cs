using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC;
using LinFu.IoC.Configuration;
using LinFu.Reflection;
using Moq;
using NUnit.Framework;
using SampleLibrary;

namespace LinFu.UnitTests.IOC.Configuration
{
    [TestFixture]
    public class AssemblyContainerLoaderTests
    {
        private Mock<IAssemblyLoader> _mockAssemblyLoader;
        private Mock<ITypeExtractor> _mockTypeExtractor;
        private Mock<ITypeLoader> _mockTypeLoader;
        [SetUp]
        public void Init()
        {
            _mockAssemblyLoader = new Mock<IAssemblyLoader>();
            _mockTypeExtractor = new Mock<ITypeExtractor>();
            _mockTypeLoader = new Mock<ITypeLoader>();
        }
        [TearDown]
        public void Term()
        {
            _mockAssemblyLoader.VerifyAll();
            _mockTypeExtractor.VerifyAll();
            _mockTypeLoader.VerifyAll();

            _mockAssemblyLoader = null;
            _mockTypeExtractor = null;
            _mockTypeLoader = null;
        }

        [Test]
        public void AssemblyContainerLoaderShouldOnlyLoadDllFiles()
        {
            var mockTypeLoader = new Mock<ITypeLoader>();
            var containerLoader = new AssemblyContainerLoader();
            containerLoader.TypeLoaders.Add(mockTypeLoader.Object);

            // This should return true
            string validFile = typeof(AssemblyContainerLoaderTests).Assembly.Location;
            Assert.IsTrue(containerLoader.CanLoad(validFile));

            // This should return false;
            string invalidFile = "input.txt";
            Assert.IsFalse(containerLoader.CanLoad(invalidFile));
        }
        [Test]
        public void AssemblyContainerLoaderShouldCallAssemblyLoader()
        {
            var containerLoader = new AssemblyContainerLoader();

            // The container loader should use the assembly loader
            // to load the assembly
            string filename = "input.dll";
            
            _mockAssemblyLoader.Expect(loader => loader.Load(filename)).Returns(typeof(SampleClass).Assembly);

            containerLoader.AssemblyLoader = _mockAssemblyLoader.Object;
            containerLoader.Load(filename);
        }

        [Test]
        public void AssemblyContainerLoaderShouldCallTypeExtractor()
        {
            var containerLoader = new AssemblyContainerLoader();
            string filename = "input.dll";

            var targetAssembly = typeof (SampleClass).Assembly;
            
            // Make sure that it calls the assembly loader
            _mockAssemblyLoader.Expect(loader => loader.Load(filename)).Returns(targetAssembly);

            // It must call the Type Extractor
            _mockTypeExtractor.Expect(extractor => extractor.GetTypes(targetAssembly))
                .Returns(new Type[]{typeof(SampleClass)});

            containerLoader.AssemblyLoader = _mockAssemblyLoader.Object;
            containerLoader.TypeExtractor = _mockTypeExtractor.Object;
            containerLoader.Load(filename);            
        }

        [Test]
        public void AssemblyContainerLoaderShouldCallTypeLoader()
        {
            // HACK: The Cut&Paste is ugly, but it works
            var containerLoader = new AssemblyContainerLoader();
            string filename = "input.dll";

            var targetAssembly = typeof(SampleClass).Assembly;

            // Make sure that it calls the assembly loader
            _mockAssemblyLoader.Expect(loader => loader.Load(filename)).Returns(targetAssembly);

            // It must call the Type Extractor
            _mockTypeExtractor.Expect(extractor => extractor.GetTypes(targetAssembly))
                .Returns(new Type[] { typeof(SampleClass) });

            // Make sure that it calls the type loaders
            _mockTypeLoader.Expect(loader => loader.CanLoad(typeof (SampleClass))).Returns(true);
            _mockTypeLoader.Expect(loader => loader.Load(typeof (SampleClass)))
                .Returns(new Action<IServiceContainer>[0]);

            containerLoader.AssemblyLoader = _mockAssemblyLoader.Object;
            containerLoader.TypeExtractor = _mockTypeExtractor.Object;

            // The container loader should call the type loader
            // once the load method is called
            containerLoader.TypeLoaders.Add(_mockTypeLoader.Object);

            containerLoader.Load(filename);            
        }
    }
}
