using System;
using System.Collections.Generic;
using CashlessRegisterSystemCore.Model;
using CashlessRegisterSystemCore.Tasks;
using NUnit.Framework;

namespace CashlessRegisterSystemCore.UnitTests
{
    [TestFixture]
    public class ProcessBankTransactionsTest
    {
        private List<BankTransfer> bankTransfers;
        private MemberList members;

        [SetUp]
        public void Init()
        {
            var lines = new List<string>();
            lines.AddRange(transfersText.Split(new[] { '\r', '\n' }));
            bankTransfers = ProcessBankTransfers.ParseBankTransferLines(lines);

            memberLines = new List<string>();
            memberLines.AddRange(membersText.Split(new[] { '\r', '\n' }));
            members = new MemberList(true, Environment.CurrentDirectory);
            members.Init(memberLines);
        }

        [Test]
        public void ParseBankTransferLinesTest()
        {
            Assert.AreEqual(8, bankTransfers.Count);
            Assert.AreEqual(new DateTime(2013, 01, 02), bankTransfers[0].PaymentDate);
            Assert.AreEqual("3056108", bankTransfers[0].AccountNumber);
            Assert.AreEqual(BankTransferTypeEnum.Internetbankieren, bankTransfers[0].TypeEnum);
            Assert.AreEqual(1.20, bankTransfers[0].Amount);
        }

        [Test]
        public void ParseFilterAssociateMembersTest()
        {
            Assert.AreEqual(8, bankTransfers.Count);
            List<BankTransfer> filtered;
            var filteredList = ProcessBankTransfers.FilterAssociateMembers(bankTransfers, members.AllList, out filtered);
            Assert.AreEqual(2, filteredList.Count);
            Assert.AreEqual(6, filtered.Count);
        }

        private string transfersText = @"Datum,Naam / Omschrijving,Rekening,Tegenrekening,Code,Af Bij,Bedrag (EUR),MutatieSoort,Mededelingen";

        private List<string> memberLines;
        private string membersText = @"#Name[string];KeyCode[(hex)string];PaymentMethod[enum:Incasso|PrePaid];IncassoLevel[int in cents];AccountNumber[string];AccountName[string];BalanceDate[date:yyyy-MM-dd];Balance[int in cents]";
    }
}
