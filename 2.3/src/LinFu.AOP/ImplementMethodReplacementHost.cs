﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.AOP.Cecil.Interfaces;
using LinFu.AOP.Interfaces;
using LinFu.Reflection.Emit;
using Mono.Cecil;

namespace LinFu.AOP.Cecil
{
    internal class ImplementMethodReplacementHost : ITypeWeaver
    {
        private TypeReference _hostType;
        private readonly Func<TypeReference, bool> _filter;

        public ImplementMethodReplacementHost(Func<TypeReference, bool> filter)
        {
            _filter = filter;
        }

        public bool ShouldWeave(TypeDefinition item)
        {
            if (item.Name == "<Module>")
                return false;

            if (!item.IsClass || item.IsInterface)
                return false;

            // Implement the host interface once and only once
            if (item.Interfaces.Contains(_hostType))
                return false;

            if (_filter != null)
                return _filter(item);

            return true;
        }

        public void Weave(TypeDefinition item)
        {
            item.Interfaces.Add(_hostType);
            item.AddProperty("MethodReplacementProvider", typeof(IMethodReplacementProvider));
        }

        public void AddAdditionalMembers(ModuleDefinition host)
        {
        }

        public void ImportReferences(ModuleDefinition module)
        {
            _hostType = module.Import(typeof(IMethodReplacementHost));
        }
    }
}
