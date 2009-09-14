using System;
using System.Linq;
using LinFu.AOP.Cecil;
using LinFu.AOP.Cecil.Loaders;
using LinFu.AOP.Interfaces;
using LinFu.IoC.Reflection;
using LinFu.Reflection;
using LinFu.Reflection.Emit;
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
        public void ShouldImplementIModifiableTypeOnModifiedSampleClass()
        {
            var assembly = AssemblyFactory.GetAssembly("SampleLibrary.dll");
            var module = assembly.MainModule;
            const string typeName = "SampleClassWithNonVirtualMethod";

            var targetType = (from TypeDefinition t in module.Types
                              where t.Name == typeName
                              select t).First();

            Assert.IsNotNull(targetType);

            Func<MethodDefinition, bool> methodFilter = method => method.Name == "DoSomething";
            targetType.InterceptMethodBodies(methodFilter);

            var modifiedAssembly = assembly.ToAssembly();
            var modifiedTargetType = (from t in modifiedAssembly.GetTypes()
                                      where t.Name == typeName
                                      select t).First();

            var instance = Activator.CreateInstance(modifiedTargetType);
            Assert.IsNotNull(instance);
            Assert.IsTrue(instance is IModifiableType);
        }

        [Test]
        public void ShouldNotCallAroundInvokeIfInterceptionIsDisabled()
        {
            const string targetClassName = "SampleClassWithNonVirtualMethod";
            var assembly = AssemblyFactory.GetAssembly("SampleLibrary.dll");

            var module = assembly.MainModule;
            var targetTypeDefinition = (from TypeDefinition t in module.Types
                                        where t.Name == targetClassName
                                        select t).First();

            targetTypeDefinition.InterceptMethodBodies(m => m.Name == "DoSomething");

            var modifiedAssembly = assembly.ToAssembly();
            var targetType = (from t in modifiedAssembly.GetTypes()
                              where t.Name == targetClassName
                              select t).First();

            Assert.IsNotNull(targetType);

            // The modified type should never call the provider
            var provider = new Mock<IAroundInvokeProvider>();
            var targetInstance = Activator.CreateInstance(targetType);

            Assert.IsNotNull(targetInstance);

            var modifiedType = targetInstance as IModifiableType;
            Assert.IsNotNull(modifiedType);
            modifiedType.AroundInvokeProvider = provider.Object;

            // Disable interception
            modifiedType.IsInterceptionDisabled = true;

            var targetMethod = targetType.GetMethod("DoSomething");
            Assert.IsNotNull(targetMethod);

            targetMethod.Invoke(targetInstance, new object[0]);
            provider.VerifyAll();
        }

        [Test]
        public void ShouldCallAroundInvoke()
        {
            const string targetClassName = "SampleClassWithNonVirtualMethod";
            var assembly = AssemblyFactory.GetAssembly("SampleLibrary.dll");

            var module = assembly.MainModule;
            var targetTypeDefinition = (from TypeDefinition t in module.Types
                                        where t.Name == targetClassName
                                        select t).First();

            targetTypeDefinition.InterceptMethodBodies(m => m.Name == "DoSomething");

            var modifiedAssembly = assembly.ToAssembly();
            var targetType = (from t in modifiedAssembly.GetTypes()
                              where t.Name == targetClassName
                              select t).First();

            Assert.IsNotNull(targetType);

            // The modified type should call both the BeforeInvoke and AfterInvoke methods
            var aroundInvoke = new Mock<IAroundInvoke>();
            aroundInvoke.Expect(around => around.BeforeInvoke(It.IsAny<IInvocationInfo>()));
            aroundInvoke.Expect(around => around.AfterInvoke(It.IsAny<IInvocationInfo>(), It.IsAny<object>()));

            var provider = new Mock<IAroundInvokeProvider>();
            provider.Expect(p => p.GetSurroundingImplementation(It.IsAny<IInvocationInfo>())).Returns(aroundInvoke.Object);

            var targetInstance = Activator.CreateInstance(targetType);
            Assert.IsNotNull(targetInstance);

            var modifiedType = targetInstance as IModifiableType;
            Assert.IsNotNull(modifiedType);
            modifiedType.AroundInvokeProvider = provider.Object;

            var targetMethod = targetType.GetMethod("DoSomething");
            Assert.IsNotNull(targetMethod);

            targetMethod.Invoke(targetInstance, new object[0]);

            provider.VerifyAll();
            aroundInvoke.VerifyAll();
        }

        [Test]
        public void ShouldCallAroundInvokeProvider()
        {
            const string targetClassName = "SampleClassWithNonVirtualMethod";

            var provider = new SampleAroundInvokeProvider();

            var assemblyLocation = typeof(SampleClassWithNonVirtualMethod).Assembly.Location;

            // Rewrite the target assembly
            var targetAssembly = AssemblyFactory.GetAssembly(assemblyLocation);

            var module = targetAssembly.MainModule;
            var targetTypeDefinition = (from TypeDefinition t in module.Types
                                        where t.Name == targetClassName
                                        select t).First();

            targetTypeDefinition.InterceptMethodBodies(m => m.Name == "DoSomething");

            var modifiedAssembly = targetAssembly.ToAssembly();

            // Get the target type
            var targetType = (from t in modifiedAssembly.GetTypes()
                              where t.Name == targetClassName
                              select t).First();

            Assert.IsNotNull(targetType);

            // Instantiate the type
            var targetInstance = Activator.CreateInstance(targetType);
            Assert.IsNotNull(targetInstance);
            Assert.IsTrue(targetInstance is IModifiableType);


            // Assign the provider instance to the modified type
            var modifiableType = targetInstance as IModifiableType;
            Assert.IsNotNull(modifiableType);

            modifiableType.AroundInvokeProvider = provider;

            var targetMethod = targetType.GetMethod("DoSomething");
            Assert.IsNotNull(targetMethod);

            targetMethod.Invoke(targetInstance, new object[0]);

            Assert.IsTrue(provider.GetSurroundingImplementationWasCalled);
        }

        [Test]
        public void ShouldCallInterceptorIfInterceptionIsEnabled()
        {
            const string targetClassName = "SampleClassWithNonVirtualMethod";
            var assembly = AssemblyFactory.GetAssembly("SampleLibrary.dll");

            var module = assembly.MainModule;
            var targetTypeDefinition = (from TypeDefinition t in module.Types
                                        where t.Name == targetClassName
                                        select t).First();

            targetTypeDefinition.InterceptMethodBodies(m => m.Name == "DoSomething");

            var modifiedAssembly = assembly.ToAssembly();
            var targetType = (from t in modifiedAssembly.GetTypes()
                              where t.Name == targetClassName
                              select t).First();

            Assert.IsNotNull(targetType);

            var targetInstance = Activator.CreateInstance(targetType);

            // The modified type should call the interceptor that comes out of the
            // provider
            var interceptor = new SampleMethodReplacement();
            var provider = new Mock<IMethodReplacementProvider>();
            provider.Expect(p => p.CanReplace(targetInstance, It.IsAny<IInvocationInfo>())).Returns(true);
            provider.Expect(p => p.GetMethodReplacement(targetInstance, It.IsAny<IInvocationInfo>())).Returns(interceptor);

            var modified = targetInstance as IModifiableType;
            Assert.IsNotNull(modified);

            modified.MethodReplacementProvider = provider.Object;

            var targetMethod = targetType.GetMethod("DoSomething");
            Assert.IsNotNull(targetMethod);

            targetMethod.Invoke(targetInstance, new object[0]);

            Assert.IsTrue(interceptor.HasBeenCalled);
            provider.VerifyAll();
        }

        [Test]
        public void ShouldNotCallInterceptorIfInterceptionIsDisabled()
        {
            const string targetClassName = "SampleClassWithNonVirtualMethod";
            var assembly = AssemblyFactory.GetAssembly("SampleLibrary.dll");

            var module = assembly.MainModule;
            var targetTypeDefinition = (from TypeDefinition t in module.Types
                                        where t.Name == targetClassName
                                        select t).First();

            targetTypeDefinition.InterceptMethodBodies(m => m.Name == "DoSomething");

            var modifiedAssembly = assembly.ToAssembly();
            var targetType = (from t in modifiedAssembly.GetTypes()
                              where t.Name == targetClassName
                              select t).First();

            Assert.IsNotNull(targetType);

            var targetInstance = Activator.CreateInstance(targetType);

            // The modified type should never call the provider
            var provider = new Mock<IMethodReplacementProvider>();
            var modified = targetInstance as IModifiableType;

            Assert.IsNotNull(modified);

            // Disable interception
            modified.MethodReplacementProvider = provider.Object;
            modified.IsInterceptionDisabled = true;

            var targetMethod = targetType.GetMethod("DoSomething");
            Assert.IsNotNull(targetMethod);

            targetMethod.Invoke(targetInstance, new object[0]);
            provider.VerifyAll();
        }

        [Test]
        public void ShouldBeAbleToReplaceBodyOfMethodThatThrowsAnException()
        {
            const string targetClassName = "SampleClassWithNonVirtualMethodThatThrowsExceptions";
            var assembly = AssemblyFactory.GetAssembly("SampleLibrary.dll");

            var module = assembly.MainModule;
            var targetTypeDefinition = (from TypeDefinition t in module.Types
                                        where t.Name == targetClassName
                                        select t).First();

            targetTypeDefinition.InterceptMethodBodies(m => m.Name == "DoSomething");

            var modifiedAssembly = assembly.ToAssembly();
            var targetType = (from t in modifiedAssembly.GetTypes()
                              where t.Name == targetClassName
                              select t).First();

            Assert.IsNotNull(targetType);

            var targetInstance = Activator.CreateInstance(targetType);

            // The modified type should call the interceptor that comes out of the
            // provider
            var interceptor = new SampleMethodReplacement();
            var provider = new Mock<IMethodReplacementProvider>();
            provider.Expect(p => p.CanReplace(targetInstance, It.IsAny<IInvocationInfo>())).Returns(true);
            provider.Expect(p => p.GetMethodReplacement(targetInstance, It.IsAny<IInvocationInfo>())).Returns(interceptor);

            var modified = targetInstance as IModifiableType;
            Assert.IsNotNull(modified);

            modified.MethodReplacementProvider = provider.Object;

            var targetMethod = targetType.GetMethod("DoSomething");
            Assert.IsNotNull(targetMethod);

            // The target method should NOT throw an interception
            try
            {
                targetMethod.Invoke(targetInstance, new object[0]);
            }
            catch (NotImplementedException)
            {
                Assert.Fail();
                throw;
            }

            Assert.IsTrue(interceptor.HasBeenCalled);
            provider.VerifyAll();
        }

        [Test]
        public void ShouldBeAbleToChangeMethodReturnValueUsingInterceptor()
        {
            var interceptor = new Mock<IInterceptor>();
            interceptor.Expect(e => e.Intercept(It.IsAny<IInvocationInfo>())).Returns(42);

            var provider = new Mock<IMethodReplacementProvider>();
            provider.Expect(p => p.GetMethodReplacement(It.IsAny<object>(), It.IsAny<IInvocationInfo>())).Returns(interceptor.Object);
            provider.Expect(p => p.CanReplace(It.IsAny<object>(), It.IsAny<IInvocationInfo>())).Returns(true);

            var assembly = AssemblyFactory.GetAssembly("SampleLibrary.dll");
            var module = assembly.MainModule;

            var targetTypeDef = (from TypeDefinition t in module.Types
                                 where t.Name == "SampleClassWithNonVirtualMethodWithReturnValue"
                                 select t).First();

            targetTypeDef.InterceptMethodBodies(m => m.Name == "DoSomething");

            var modifiedAssembly = assembly.ToAssembly();
            var targetType = modifiedAssembly.GetTypes()
                .Where(t => t.Name == "SampleClassWithNonVirtualMethodWithReturnValue")
                .First();

            var targetInstance = Activator.CreateInstance(targetType);
            Assert.IsNotNull(targetInstance);

            var modified = targetInstance as IModifiableType;
            Assert.IsNotNull(modified);

            modified.MethodReplacementProvider = provider.Object;

            var targetMethod = targetType.GetMethod("DoSomething");
            Assert.IsNotNull(targetMethod);

            object result = targetMethod.Invoke(targetInstance, new object[]{1, 1});

            Assert.AreEqual(42, result);
            provider.VerifyAll();
        }
    }
}
