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
            Action<object> testInstance = (instance) =>
                           {
                               Assert.IsNotNull(instance);
                               Assert.IsTrue(instance is IModifiableType);
                           };

            Test(testInstance);
        }

        private void Test(Action<object> testInstance)
        {
            var libraryFileName = "SampleLibrary.dll";
            var typeName = "SampleClassWithNonVirtualMethod";
            Func<MethodDefinition, bool> methodFilter = m => m.Name == "DoSomething";


            Test(libraryFileName, typeName, methodFilter, type => Test(type, testInstance));
        }
        private void Test(string libraryFileName, string typeName, Func<MethodDefinition, bool> methodFilter, Action<Type> testTargetType)
        {
            var assembly = AssemblyFactory.GetAssembly(libraryFileName);
            var module = assembly.MainModule;

            var targetType = (from TypeDefinition t in module.Types
                              where t.Name == typeName
                              select t).First();

            Assert.IsNotNull(targetType);

            ModifyType(targetType, methodFilter);
            Type modifiedTargetType = CreateModifiedType(assembly, typeName);

            testTargetType(modifiedTargetType);
        }

        private void Test(Type modifiedTargetType, Action<object> testInstance)
        {
            var instance = Activator.CreateInstance(modifiedTargetType);
            testInstance(instance);
        }

        private Type CreateModifiedType(AssemblyDefinition assembly, string typeName)
        {
            var modifiedAssembly = assembly.ToAssembly();
            return (from t in modifiedAssembly.GetTypes()
                    where t.Name == typeName
                    select t).First();
        }
        private void ModifyType(TypeDefinition targetType, Func<MethodDefinition, bool> methodFilter)
        {
            targetType.InterceptMethodBody(methodFilter);
        }
    }
}
