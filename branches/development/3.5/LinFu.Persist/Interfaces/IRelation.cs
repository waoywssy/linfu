﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist
{
    public interface IRelation
    {
        IKey SourceKey { get; set; }
        IKey TargetKey { get; set; }
    }
}
