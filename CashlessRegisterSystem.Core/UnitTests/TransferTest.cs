using System.Collections.Generic;
using CashlessRegisterSystemCore.Model;
using NUnit.Framework;

namespace CashlessRegisterSystemCore.UnitTests
{
    public class TransferTest
    {
        [SetUp]
        public void Init()
        {
            transferLines = transferText.Split(new[] { '\r', '\n' });
        }

        [Test]
        public void LoadMembersTest()
        {
            var transfers = new List<Transfer>();
            foreach (var transferLine in transferLines)
            {
                var transfer = Transfer.Parse(transferLine);
                if(transfer != null) transfers.Add(transfer);
            }
            Assert.AreEqual(6, transfers.Count);
        }

        private string[] transferLines;
        private string transferText = @"#Transfer voor maand: 2012-11";
    }
}
