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
            var _interceptionDisabled = method.AddLocal<bool>();

            // var interceptionDisabled = this.IsInterceptionDisabled;
            IL.Emit(OpCodes.Ldarg_0);
            IL.Emit(OpCodes.Isinst, modifiableType);
            IL.Emit(OpCodes.Callvirt, _getInterceptionDisabled);
            IL.Emit(OpCodes.Stloc, _interceptionDisabled);

            var invocationInfo = method.AddLocal<IInvocationInfo>();
            var skipInvocationInfo = IL.Create(OpCodes.Nop);
            IL.Emit(OpCodes.Ldloc, _interceptionDisabled);
            IL.Emit(OpCodes.Brtrue, skipInvocationInfo);

            var emitter = new InvocationInfoEmitter();
            emitter.Emit(method, method, invocationInfo);

            IL.Append(skipInvocationInfo);

            #region Prolog Code
            _skipProlog = IL.Create(OpCodes.Nop);

            AddProlog(method, IL, _interceptionDisabled);

            IL.Append(_skipProlog);

            #endregion

            AddOriginalInstructions(IL, oldInstructions);

            #region Epilog Code
            _skipEpilog = IL.Create(OpCodes.Nop);
            IL.Emit(OpCodes.Ldloc, _interceptionDisabled);
            IL.Emit(OpCodes.Brtrue, _skipEpilog);

            // TODO: Add the AfterInvoke code here
            IL.Append(_skipEpilog);

            var lastInstruction = oldInstructions.LastOrDefault();
            var opCode = lastInstruction.OpCode;
            var flowControl = opCode.FlowControl;

            IL.Emit(OpCodes.Ret);

            #endregion

            throw new NotImplementedException();
        }

        protected virtual void AddProlog(MethodDefinition method, CilWorker IL, VariableDefinition interceptionDisabled)
        {
            var declaringType = method.DeclaringType;
            var module = declaringType.Module;
            var modifiableType = module.ImportType<IModifiableType>();

            IL.Emit(OpCodes.Ldarg_0);
            IL.Emit(OpCodes.Isinst, modifiableType);
            IL.Emit(OpCodes.Brfalse, _skipProlog);

            IL.Emit(OpCodes.Ldloc, interceptionDisabled);
            IL.Emit(OpCodes.Brtrue, _skipProlog);

            // TODO: Add the prolog code here
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
