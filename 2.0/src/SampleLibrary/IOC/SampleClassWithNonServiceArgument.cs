using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleLibrary.IOC
{
    public class SampleClassWithNonServiceArgument
    {
        private readonly string _value;
        public SampleClassWithNonServiceArgument(string value)
        {
            _value = value;
        }
        public string Value
        {
            get { return _value; }
        }
    }
}
