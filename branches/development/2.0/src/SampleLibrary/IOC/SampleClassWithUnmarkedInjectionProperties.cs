using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleLibrary.IOC
{
    public class SampleClassWithUnmarkedInjectionProperties : ISampleService
    {
        public ISampleService SomeProperty { get; set; }
    }
}
