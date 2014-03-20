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
        private string transferText = @"#Transfer voor maand: 2012-11
2012-11-29;Djoeke;50;Bank; ANDEREN Mw D S T Ardon ANDEREN           contributie!!                    transactiedatum: 29-11-2012
2012-11-27;Amber;50;Incasso; DELFT A M LEEMAN DELFT                 transactiedatum: 27-11-2012
2012-11-26;Jitske;115;Cash; Mw J N Folkertsma BELGIE         rest afbetaling.                 transactiedatum: 26-11-2012
2012-11-21;Remy;300;Bonnetje; DELFT Hr R van Rooijen DELFT           schuldenlijst                    transactiedatum: 21-11-2012
2012-11-12;Judith;75;Contributie; SCHULD JUDITH
2012-11-12;Annette;50;Activiteiten; AFLOSSING SCHULD OKTOBER +       VOORSCHOT";
    }
}
