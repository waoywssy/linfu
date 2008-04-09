﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinFu.Persist
{
    public class Row : IRow
    {
        private readonly Dictionary<string, ICell> _cells = new Dictionary<string, ICell>();

        public ITable Table
        {
            get;
            set;
        }
        public IDictionary<string, ICell> Cells
        {
            get
            {
                return _cells;
            }
        }
    }
}
