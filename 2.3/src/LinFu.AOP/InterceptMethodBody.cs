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
            var declaringType = method.DeclaringType;
            var module = declaringType.Module;

            var modifiableType = module.ImportType<IModifiableType>();
            _getInterceptionDisabled = module.ImportMethod<IModifiableType>("get_IsInterceptionDisabled");

            // Determine whether or not the method should be intercepted
            _interceptionDisabled = method.AddLocal<bool>();

            // var interceptionDisabled = this.IsInterceptionDisabled;
            IL.Emit(OpCodes.Ldarg_0);
            IL.Emit(OpCodes.Isinst, modifiableType);
            IL.Emit(OpCodes.Callvirt, _getInterceptionDisabled);
            IL.Emit(OpCodes.Stloc, _interceptionDisabled);
            
            // Construct the InvocationInfo instance
            _invocationInfo = method.AddLocal<IInvocationInfo>();
            var skipInvocationInfo = IL.Create(OpCodes.Nop);
            IL.Emit(OpCodes.Ldloc, _interceptionDisabled);
            IL.Emit(OpCodes.Brtrue, skipInvocationInfo);

            var emitter = new InvocationInfoEmitter();
            emitter.Emit(method, method, _invocationInfo);            
            
            IL.Append(skipInvocationInfo);

            _surroundingImplementation = method.AddLocal<IAroundInvoke>();

            #region Prolog Code
            _skipProlog = IL.Create(OpCodes.Nop);

            AddProlog(method, IL);

            IL.Append(_skipProlog);

            #endregion

            var returnType = method.ReturnType.ReturnType;
            _returnValue = method.AddLocal<object>();

            // TODO: Add the method body replacement/interception code here
            AddOriginalInstructions(IL, oldInstructions);

            // Save the return type
            var voidType = module.ImportType(typeof(void));
            var returnTypeIsValueType = returnType != voidType && returnType.IsValueType;

            if (returnType is GenericParameter || returnTypeIsValueType)
                IL.Create(OpCodes.Box, returnType);           

            if (returnType != voidType)
                IL.Create(OpCodes.Stloc, _returnValue);

            #region Epilog Code
            AddEpilog(IL, module, returnType);

            #endregion

            var lastInstruction = oldInstructions.LastOrDefault();
            var opCode = lastInstruction.OpCode;
            var flowControl = opCode.FlowControl;

            IL.Emit(OpCodes.Ret);

            throw new NotImplementedException();
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
            var aroundInvoke = module.ImportMethod<IAroundInvoke>("AfterInvoke");
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
            var declaringType = method.DeclaringType;
            var module = declaringType.Module;
            var modifiableType = module.ImportType<IModifiableType>();

            IL.Emit(OpCodes.Ldarg_0);
            IL.Emit(OpCodes.Isinst, modifiableType);
            IL.Emit(OpCodes.Brfalse, _skipProlog);

            IL.Emit(OpCodes.Ldloc, _interceptionDisabled);
            IL.Emit(OpCodes.Brtrue, _skipProlog);

            // var provider = this.MethodReplacementProvider;
            var methodReplacementProviderType = module.ImportType<IMethodReplacementProvider>();
            _methodReplacementProvider = method.AddLocal<IMethodReplacementProvider>();
            var getMethodReplacement = module.ImportMethod<IMethodReplacementProvider>("get_MethodReplacementProvider");

            IL.Emit(OpCodes.Ldarg_0);
            IL.Emit(OpCodes.Isinst, methodReplacementProviderType);
            IL.Emit(OpCodes.Callvirt, getMethodReplacement);
            IL.Emit(OpCodes.Stloc, _methodReplacementProvider);

            // var aroundInvokeProvider = this.AroundInvokeProvider;
            _aroundInvokeProvider = method.AddLocal<IAroundInvokeProvider>();
            var getAroundInvokeProvider = module.ImportMethod<IMethodReplacementProvider>("get_AroundInvokeProvider");

            IL.Emit(OpCodes.Ldarg_0);
            IL.Emit(OpCodes.Isinst, methodReplacementProviderType);
            IL.Emit(OpCodes.Callvirt, getAroundInvokeProvider);
            IL.Emit(OpCodes.Stloc, _aroundInvokeProvider);

            // if (aroundInvokeProvider != null ) {
            var skipGetSurroundingImplementation = IL.Create(OpCodes.Nop);
            IL.Emit(OpCodes.Ldloc, _aroundInvokeProvider);
            IL.Emit(OpCodes.Brfalse, skipGetSurroundingImplementation);

            // var surroundingImplementation = this.GetSurroundingImplementation(this, invocationInfo);
            var getSurroundingImplementation = module.ImportMethod<IMethodReplacementProvider>("GetSurroundingImplementation");
            IL.Emit(OpCodes.Ldloc, _aroundInvokeProvider);
            IL.Emit(OpCodes.Ldloc, _invocationInfo);            
            IL.Emit(OpCodes.Callvirt, getSurroundingImplementation);
            IL.Emit(OpCodes.Stloc, _surroundingImplementation);

            // }

            IL.Append(skipGetSurroundingImplementation);

            var skipBeforeInvoke = IL.Create(OpCodes.Nop);

            // if (surroundingImplementation != null) {
            IL.Emit(OpCodes.Ldloc, _surroundingImplementation);
            IL.Emit(OpCodes.Brfalse, skipBeforeInvoke);

            var beforeInvoke = module.ImportMethod<IBeforeInvoke>("BeforeInvoke");

            // surroundingImplementation.BeforeInvoke(invocationInfo);
            IL.Emit(OpCodes.Ldloc, _surroundingImplementation);
            IL.Emit(OpCodes.Ldloc, _invocationInfo);
            IL.Emit(OpCodes.Callvirt, beforeInvoke);
            // }

            IL.Append(skipBeforeInvoke);

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
