using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC.Configuration;
using SampleLibrary;

namespace LinFu.UnitTests.IOC.Configuration
{
    [Implements(typeof(ISampleService), LifecycleType.OncePerRequest, ServiceName="MyService")]
    public class NamedOncePerRequestSampleService : ISampleService
    {
    }
}
