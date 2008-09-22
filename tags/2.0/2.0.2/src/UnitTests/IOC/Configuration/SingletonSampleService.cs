﻿using LinFu.IoC.Configuration;
using SampleLibrary;

namespace LinFu.UnitTests.IOC.Configuration
{
    [Implements(typeof(ISampleService), LifecycleType.Singleton)]
    public class SingletonSampleService : ISampleService
    {
    }
}