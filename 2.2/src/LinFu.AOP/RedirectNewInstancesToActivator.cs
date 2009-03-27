using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Cecil.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil;
using Mono.Cecil.Cil;
using LinFu.AOP.Interfaces;
using System.Reflection;

namespace LinFu.AOP.Cecil
{
    internal class RedirectNewInstancesToActivator : INewObjectWeaver
    {
        #region Private Fields
        private TypeReference _hostInterfaceType;
        private TypeReference _methodActivatorType;
        private TypeReference _objectType;
        private TypeReference _voidType;
        private TypeReference _objectListType;

        private MethodReference _getTypeFromHandle;
        private MethodReference _methodActivationContextCtor;
        private MethodReference _getActivator;
        private MethodReference _createInstance;
        private MethodReference _objectListCtor;
        private MethodReference _addMethod;
        private MethodReference _toArrayMethod;
        private MethodReference _reverseMethod;
        private MethodReference _getStaticActivator;
        private MethodReference _canActivate;
        private MethodReference _getItem;

        private VariableDefinition _constructorArguments;
        private VariableDefinition _currentArgument;
        private VariableDefinition _methodContext;
        private VariableDefinition _currentActivator;

        private Func<MethodReference, TypeReference, MethodReference, bool> _filter;

        #endregion


        public RedirectNewInstancesToActivator(Func<MethodReference, TypeReference, MethodReference, bool> filter)
        {
            _filter = filter;
        }

        public bool ShouldIntercept(MethodReference constructor, TypeReference concreteType, MethodReference hostMethod)
        {
            // Intercept all types by default
            if (_filter == null)
                return true;

            return _filter(constructor, concreteType, hostMethod);
        }

        public void AddAdditionalMembers(TypeDefinition host)
        {
            // Make sure the type implements IActivatorHost
            var interfaceWeaver = new ImplementActivatorHostWeaver();
            host.Accept(interfaceWeaver);
        }

        public void ImportReferences(ModuleDefinition module)
        {
            // Type imports
            _hostInterfaceType = module.ImportType<IActivatorHost>();
            _methodActivatorType = module.ImportType<IMethodActivator>();
            _objectType = module.ImportType<object>();
            _voidType = module.Import(typeof(void));
            _objectListType = module.ImportType<List<object>>();

            // Static method imports
            _getStaticActivator = module.ImportMethod("GetActivator", typeof(MethodActivatorRegistry), BindingFlags.Public | BindingFlags.Static);
            _getTypeFromHandle = module.ImportMethod<Type>("GetTypeFromHandle", BindingFlags.Public | BindingFlags.Static);

            // Constructor imports
            _methodActivationContextCtor = module.ImportConstructor<MethodActivationContext>(typeof(object), typeof(MethodBase), typeof(Type), typeof(object[]));

            // Instance method imports
            _getActivator = module.ImportMethod<IActivatorHost>("get_Activator");
            _objectListCtor = module.ImportConstructor<List<object>>(new Type[0]);
            _toArrayMethod = module.ImportMethod<List<object>>("ToArray", new Type[0]);
            _addMethod = module.ImportMethod<List<object>>("Add", new Type[] { typeof(object) });
            _reverseMethod = module.ImportMethod<List<object>>("Reverse", new Type[0]);
            _canActivate = module.ImportMethod<IMethodActivator>("CanActivate");
            _getItem = module.ImportMethod<List<object>>("get_Item", new Type[] { typeof(int) });

            var createInstanceMethod = typeof(IActivator<IMethodActivationContext>).GetMethod("CreateInstance");

            _createInstance = module.Import(createInstanceMethod);
        }

        public void EmitNewObject(MethodDefinition hostMethod, Queue<Instruction> newInstructions, MethodReference targetConstructor, TypeReference concreteType)
        {
            var IL = hostMethod.Body.CilWorker;
            var parameters = targetConstructor.Parameters;
            Instruction skipInterception = IL.Create(OpCodes.Nop);

            SaveConstructorArguments(newInstructions, IL, parameters);
            EmitCreateMethodActivationContext(hostMethod, newInstructions, concreteType);

            // Skip the interception if an activator cannot be found            
            EmitGetActivator(hostMethod, newInstructions, skipInterception);

            newInstructions.Enqueue(IL.Create(OpCodes.Stloc, _currentActivator));
            newInstructions.Enqueue(IL.Create(OpCodes.Ldloc, _currentActivator));
            newInstructions.Enqueue(IL.Create(OpCodes.Brfalse, skipInterception));

            // Determine if the activator can instantiate the method from the
            // current context
            newInstructions.Enqueue(IL.Create(OpCodes.Ldloc, _currentActivator));
            newInstructions.Enqueue(IL.Create(OpCodes.Ldloc, _methodContext));
            newInstructions.Enqueue(IL.Create(OpCodes.Callvirt, _canActivate));
            newInstructions.Enqueue(IL.Create(OpCodes.Brfalse, skipInterception));

            // Use the activator to create the object instance
            EmitCreateInstance(newInstructions, IL, concreteType);

            // }
            Instruction endCreate = IL.Create(OpCodes.Nop);
            newInstructions.Enqueue(IL.Create(OpCodes.Br, endCreate));
            // else {
            newInstructions.Enqueue(skipInterception);

            // Restore the arguments that were popped off the stack
            // by the list of constructor arguments
            var parameterCount = parameters.Count;
            for (var index = 0; index < parameterCount; index++)
            {
                var currentParameter = parameters[index];

                newInstructions.Enqueue(IL.Create(OpCodes.Ldloc, _constructorArguments));
                newInstructions.Enqueue(IL.Create(OpCodes.Ldc_I4, index));
                newInstructions.Enqueue(IL.Create(OpCodes.Callvirt, _getItem));
                newInstructions.Enqueue(IL.Create(OpCodes.Unbox_Any, currentParameter.ParameterType));
            }
            newInstructions.Enqueue(IL.Create(OpCodes.Newobj, targetConstructor));
            // }

            newInstructions.Enqueue(endCreate);
        }

