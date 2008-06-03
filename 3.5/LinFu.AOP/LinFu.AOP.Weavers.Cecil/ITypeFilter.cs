﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace LinFu.AOP.Weavers.Cecil
{
    public interface ITypeFilter
    {
        bool ShouldWeave(TypeDefinition type);
    }
}
