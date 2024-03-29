﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Cecil.Interfaces;
using LinFu.AOP.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LinFu.AOP.Cecil
{
    public class SurroundMethodBody : ISurroundMethodBody
    {
        private VariableDefinition _methodReplacementProvider;
        private VariableDefinition _aroundInvokeProvider;
        private VariableDefinition _invocationInfo;
        private VariableDefinition _surroundingImplementation;
        private VariableDefinition _surroundingClassImplementation;
        private VariableDefinition _interceptionDisabled;
        private VariableDefinition _returnValue;
        private IInstructionEmitter _getMethodReplacementProvider;
        private readonly Type _registryType;

        public SurroundMethodBody(IMethodBodyRewriterParameters parameters)
        {
            _methodReplacementProvider = parameters.MethodReplacementProvider;
            _aroundInvokeProvider = parameters.AroundInvokeProvider;
            _invocationInfo = parameters.InvocationInfo;
            _returnValue = parameters.ReturnValue;
            _interceptionDisabled = parameters.InterceptionDisabled;

            var getMethodReplacementProvider = new GetMethodReplacementProvider(_methodReplacementProvider, parameters.TargetMethod, parameters.GetMethodReplacementProviderMethod);

            _getMethodReplacementProvider = getMethodReplacementProvider;
            _registryType = parameters.RegistryType;
        }

        public SurroundMethodBody(VariableDefinition methodReplacementProvider,
            VariableDefinition aroundInvokeProvider,
            VariableDefinition invocationInfo,
            VariableDefinition interceptionDisabled,
            VariableDefinition returnValue, Type registryType)
        {
            _methodReplacementProvider = methodReplacementProvider;
            _aroundInvokeProvider = aroundInvokeProvider;
            _invocationInfo = invocationInfo;
            _interceptionDisabled = interceptionDisabled;
            _returnValue = returnValue;
            _registryType = registryType;
        }

        public void AddProlog(CilWorker IL)
        {
            var method = IL.GetMethod();
            _surroundingImplementation = method.AddLocal<IAroundInvoke>();
            _surroundingClassImplementation = method.AddLocal<IAroundInvoke>();

            var skipProlog = IL.Create(OpCodes.Nop);
            var declaringType = method.DeclaringType;
            var module = declaringType.Module;
            var modifiableType = module.ImportType<IModifiableType>();

            if (method.HasThis)
            {
                IL.Emit(OpCodes.Ldarg_0);
                IL.Emit(OpCodes.Isinst, modifiableType);
                IL.Emit(OpCodes.Brfalse, skipProlog);
            }

            IL.Emit(OpCodes.Ldloc, _interceptionDisabled);
            IL.Emit(OpCodes.Brtrue, skipProlog);

            // var provider = this.MethodReplacementProvider;

            if (_getMethodReplacementProvider != null)
                _getMethodReplacementProvider.Emit(IL);

            var getAroundInvokeProvider = new GetAroundInvokeProvider(_aroundInvokeProvider);
            getAroundInvokeProvider.Emit(IL);

            // if (aroundInvokeProvider != null ) {
            var skipGetSurroundingImplementation = IL.Create(OpCodes.Nop);
            var getSurroundingImplementationInstance = new GetSurroundingImplementationInstance(_aroundInvokeProvider,
                _invocationInfo, _surroundingImplementation, skipGetSurroundingImplementation);

            getSurroundingImplementationInstance.Emit(IL);

            // }

            IL.Append(skipGetSurroundingImplementation);
            var emitBeforeInvoke = new EmitBeforeInvoke(_invocationInfo, _surroundingClassImplementation,
                                                        _surroundingImplementation, _registryType);
            emitBeforeInvoke.Emit(IL);

            IL.Append(skipProlog);
        }

        public void AddEpilog(CilWorker IL)
        {
            var skipEpilog = IL.Create(OpCodes.Nop);

            // if (!IsInterceptionDisabled && surroundingImplementation != null) {
            IL.Emit(OpCodes.Ldloc, _interceptionDisabled);
            IL.Emit(OpCodes.Brtrue, skipEpilog);

            // surroundingImplementation.AfterInvoke(invocationInfo, returnValue);
            var emitAfterInvoke = new EmitAfterInvoke(_surroundingImplementation, _surroundingClassImplementation, _invocationInfo, _returnValue);
            emitAfterInvoke.Emit(IL);

            // }
            IL.Append(skipEpilog);
        }
    }
}