        private void EmitCreateInstance(Queue<Instruction> newInstructions, CilWorker IL, TypeReference concreteType)
        {
            // T instance = this.Activator.CreateInstance(context);
            newInstructions.Enqueue(IL.Create(OpCodes.Ldloc, _currentActivator));
            newInstructions.Enqueue(IL.Create(OpCodes.Ldloc, _methodContext));
            newInstructions.Enqueue(IL.Create(OpCodes.Callvirt, _createInstance));
            newInstructions.Enqueue(IL.Create(OpCodes.Isinst, concreteType));
        }

        private void EmitCreateMethodActivationContext(MethodDefinition method, Queue<Instruction> newInstructions, TypeReference concreteType)
        {
            CilWorker IL = method.Body.CilWorker;

            // TODO: Add static method support
            var pushThis = method.IsStatic ? IL.Create(OpCodes.Ldnull) : IL.Create(OpCodes.Ldarg_0);

            // Push the 'this' pointer onto the stack
            newInstructions.Enqueue(pushThis);

            var module = method.DeclaringType.Module;

            // Push the current method onto the stack
            IL.PushMethod(method, module);

            // Push the concrete type onto the stack
            newInstructions.Enqueue(IL.Create(OpCodes.Ldtoken, concreteType));
            newInstructions.Enqueue(IL.Create(OpCodes.Call, _getTypeFromHandle));

            newInstructions.Enqueue(IL.Create(OpCodes.Ldloc, _constructorArguments));
            newInstructions.Enqueue(IL.Create(OpCodes.Callvirt, _toArrayMethod));
            newInstructions.Enqueue(IL.Create(OpCodes.Newobj, _methodActivationContextCtor));

            // var context = new MethodActivationContext(this, currentMethod, concreteType, args);
            newInstructions.Enqueue(IL.Create(OpCodes.Stloc, _methodContext));
        }

        private void SaveConstructorArguments(Queue<Instruction> newInstructions, CilWorker IL, ParameterDefinitionCollection parameters)
        {
            var parameterCount = parameters.Count;

            newInstructions.Enqueue(IL.Create(OpCodes.Newobj, _objectListCtor));
            newInstructions.Enqueue(IL.Create(OpCodes.Stloc, _constructorArguments));

            var index = parameterCount - 1;
            while (index >= 0)
            {
                var param = parameters[index];

                SaveConstructorArgument(newInstructions, IL, param);

                index--;
            }

            // Reverse the constructor arguments so that they appear in the correct order
            newInstructions.Enqueue(IL.Create(OpCodes.Ldloc, _constructorArguments));
            newInstructions.Enqueue(IL.Create(OpCodes.Callvirt, _reverseMethod));
        }

        private void SaveConstructorArgument(Queue<Instruction> newInstructions, CilWorker IL, ParameterDefinition param)
        {
            // Box the type if necessary
            var parameterType = param.ParameterType;
            if (parameterType.IsValueType || parameterType is GenericParameter)
                newInstructions.Enqueue(IL.Create(OpCodes.Box, parameterType));

            // Save the current argument
            newInstructions.Enqueue(IL.Create(OpCodes.Stloc, _currentArgument));

            // Add the item to the item to the collection
            newInstructions.Enqueue(IL.Create(OpCodes.Ldloc, _constructorArguments));
            newInstructions.Enqueue(IL.Create(OpCodes.Ldloc, _currentArgument));
            newInstructions.Enqueue(IL.Create(OpCodes.Callvirt, _addMethod));
        }

        private void EmitGetActivator(MethodDefinition method, Queue<Instruction> newInstructions, Instruction skipInterception)
        {
            CilWorker IL = method.Body.CilWorker;

            if (method.IsStatic)
            {
                // If this is a static method, get the static Activator for this
                // particular type
                newInstructions.Enqueue(IL.Create(OpCodes.Ldloc, _methodContext));
                newInstructions.Enqueue(IL.Create(OpCodes.Call, _getStaticActivator));

                return;
            }

            // Instance-specific code
            // if (this is IActivatorHost && this.Activator != null) {
            newInstructions.Enqueue(IL.Create(OpCodes.Ldarg_0));
            newInstructions.Enqueue(IL.Create(OpCodes.Isinst, _hostInterfaceType));
            newInstructions.Enqueue(IL.Create(OpCodes.Brfalse, skipInterception));

            // var activator = this.Activator;
            newInstructions.Enqueue(IL.Create(OpCodes.Ldarg_0));
            newInstructions.Enqueue(IL.Create(OpCodes.Isinst, _hostInterfaceType));
            newInstructions.Enqueue(IL.Create(OpCodes.Callvirt, _getActivator));
        }

        public void AddLocals(MethodDefinition hostMethod)
        {
            _constructorArguments = hostMethod.AddLocal<List<object>>();
            _currentArgument = hostMethod.AddLocal<object>();
            _methodContext = hostMethod.AddLocal<IMethodActivationContext>();
            _currentActivator = hostMethod.AddLocal<IMethodActivator>();
        }
    }
}
