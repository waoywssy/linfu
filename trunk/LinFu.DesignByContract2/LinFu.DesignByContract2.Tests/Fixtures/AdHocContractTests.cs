using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using LinFu.DesignByContract2.Contracts;
using LinFu.DesignByContract2.Core;
using NMock2;
using NUnit.Framework;

namespace LinFu.DesignByContract2.Tests
{
    // TODO: Add support for error messages
    [TestFixture]
    public class AdHocContractTests : BaseFixture
    {
        [Test]
        public void ShouldBeAbleToCreatePreconditions()
        {
            AdHocContract contract = new AdHocContract();

            Predicate<MethodInfo> givenName = delegate(MethodInfo method)
                                                  {
                                                      return method.Name == "Open";
                                                  };

            Predicate<IDbConnection> shouldBeClosed = delegate(IDbConnection connection)
                                                          {
                                                              return connection.State == ConnectionState.Closed;
                                                          };

            Require.On(contract).ForMethodWith(givenName).That<IDbConnection>(shouldBeClosed)
                .OtherwisePrint("This connection should be closed before you call the Open() method");

            Assert.IsTrue(contract.Preconditions.Count > 0);
            Assert.IsNotNull(contract.Preconditions[0]);
        }

        [Test]
        public void ShouldBeAbleToCreatePostconditions()
        {
            AdHocContract contract = new AdHocContract();
            
            Predicate<MethodInfo> givenName = delegate(MethodInfo method)
                                      {
                                          return method.Name == "Open";
                                      };

            Predicate<IDbConnection> isConnected = delegate(IDbConnection connection)
                                                       {
                                                           return connection.State == ConnectionState.Open;
                                                       };
            
            Ensure.On(contract).ForMethodWith(givenName).That(isConnected).OtherwisePrint("Postcondition failed!");
            Assert.IsTrue(contract.Postconditions.Count > 0);
            Assert.IsNotNull(contract.Postconditions[0]);
        }

        [Test]
        public void ShouldBeAbleToCreateInvariants()
        {
            Type testType = typeof (IList<int>);
            ITypeContract contract = mock.NewMock<ITypeContract>();
            
            Predicate<Type> isIntList = delegate(Type currentType)
                                            {
                                                return currentType == testType;   
                                            };

            IList<IInvariant> mockInvariantList = mock.NewMock<IList<IInvariant>>();
            Stub.On(contract).GetProperty("Invariants").Will(Return.Value(mockInvariantList));
            
            // Add an invariant condition that should always be true
            Expect.AtLeastOnce.On(mockInvariantList).Method("Add").WithAnyArguments();
            Invariant.On(contract)
                .WhereTypeIs(isIntList)
                .IsAlwaysTrue<IList<int>>(CountNeverBelowZero)
                .OtherwisePrint("This condition should always be true"); ;
            
                        
            // Add an invariant condition that should always be false
            
            Invariant.On(contract).WhereTypeIs(isIntList).IsAlwaysFalse<IList<int>>(CountBelowZero)
                .OtherwisePrint("This condition should always be false");
            
            
            // Emulate Eiffel's 'implies' syntax
            
            Invariant.On(contract).WhereTypeIs(isIntList)
                .HavingCondition<IList<int>>(ListNotEmpty)
                .ImpliesThat(CountNotZero)
                .OtherwisePrint("Invariant error");
                                               
        }

        private static bool ListNotEmpty(IList<int> list)
        {
            return list.Count > 0;
        }
        private static bool CountNeverBelowZero(IList<int> list)
        {
            return list.Count >= 0;
        }
        private static bool CountNotZero(IList<int> list)
        {
            return list.Count != 0;
        }
        private static bool CountBelowZero(IList<int> list)
        {
            return list.Count < 0;
        }
    }
}
