using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.IoC;
using LinFu.IoC.Configuration.Interfaces;
using NUnit.Framework;
using SampleLibrary.IOC;

namespace LinFu.UnitTests.IOC
{
    [TestFixture]
    public class PropertyInjectionTests : BaseTestFixture
    {
        [Test]
        public void ShouldDetermineWhichPropertiesShouldBeInjected()
        {
            var targetType = typeof(SampleClassWithInjectionProperties);
            var targetProperty = targetType.GetProperty("SomeProperty");
            Assert.IsNotNull(targetProperty);

            // Load the property injection filter by default
            var container = new ServiceContainer();
            container.LoadFrom(AppDomain.CurrentDomain.BaseDirectory, "LinFu*.dll");

            var filter = container.GetService<IPropertyInjectionFilter>();

            Assert.IsNotNull(filter);

            var properties = filter.GetInjectableProperties(targetType);
            Assert.IsTrue(properties.Count() > 0);

            var result = properties.First();
            Assert.AreEqual(targetProperty, result);
        }
    }
}
