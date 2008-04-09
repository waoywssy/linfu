﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist
{
    public class Cell : ICell
    {
        public object Value
        {
            get;
            set;
        }
        public override string ToString()
        {
            if (Value == null)
                return base.ToString();

            return Value.ToString();
        }
    }
}
