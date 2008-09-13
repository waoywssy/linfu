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
        public void ShouldBeAbleToAddLambdasAsCriteria()
        {
            var fuzzyList = new List<IFuzzyItem<int>>();
            fuzzyList.Add(5);

            // Directly apply the predicate instead of
            // having to manually construct the criteria
            fuzzyList.AddCriteria(item => item == 5);

            Assert.AreEqual(5, fuzzyList.BestMatch().Item);
        }

        [Test]
        public void ShouldBeAbleToIgnoreFailedOptionalCriteria()
        {
            // The criteria will be set up to fail by default
            var falseCriteria = new Mock<ICriteria<object>>();            
            falseCriteria.Expect(c => c.Predicate).Returns(predicate => false);
            falseCriteria.Expect(c => c.IsOptional).Returns(true);
            falseCriteria.Expect(c => c.Weight).Returns(1);

            // Use the true criteria as the control result
            var trueCriteria = new Mock<ICriteria<object>>();
            trueCriteria.Expect(c => c.Predicate).Returns(predicate => true);
            trueCriteria.Expect(c => c.IsOptional).Returns(false);
            trueCriteria.Expect(c => c.Weight).Returns(1);

            var fuzzyItem = new FuzzyItem<object>(new object());

            var fuzzyList = new List<IFuzzyItem<object>>();
            fuzzyList.Add(fuzzyItem);

            // Apply the criteria
            fuzzyList.AddCriteria(trueCriteria.Object);
            fuzzyList.AddCriteria(falseCriteria.Object);

            // The score must be left unchanged
            // since the criteria is optional and
            // the failed predicate does not count
            // against the current fuzzy item.
            Assert.AreEqual(fuzzyItem.Confidence, 1);
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
