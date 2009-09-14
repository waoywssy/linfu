using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinFu.AOP.Cecil
{
    public class InterceptMethodBody : BaseMethodRewriter
    {
        private Func<MethodDefinition, bool> _methodFilter;

        private Instruction _skipProlog;
        private Instruction _skipEpilog;
        
        private VariableDefinition _interceptionDisabled;
        private VariableDefinition _invocationInfo;
        private VariableDefinition _methodReplacementProvider;
        private VariableDefinition _aroundInvokeProvider;
        private VariableDefinition _surroundingImplementation;
        private VariableDefinition _returnValue;
        private VariableDefinition _methodBodyReplacement;

        private MethodReference _getInterceptionDisabled;
        /// <summary>
        /// Initializes a new instance of the <see cref="InterceptMethodBody"/> class.
        /// </summary>
        /// <param name="methodFilter">The method filter that will determine the methods with the method bodies that will be intercepted.</param>
        public InterceptMethodBody(Func<MethodDefinition, bool> methodFilter)
        {
            _methodFilter = methodFilter;
        }

        /// <summary>
        /// Rewrites the instructions in the target method body.
        /// </summary>
        /// <param name="method">The target method.</param>
        /// <param name="IL">The <see cref="CilWorker"/> instance that represents the method body.</param>
        /// <param name="oldInstructions">The IL instructions of the original method body.</param>
        protected override void RewriteMethodBody(MethodDefinition method, CilWorker IL, IEnumerable<Instruction> oldInstructions)
        {
            if (!_methodFilter(method))
                return;

            var declaringType = method.DeclaringType;
            var module = declaringType.Module;

            var modifiableType = module.ImportType<IModifiableType>();
            _getInterceptionDisabled = module.ImportMethod<IModifiableType>("get_IsInterceptionDisabled");

            // Determine whether or not the method should be intercepted
            _interceptionDisabled = method.AddLocal<bool>();

            // var interceptionDisabled = this.IsInterceptionDisabled;
            GetIsInterceptionDisabled(IL, modifiableType);

            // Construct the InvocationInfo instance
            Instruction skipInvocationInfo = CreateInvocationInfo(method, IL);

            IL.Append(skipInvocationInfo);

            _surroundingImplementation = method.AddLocal<IAroundInvoke>();

            AddProlog(method, IL);

            var returnType = method.ReturnType.ReturnType;
            _returnValue = method.AddLocal<object>();

            GetMethodBodyReplacement(method, module, IL);

            // if (methodBodyReplacement != null) {
            var callOriginalInstructions = IL.Create(OpCodes.Nop);
            IL.Emit(OpCodes.Ldloc, _methodBodyReplacement);
            IL.Emit(OpCodes.Brfalse, callOriginalInstructions);

            var interceptMethod = module.ImportMethod<IInterceptor>("Intercept");

            IL.Emit(OpCodes.Ldloc, _methodBodyReplacement);
            IL.Emit(OpCodes.Ldloc, _invocationInfo);
            IL.Emit(OpCodes.Callvirt, interceptMethod);
            
            // Ignore the return value if the current method 
            // doesn't have a return type
            var voidType = module.ImportType(typeof(void));

            SaveReturnValue(module, IL, returnType, voidType);
                
            var packageReturnType = IL.Create(OpCodes.Nop);
            IL.Emit(OpCodes.Br, packageReturnType);

            // }
            IL.Append(callOriginalInstructions);
            // else {
            //      CallOriginalInstructions();
            AddOriginalInstructions(IL, oldInstructions);

            // }
            IL.Append(packageReturnType);

            // Save the return type
            var returnTypeIsValueType = returnType != voidType && returnType.IsValueType;

            if (returnType is GenericParameter || returnTypeIsValueType)
                IL.Create(OpCodes.Box, returnType);           

            if (returnType != voidType)
                IL.Create(OpCodes.Stloc, _returnValue);

            AddEpilog(IL, module, returnType);

            IL.Emit(OpCodes.Ret);
        }

        private void SaveReturnValue(ModuleDefinition module, CilWorker IL, TypeReference returnType, TypeReference voidType)
        {
            if (returnType == voidType)
            {
                IL.Emit(OpCodes.Pop);                
                return;
            }

            IL.PackageReturnValue(module, returnType);
            IL.Emit(OpCodes.Stloc, _returnValue);
            IL.Emit(OpCodes.Ldloc, _returnValue);
        }

        private void GetMethodBodyReplacement(MethodDefinition method, ModuleDefinition module, CilWorker IL)
        {
            _methodBodyReplacement = method.AddLocal<IInterceptor>();

            var skipGetMethodReplacement = IL.Create(OpCodes.Nop);

            IL.Emit(OpCodes.Ldloc, _methodReplacementProvider);
            IL.Emit(OpCodes.Brfalse, skipGetMethodReplacement);

            var canReplace = module.ImportMethod<IMethodReplacementProvider>("CanReplace");

            IL.Emit(OpCodes.Ldloc, _methodReplacementProvider);
            IL.Emit(OpCodes.Ldarg_0);
            IL.Emit(OpCodes.Ldloc, _invocationInfo);
            IL.Emit(OpCodes.Callvirt, canReplace);

            IL.Emit(OpCodes.Brfalse, skipGetMethodReplacement);

            var getMethodReplacement = module.ImportMethod<IMethodReplacementProvider>("GetMethodReplacement");
            IL.Emit(OpCodes.Ldloc, _methodReplacementProvider);
            IL.Emit(OpCodes.Ldarg_0);
            IL.Emit(OpCodes.Ldloc, _invocationInfo);
            IL.Emit(OpCodes.Callvirt, getMethodReplacement);
            IL.Emit(OpCodes.Stloc, _methodBodyReplacement);

            IL.Append(skipGetMethodReplacement);
        }

        private void GetIsInterceptionDisabled(CilWorker IL, TypeReference modifiableType)
        {
            IL.Emit(OpCodes.Ldarg_0);
            IL.Emit(OpCodes.Isinst, modifiableType);
            IL.Emit(OpCodes.Callvirt, _getInterceptionDisabled);
            IL.Emit(OpCodes.Stloc, _interceptionDisabled);
        }

        private Instruction CreateInvocationInfo(MethodDefinition method, CilWorker IL)
        {
            _invocationInfo = method.AddLocal<IInvocationInfo>();
            var skipInvocationInfo = IL.Create(OpCodes.Nop);
            IL.Emit(OpCodes.Ldloc, _interceptionDisabled);
            IL.Emit(OpCodes.Brtrue, skipInvocationInfo);

            var emitter = new InvocationInfoEmitter();
            emitter.Emit(method, method, _invocationInfo);
            return skipInvocationInfo;
        }

        private void AddEpilog(CilWorker IL, ModuleDefinition module, TypeReference returnType)
        {
            var voidType = module.ImportType(typeof(void));
            _skipEpilog = IL.Create(OpCodes.Nop);

            // if (!IsInterceptionDisabled && surroundingImplementation != null) {
            IL.Emit(OpCodes.Ldloc, _interceptionDisabled);
            IL.Emit(OpCodes.Brtrue, _skipEpilog);

            IL.Emit(OpCodes.Ldloc, _surroundingImplementation);
            IL.Emit(OpCodes.Brfalse, _skipEpilog);

            // surroundingImplementation.AfterInvoke(invocationInfo, returnValue);
            var aroundInvoke = module.ImportMethod<IAfterInvoke>("AfterInvoke");
            IL.Emit(OpCodes.Ldloc, _surroundingImplementation);
            IL.Emit(OpCodes.Ldloc, _invocationInfo);
            IL.Emit(OpCodes.Ldloc, _returnValue);
            IL.Emit(OpCodes.Callvirt, aroundInvoke);

            // }
            IL.Append(_skipEpilog);

            // Push the return value
            if (returnType != voidType)
                IL.Emit(OpCodes.Ldloc, _returnValue);
        }

        protected virtual void AddProlog(MethodDefinition method, CilWorker IL)
        {
            _skipProlog = IL.Create(OpCodes.Nop);

            var declaringType = method.DeclaringType;
            var module = declaringType.Module;
            var modifiableType = module.ImportType<IModifiableType>();
            
            IL.Emit(OpCodes.Ldarg_0);
            IL.Emit(OpCodes.Isinst, modifiableType);
            IL.Emit(OpCodes.Brfalse, _skipProlog);

            IL.Emit(OpCodes.Ldloc, _interceptionDisabled);
            IL.Emit(OpCodes.Brtrue, _skipProlog);

            // var provider = this.MethodReplacementProvider;
            GetMethodReplacementProvider(IL, method, module, modifiableType);

            // var aroundInvokeProvider = this.AroundInvokeProvider;
            GetAroundInvokeProvider(IL, method, module, modifiableType);

            // if (aroundInvokeProvider != null ) {
            GetSurroundingImplementation(IL, module);

            var skipBeforeInvoke = IL.Create(OpCodes.Nop);

            // if (surroundingImplementation != null) {
            IL.Emit(OpCodes.Ldloc, _surroundingImplementation);
            IL.Emit(OpCodes.Brfalse, skipBeforeInvoke);

            CallBeforeInvoke(IL, module);
            // }

            IL.Append(skipBeforeInvoke);

            IL.Append(_skipProlog);
        }

        private void GetMethodReplacementProvider(CilWorker IL, MethodDefinition method, ModuleDefinition module, TypeReference modifiableType)
        {
            _methodReplacementProvider = method.AddLocal<IMethodReplacementProvider>();
            var getMethodReplacement = module.ImportMethod<IMethodReplacementHost>("get_MethodReplacementProvider");

            IL.Emit(OpCodes.Ldarg_0);
            IL.Emit(OpCodes.Isinst, modifiableType);
            IL.Emit(OpCodes.Callvirt, getMethodReplacement);
            IL.Emit(OpCodes.Stloc, _methodReplacementProvider);
        }

        private void GetAroundInvokeProvider(CilWorker IL, MethodDefinition method, ModuleDefinition module, TypeReference modifiableType)
        {
            _aroundInvokeProvider = method.AddLocal<IAroundInvokeProvider>();
            var getAroundInvokeProvider = module.ImportMethod<IModifiableType>("get_AroundInvokeProvider");

            IL.Emit(OpCodes.Ldarg_0);
            IL.Emit(OpCodes.Isinst, modifiableType);
            IL.Emit(OpCodes.Callvirt, getAroundInvokeProvider);
            IL.Emit(OpCodes.Stloc, _aroundInvokeProvider);
        }

        private void CallBeforeInvoke(CilWorker IL, ModuleDefinition module)
        {
            var beforeInvoke = module.ImportMethod<IBeforeInvoke>("BeforeInvoke");

            // surroundingImplementation.BeforeInvoke(invocationInfo);
            IL.Emit(OpCodes.Ldloc, _surroundingImplementation);
            IL.Emit(OpCodes.Ldloc, _invocationInfo);
            IL.Emit(OpCodes.Callvirt, beforeInvoke);
        }

        private void GetSurroundingImplementation(CilWorker IL, ModuleDefinition module)
        {
            var skipGetSurroundingImplementation = IL.Create(OpCodes.Nop);
            IL.Emit(OpCodes.Ldloc, _aroundInvokeProvider);
            IL.Emit(OpCodes.Brfalse, skipGetSurroundingImplementation);

            // var surroundingImplementation = this.GetSurroundingImplementation(this, invocationInfo);
            var getSurroundingImplementation = module.ImportMethod<IAroundInvokeProvider>("GetSurroundingImplementation");
            IL.Emit(OpCodes.Ldloc, _aroundInvokeProvider);
            IL.Emit(OpCodes.Ldloc, _invocationInfo);            
            IL.Emit(OpCodes.Callvirt, getSurroundingImplementation);
            IL.Emit(OpCodes.Stloc, _surroundingImplementation);

            // }

            IL.Append(skipGetSurroundingImplementation);
        }

        protected virtual void AddOriginalInstructions(CilWorker IL, IEnumerable<Instruction> oldInstructions)
        {
            var originalInstructions = new List<Instruction>(oldInstructions);
            var lastInstruction = originalInstructions.LastOrDefault();

            if (lastInstruction != null && lastInstruction.OpCode == OpCodes.Ret)
            {
                // HACK: Convert the Ret instruction into a Nop
                // instruction so that the code will
                // fall through to the epilog
                lastInstruction.OpCode = OpCodes.Nop;
            }

            foreach (var instruction in originalInstructions)
            {
                if (instruction.OpCode != OpCodes.Ret || instruction == lastInstruction)
                    continue;

                // HACK: Modify all ret instructions to call
                // the epilog after execution
                instruction.OpCode = OpCodes.Br;
                instruction.Operand = lastInstruction;
            }

            // Emit the original instructions
            foreach (var instruction in originalInstructions)
            {
                IL.Append(instruction);
            }
        }
    }
}
