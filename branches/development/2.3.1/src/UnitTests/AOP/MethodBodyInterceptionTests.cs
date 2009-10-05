using System;
using System.Linq;
using LinFu.AOP.Cecil;
using LinFu.AOP.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil;
using NUnit.Framework;

namespace LinFu.UnitTests.AOP
{
    [TestFixture]
    public class MethodBodyInterceptionTests
    {
        [Test]
        public void ShouldImplementIModifiableTypeOnModifiedSampleClass()
        {
            var assembly = AssemblyFactory.GetAssembly("SampleLibrary.dll");
            var module = assembly.MainModule;
            var typeName = "SampleClassWithNonVirtualMethod";

            var targetType = (from TypeDefinition t in module.Types
                              where t.Name == typeName
                              select t).First();

            Assert.IsNotNull(targetType);

            Func<MethodDefinition, bool> methodFilter = method => method.Name == "DoSomething";
            targetType.InterceptMethodBody(methodFilter);

            var modifiedAssembly = assembly.ToAssembly();
            var modifiedTargetType = (from t in modifiedAssembly.GetTypes()
                                      where t.Name == typeName
                                      select t).First();

            var instance = Activator.CreateInstance(modifiedTargetType);
            Assert.IsNotNull(instance);
            Assert.IsTrue(instance is IModifiableType);
        }
    }
}
