using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinFu.Finders;
using LinFu.Finders.Interfaces;
using Moq;
using NUnit.Framework;

namespace LinFu.UnitTests.Finders
{
    [TestFixture]
    public class FinderTests : BaseTestFixture
    {
        [Test]
        public void ShouldBeAbleToAddCriteriaToList()
        {
            // Return a predicate that always returns true
            var mockCriteria = new Mock<ICriteria<object>>();
            var criteria = mockCriteria.Object;

            var mockFuzzyItem = new Mock<IFuzzyItem<object>>();
            var fuzzyItem = mockFuzzyItem.Object;

            // The Test method must be called on the fuzzy item
            mockFuzzyItem.Expect(fuzzy => fuzzy.Test(criteria));

            // Initialize the list of fuzzy items
            var fuzzyList = new List<IFuzzyItem<object>>();
            fuzzyList.Add(fuzzyItem);

            // Apply the criteria
            fuzzyList.AddCriteria(criteria);

            mockCriteria.VerifyAll();
            mockFuzzyItem.VerifyAll();
        }

        [Test]
        public void ShouldBeAbleToDetermineBestFuzzyMatch()
        {
            var mockFuzzyItem = new Mock<IFuzzyItem<object>>();
            var fuzzyItem = mockFuzzyItem.Object;

            // This should be the best match
            mockFuzzyItem.Expect(f => f.Confidence).Returns(.8);

            var otherMockFuzzyItem = new Mock<IFuzzyItem<object>>();
            var fauxFuzzyItem = otherMockFuzzyItem.Object;

            // This fuzzy item should be ignored since it has
            // a lower confidence rate
            otherMockFuzzyItem.Expect(f => f.Confidence).Returns(.1);

            var fuzzyList = new List<IFuzzyItem<object>> { fuzzyItem, fauxFuzzyItem };

            var bestMatch = fuzzyList.BestMatch();
            Assert.AreSame(bestMatch, fuzzyItem);
        }
    }
}
