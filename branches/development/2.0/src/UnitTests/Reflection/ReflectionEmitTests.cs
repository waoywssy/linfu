using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using LinFu.IoC;
using LinFu.IoC.Configuration;
using LinFu.Reflection;
using LinFu.Reflection.Emit;
using LinFu.Reflection.Emit.Interfaces;
using Mono.Cecil;
using NUnit.Framework;

namespace LinFu.UnitTests.Reflection
{
    [TestFixture]
    public class ReflectionEmitTests : BasePEVerifyTestCase
    {
        private Loader loader;
        private ServiceContainer container;
        private string filename;
        protected override void OnInit()
        {
            if (File.Exists(filename))
                File.Delete(filename);

            // Initialize the loader
            // with all of the default LinFu plugins
            loader = new Loader();
            container = new ServiceContainer();
            filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output.dll");
            AutoDelete(filename);

            loader.LoadDirectory(AppDomain.CurrentDomain.BaseDirectory, "LinFu*.dll");
            loader.LoadInto(container);
        }
        protected override void OnTerm()
        {
            if (!File.Exists(filename))
                return;

            PEVerify(filename);
        }
        [Test]
        public void AssemblyFactoryMustEmitValidAssembly()
        {
            IAssemblyFactory factory = container.GetService<IAssemblyFactory>();
            Assert.IsTrue(container.Contains(typeof(IAssemblyFactory)));

            string assemblyName = "testAssembly";
            AssemblyDefinition assembly = factory.DefineAssembly(assemblyName, AssemblyKind.Dll);
            Assert.IsNotNull(assembly);

            // Save the assembly and verify the result
            AssemblyFactory.SaveAssembly(assembly, filename);
        }        

        [Test]
        public void AssemblyFactoryMustEmitBlankAssembly()
        {
            IAssemblyFactory factory = container.GetService<IAssemblyFactory>();
            Assert.IsTrue(container.Contains(typeof(IAssemblyFactory)));

            string assemblyName = "testAssembly";
            AssemblyDefinition assembly = factory.DefineAssembly(assemblyName, AssemblyKind.Dll);
            Assert.IsNotNull(assembly);

            Assert.IsTrue(assembly.MainModule.Types.Count == 0);
        }

        [Test]
        public void AssemblyDefinitionMustBeConvertibleToActualAssembly()
        {
            IAssemblyFactory factory = container.GetService<IAssemblyFactory>();
            Assert.IsNotNull(factory);

            Assert.IsTrue(container.Contains(typeof(IAssemblyFactory)));
            var definition = factory.DefineAssembly("testAssembly", AssemblyKind.Dll);

            Assembly assembly = definition.ToAssembly();
            Assert.IsTrue(assembly != null);
        }
    }
}
