﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.CecilExtensions;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Reflection;
using LinFu.AOP.Interfaces;
using System.Diagnostics;
using Simple.IoC;
using Simple.IoC.Loaders;
using System.IO;

namespace LinFu.AOP.Weavers.Cecil
{
    public class ConstructorCrossCutter : IWeaver<TypeDefinition>
    {
        private MethodReference _initInstance;
        private MethodReference _initType;
        private MethodReference _getTypeFromHandle;
        private IContainer  _container;
        public ConstructorCrossCutter()
        {
            _container = new SimpleContainer();
            var assemblyLocation = typeof(ConstructorCrossCutter).Assembly.Location;
            var currentLocation = Path.GetFullPath(Path.GetDirectoryName(assemblyLocation));
            
            var loader = new Loader(_container);
            loader.LoadDirectory(currentLocation, "*.dll");
        }
        public virtual bool ShouldWeave(TypeDefinition item)
        {           
            var defaultResult = item.IsClass && item.IsPublic;

            // Use the type filter if it exists
            ITypeFilter typeFilter = null;
            if (_container.Contains(typeof(ITypeFilter)))
                typeFilter = _container.GetService<ITypeFilter>();

            if (typeFilter == null)
                return defaultResult;

            var result = typeFilter.ShouldWeave(item);

            return result;
        }
        public void ImportReferences(ModuleDefinition module)
        {
            MethodBase initInstanceMethod = typeof(InstanceRegistry).GetMethod("InitializeInstance",
                BindingFlags.Static | BindingFlags.Public);

            MethodBase initTypeMethod = typeof(InstanceRegistry)
                .GetMethod("InitializeType", BindingFlags.Static | BindingFlags.Public);

            Debug.Assert(initInstanceMethod != null);
            Debug.Assert(initTypeMethod != null);

            _initInstance = module.Import(initInstanceMethod);

            _initType = module.Import(initTypeMethod);
            _getTypeFromHandle =
                module.Import(typeof(Type).GetMethod("GetTypeFromHandle",
                BindingFlags.Static | BindingFlags.Public));

        }
        public void Weave(TypeDefinition item)
        {
            List<MethodDefinition> constructors = new List<MethodDefinition>();
            foreach (MethodDefinition ctor in item.Constructors)
            {
                constructors.Add(ctor);
            }

            // Only instance constructors can be modified
            var targetList = constructors.Where(c => !c.IsStatic).ToList();

            // Modify all constructors regardless
            // of whether or not they are public
            targetList.ForEach(Weave);
        }
        private void Weave(MethodDefinition ctor)
        {
            // The target method must have a body
            if (ctor.Body == null)
                return;

            // Skip constructors that are handled by the runtime
            if (ctor.IsRuntime && ctor.IsManaged)
                return;

            List<Instruction> originalInstructions = new List<Instruction>();
            Mono.Cecil.Cil.MethodBody methodBody = ctor.Body;
            foreach (Instruction instruction in methodBody.Instructions)
            {
                originalInstructions.Add(instruction);
            }

            // HACK: Skip methods that don't have any instructions
            if (originalInstructions.Count == 0)
                return;

            var lastInstruction = originalInstructions.LastOrDefault();
            if (lastInstruction != null && lastInstruction.OpCode == OpCodes.Ret)
            {
                // HACK: Convert the Ret instruction into a Nop
                // instruction so that the code will
                // fall through to the initializer
                lastInstruction.OpCode = OpCodes.Nop;

                foreach (var instruction in originalInstructions)
                {
                    if (instruction.OpCode != OpCodes.Ret)
                        continue;

                    // HACK: Modify all ret instructions to branch
                    // to the last ret instruction instead of exiting
                    // the method
                    instruction.OpCode = OpCodes.Br;
                    instruction.Operand = lastInstruction;
                }
            }
            

            methodBody.Instructions.Clear();
            CilWorker IL = methodBody.CilWorker;
            AppendInstructions(IL, originalInstructions);

            TypeReference declaringType = ctor.DeclaringType;
            // Instantiate the generic type before determining
            // the current method
            if (declaringType.GenericParameters.Count > 0)
            {
                GenericInstanceType genericType = new GenericInstanceType(declaringType);
                foreach (GenericParameter parameter in declaringType.GenericParameters)
                {
                    genericType.GenericArguments.Add(parameter);
                }

                declaringType = genericType;
            }

            if (ctor.IsStatic)
            {
                // Call the InitializeType() method
                IL.Emit(OpCodes.Ldtoken, declaringType);
                IL.Emit(OpCodes.Call, _getTypeFromHandle);
                IL.Emit(OpCodes.Call, _initType);
                IL.Emit(OpCodes.Ret);
                return;
            }
            IL.Emit(OpCodes.Ldarg_0);
            IL.Emit(OpCodes.Call, _initInstance);
            IL.Emit(OpCodes.Ret);
        }
        private static void AppendInstructions(CilWorker IL, IEnumerable<Instruction> queue)
        {
            foreach (Instruction current in queue)
            {
                IL.Append(current);
            }
        }
    }
}