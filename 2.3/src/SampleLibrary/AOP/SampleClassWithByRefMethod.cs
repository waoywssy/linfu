﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleLibrary.AOP
{
    public class SampleClassWithByRefMethod
    {
        public void ByRefMethod(ref int a)
        {
        }
        public void NonByRefMethod(int a)
        {            
        }
    }
}
