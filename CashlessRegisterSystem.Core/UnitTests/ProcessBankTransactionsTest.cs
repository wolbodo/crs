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

        private string[] transferLines;
        private string transfersText = @"Datum,Naam / Omschrijving,Rekening,Tegenrekening,Code,Af Bij,Bedrag (EUR),MutatieSoort,Mededelingen
02-01-2013,Hr E S Roozen,238417,3056108,GT,Bij,1,20,Internetbankieren, ROTTERDAM Hr E S Roozen ROTTERDAM          viltjes Erik                     transactiedatum: 02-01-2013
02-01-2013,Hr M Oomen,238417,8672870,GT,Bij,19,50,Internetbankieren, DELFT Hr M Oomen DELFT                 Schuld Max Laatste lijst 2012    transactiedatum: 02-01-2013
02-01-2013,STICHTING INTRA MUROS DELFT     ,238417,1942033,GT,Af,3648,00,Internetbankieren, Servicekosten Q3, Q4
02-01-2013,H J MANDERS,238417,2722538,GT,Bij,50,00,Internetbankieren, DELFT H J MANDERS DELFT                Van Jeroen, met de beste wensen! transactiedatum: 02-01-2013
02-01-2013,T VERBRUGGEN,238417,452570255,OV,Bij,120,00,Overschrijving,
02-01-2013,STORTING 02-01-2013 11:39 80122GL2,238417,,ST,Bij,115,00,Storting, TRANSACTIENR 13229126, 002       BONNR 91260                     
02-01-2013,Hr T W Agema,238417,6822615,GT,Bij,200,00,Internetbankieren, Hr T W Agema BELGIE              transactiedatum: 02-01-2013
02-01-2013,Hr M Brouns,238417,8869110,GT,Bij,14,20,Internetbankieren, S GRAVENHAGE Hr M Brouns S GRAVENHAGE         transactiedatum: 02-01-2013
02-01-2013,STORTING 02-01-2013 11:25 009131,238417,,ST,Bij,5530,00,Storting, TRANSACTIENR 14057920, 002       BONNR 85631                     
02-01-2013,B.V. BOLDERDIJK,238417,142869678,OV,Bij,80,00,Overschrijving, VILTJES EN BIER BIJ DE LAN *12   EURO* PLUS EEN BEETJE TEGOED
02-01-2013,AJ HEAD,238417,439925436,OV,Bij,10,00,Overschrijving, SCHULD VAN ADAM **
02-01-2013,ROZEMA VERHUUR BV DELFT         ,238417,652854,GT,Af,105,96,Internetbankieren, factuurnummer 12503435           debiteur 82079
02-01-2013,Wines & Wiskies                 ,238417,319400077,GT,Af,161,19,Internetbankieren, factuurnummer 1634               bedankt maar weer!
02-01-2013,Hr J van Lenteren DELFT         ,238417,8076600,GT,Af,60,00,Internetbankieren, servicekosten november
02-01-2013,Hr N van der Pas DELFT          ,238417,7901073,GT,Af,60,00,Internetbankieren, servicekosten november
02-01-2013,Mw L Moerenhout DELFT           ,238417,5046417,GT,Af,60,00,Internetbankieren, servicekosten november
02-01-2013,Hr R J Aalbers DELFT            ,238417,8156257,GT,Af,60,00,Internetbankieren, servicekosten november
02-01-2013,BIERENKO AMSTERDAM BV,238417,298793113,IC,Af,527,41,Incasso, FAC.12046523";

        private List<string> memberLines;
        private string membersText = @"#Name[string];KeyCode[(hex)string];PaymentMethod[enum:Incasso|PrePaid];IncassoLevel[int in cents];AccountNumber[string];AccountName[string];BalanceDate[date:yyyy-MM-dd];Balance[int in cents]
#Wikkert;A1B2C3;Incasso;50000;123456;W. Olbodo;2013-01-14;1250
#This means: Wikkert has key A1B2C3; pays with Incasso (level to € 50.00 positive); has the bank account 123456 with the name W. Olbodo; last time (2013-01-14) he had a balace of ? 12.50;;;;
Amber;??????;PrePaid;0;6822615;A.M. Leeman;2012-07-23;3404
BWB;??????;Incasso;5000;390362913;B.W. Broersma;2012-07-23;5000
Eettafel;??????;PrePaid;0;;;2012-07-23;0
Emiel;??????;PrePaid;0;8869110;E. Verstegen;2012-07-23;-13426";
    }
}
