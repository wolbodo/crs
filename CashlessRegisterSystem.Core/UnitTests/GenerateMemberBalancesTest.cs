using System;
using System.Collections.Generic;
using CashlessRegisterSystemCore.Helpers;
using CashlessRegisterSystemCore.Tasks;
using NUnit.Framework;

namespace CashlessRegisterSystemCore.UnitTests
{
    [TestFixture]
    public class GenerateMemberBalancesTest
    {
        [SetUp]
        public void Init()
        {
        }

        [Test]
        public void MonthDiffTest()
        {
            Assert.AreEqual(1, new DateTime(2013, 1, 1).MonthDiff(new DateTime(2012, 12, 1)));
            Assert.AreEqual(2, new DateTime(2013, 2, 1).MonthDiff(new DateTime(2012, 12, 1)));
            //Assert.AreEqual(0, GenerateMonthBalances.MonthDiff(new DateTime(2013, 1, 1), new DateTime(2013, 1, 23)));
            //Assert.Throws<Exception>(delegate { GenerateMonthBalances.MonthDiff(new DateTime(2013, 2, 1), new DateTime(2013, 1, 1)); });
        }

        [Test]
        public void GetValidBalanceDateTest()
        {
            var dates = new List<DateTime>();
            dates.Add(new DateTime(2013, 1, 1));
            dates.Add(new DateTime(2013, 2, 1));
            dates.Add(new DateTime(2013, 3, 1));
            Assert.AreEqual(new DateTime(2013, 3, 1), GenerateMonthBalances.GetValidBalanceDate(dates));
            dates.RemoveAt(1);
            Assert.AreEqual(new DateTime(2013, 1, 1), GenerateMonthBalances.GetValidBalanceDate(dates));
        }
    }
}
