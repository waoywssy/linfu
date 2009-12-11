using System;
using System.IO;
using System.Linq;
using LinFu.AOP.Cecil;
using LinFu.AOP.Interfaces;
using LinFu.IoC.Reflection;
using LinFu.Reflection.Emit;
using LinFu.UnitTests.Reflection;
using Mono.Cecil;
using Moq;
using NUnit.Framework;
using SampleLibrary.AOP;

namespace LinFu.UnitTests.AOP
{
    [TestFixture]
    public class MethodBodyInterceptionTests 
    {
        [Test]
        public void ShouldInvokeAroundInvokeProviderIfInterceptionIsEnabled()
        {
            var aroundInvoke = new SampleAroundInvoke();
            var provider = new SampleAroundInvokeProvider(aroundInvoke);
            Action<object> condition = (instance) =>
            {
                Assert.IsNotNull(instance);

                var modifiedInstance = (IModifiableType) instance;
                modifiedInstance.AroundInvokeProvider = provider;

                instance.Invoke("DoSomething");

                Assert.IsTrue(aroundInvoke.BeforeInvokeWasCalled);
                Assert.IsTrue(aroundInvoke.AfterInvokeWasCalled);
            };

            Test(condition);
        }
        [Test]
        public void ShouldNotInvokeAroundInvokeProviderIfInterceptionIsDisabled()
        {
            var aroundInvoke = new SampleAroundInvoke();
            var provider = new SampleAroundInvokeProvider(aroundInvoke);
            Action<object> condition = (instance) =>
            {
                Assert.IsNotNull(instance);

                var modifiedInstance = (IModifiableType)instance;
                modifiedInstance.AroundInvokeProvider = provider;
                modifiedInstance.IsInterceptionDisabled = true;

                instance.Invoke("DoSomething");

                Assert.IsFalse(aroundInvoke.BeforeInvokeWasCalled);
                Assert.IsFalse(aroundInvoke.AfterInvokeWasCalled);
            };

            Test(condition);
        }

        [Test]
        public void ShouldImplementIModifiableTypeOnModifiedSampleClass()
        {
            Action<object> condition = (instance) =>
                           {
                               Assert.IsNotNull(instance);
                               Assert.IsTrue(instance is IModifiableType);
                           };

            Test(condition);
        }

        [Test]
        public void ShouldInvokeMethodBodyReplacementIfInterceptionIsEnabled()
        {
            var sampleInterceptor = new SampleInterceptor();
            var sampleProvider = new SampleMethodReplacementProvider(sampleInterceptor);

            Action<object> condition = (instance) =>
                                           {
                                               Assert.IsNotNull(instance);
                                               Assert.IsTrue(instance is IModifiableType);

                                               var modifiableType = (IModifiableType) instance;
                                               modifiableType.MethodReplacementProvider = sampleProvider;
                                               modifiableType.IsInterceptionDisabled = false;

                                               instance.Invoke("DoSomething");
                                           };

            Test(condition);
            Assert.IsTrue(sampleInterceptor.WasInvoked);
        }

        [Test]
        public void ShouldNotInvokeMethodBodyReplacementIfInterceptionIsDisabled()
        {
            var sampleInterceptor = new SampleInterceptor();
            var sampleProvider = new SampleMethodReplacementProvider(sampleInterceptor);

            Action<object> condition = (instance) =>
            {
                Assert.IsNotNull(instance);
                Assert.IsTrue(instance is IModifiableType);

                var modifiableType = (IModifiableType)instance;
                modifiableType.MethodReplacementProvider = sampleProvider;
                modifiableType.IsInterceptionDisabled = true;

                instance.Invoke("DoSomething");
            };

            Test(condition);
            Assert.IsFalse(sampleInterceptor.WasInvoked);
        }

        #region Private Implementation
        private void Test(Action<object> testInstance)
        {
            var libraryFileName = "SampleLibrary.dll";
            var typeName = "SampleClassWithNonVirtualMethod";
            Func<MethodReference, bool> methodFilter = m => m.Name == "DoSomething";


            Test(libraryFileName, typeName, methodFilter, type => Test(type, testInstance));
        }
        private void Test(string libraryFileName, string typeName, Func<MethodReference, bool> methodFilter, Action<Type> testTargetType)
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
        private void ModifyType(TypeDefinition targetType, Func<MethodReference, bool> methodFilter)
        {
            targetType.InterceptMethodBody(methodFilter);
        }
        #endregion
    }
}
