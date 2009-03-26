using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Cecil.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Reflection;

namespace LinFu.AOP.Cecil
{
    internal class InterceptNewCalls : IMethodRewriter
    {
        #region Private Fields
        private MethodReference _getCurrentMethod;
        #endregion

        private INewObjectWeaver _emitter;
        public InterceptNewCalls(INewObjectWeaver emitter)
        {
            _emitter = emitter;
        }

        public IEnumerable<Instruction> GetNewInstructions(MethodDefinition method, CilWorker IL, IEnumerable<Instruction> oldInstructions)
        {
            var newInstructions = new Queue<Instruction>();

            _emitter.AddLocals(method);
            #region Copy the old instructions
            foreach (var instruction in oldInstructions)
            {
                // Intercept only the new operator
                if (instruction.OpCode != OpCodes.Newobj)
                {
                    newInstructions.Enqueue(instruction);
                    continue;
                }

                EmitNewObject(method, newInstructions, instruction);
            }

            return newInstructions;
            #endregion
        }

        public void AddAdditionalMembers(TypeDefinition host)
        {
            _emitter.AddAdditionalMembers(host);
        }

        public void ImportReferences(ModuleDefinition module)
        {
            _getCurrentMethod = module.ImportMethod<MethodBase>("GetCurrentMethod", BindingFlags.Public | BindingFlags.Static);
            _emitter.ImportReferences(module);
        }

        private void EmitNewObject(MethodDefinition method, Queue<Instruction> newInstructions, Instruction currentInstruction)
        {
            CilWorker IL = method.Body.CilWorker;

            MethodReference constructor = (MethodReference)currentInstruction.Operand;
            TypeReference concreteType = constructor.DeclaringType;
            var parameters = constructor.Parameters;

            if (!_emitter.ShouldIntercept(constructor, concreteType, method))
            {
                // Reuse the old instruction instead of emitting a new one
                newInstructions.Enqueue(currentInstruction);
                return;
            }

            _emitter.EmitNewObject(method, newInstructions, constructor, concreteType);
        }
    }    
}
