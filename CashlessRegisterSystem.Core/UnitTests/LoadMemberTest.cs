using System;
using System.Collections.Generic;
using CashlessRegisterSystemCore.Model;
using NUnit.Framework;

namespace CashlessRegisterSystemCore.UnitTests
{
    [TestFixture]
    public class LoadMemberTest
    {
        [SetUp]
        public void Init()
        {
            memberLines = new List<string>();
            memberLines.AddRange(membersText.Split(new[] { '\r', '\n' }));
        }

        [Test]
        public void LoadMembersTest()
        {
            var memberList = new MemberList(true, Environment.CurrentDirectory);
            memberList.Init(memberLines);
            Assert.AreEqual(4, memberList.All.Count);
        }

        private List<string> memberLines;
        private string membersText = @"#Name[string];KeyCode[(hex)string];PaymentMethod[enum:Incasso|PrePaid];IncassoLevel[int in cents];AccountNumber[string];AccountName[string];BalanceDate[date:yyyy-MM-dd];Balance[int in cents]
#Wikkert;A1B2C3;Incasso;50000;123456;W. Olbodo;2013-01-14;1250
#This means: Wikkert has key A1B2C3; pays with Incasso (level to € 50.00 positive); has the bank account 123456 with the name W. Olbodo; last time (2013-01-14) he had a balace of ? 12.50;;;;";
    }
}
