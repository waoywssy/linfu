using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.DynamicProxy2.Interfaces;
using LinFu.IoC;
using LinFu.IoC.Configuration;
using LinFu.Reflection;
using LinFu.UnitTests.Tools;
using Mono.Cecil;
using NUnit.Framework;
using Moq;

namespace LinFu.UnitTests.Reflection
{
    [TestFixture]
    public class BuilderChainTests
    {
        private Loader loader;
        private ServiceContainer container;
        [SetUp]
        public void Init()
        {
            loader = new Loader();
            loader.LoadDirectory(AppDomain.CurrentDomain.BaseDirectory, "LinFu*.dll");
            container = new ServiceContainer();

            loader.LoadInto(container);
        }
        [TearDown]
        public void Term()
        {
            loader = null;
            container = null;
        }
        [Test]
        public void BuilderChainShouldExecuteWithGivenContext()
        {            
            var moqBuilder = new Mock<IBuilder<IProxyBuilderContext, AssemblyDefinition>>();
            var moqContext = new Mock<IProxyBuilderContext>();
            var context = moqContext.Object;
            var assemblyDefinition = AssemblyFactory.DefineAssembly("testAssembly", AssemblyKind.Dll);

            moqBuilder.Expect(builder => builder.Construct(context, assemblyDefinition));
            var chain = new List<IBuilder<IProxyBuilderContext, AssemblyDefinition>>();

            chain.ForEach(builder => builder.Construct(context, assemblyDefinition));
        }
    }
}
