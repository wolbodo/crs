using System;
using System.Collections.Generic;
using CashlessRegisterSystemCore.Model;
using CashlessRegisterSystemCore.Tasks;
using NUnit.Framework;

namespace CashlessRegisterSystemCore.UnitTests
{
    [TestFixture]
    public class ProcessIncassoTransfersTest
    {
        private List<string> incassoLines;
        private MemberList members;

        [SetUp]
        public void Init()
        {
            incassoLines = new List<string>();
            incassoLines.AddRange(incassoText.Split(new[] { '\r', '\n' }));

            memberLines = new List<string>();
            memberLines.AddRange(membersText.Split(new[] { '\r', '\n' }));
            members = new MemberList(true, Environment.CurrentDirectory);
            members.Init(memberLines);
        }

        [Test]
        public void ParseIncassoTransferLinesTest()
        {
            var transfers = ProcessIncassoTransfers.ParseIncassoTransferLines(incassoLines, members.AllList);

            Assert.AreEqual(5, transfers.Count);
            Assert.AreEqual(new DateTime(2013, 01, 03), transfers[0].Date);
            Assert.AreEqual("", transfers[0].MemberName);
            Assert.AreEqual(TransferTypeEnum.Incasso, transfers[0].Type);
            Assert.AreEqual(75.10, transfers[0].Amount);
            Assert.AreEqual("", transfers[0].Note);
        }

        private string incassoText = @"";

        private List<string> memberLines;
        private string membersText = @"";

    }
}
