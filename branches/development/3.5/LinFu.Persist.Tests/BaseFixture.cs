using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace LinFu.Persist.Tests
{
    public abstract class BaseFixture
    {
        [SetUp]
        public virtual void Setup()
        {
        }
        [TearDown]
        public virtual void TearDown()
        {

        }
        [TestFixtureSetUp]
        public virtual void FixtureSetup()
        {
        }
        [TestFixtureTearDown]
        public virtual void FixtureTearDown()
        {
        }
    }
}
