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
            Assert.AreEqual("BWB", transfers[0].MemberName);
            Assert.AreEqual(TransferTypeEnum.Incasso, transfers[0].Type);
            Assert.AreEqual(75.10, transfers[0].Amount);
            Assert.AreEqual("B.W. Broersma                                 schuldenlijst incasso                        ", transfers[0].Note);
        }

        private string incassoText = @"0001A060213CLIEOP03MPZ0199991                     
0010B1000002384170019EUR                          
0020ASchuldenlijst december                       
0030B1030113WOLBODO DELFT                      P  
0100A100100000000751003903629130000238417         
0110BB.W. Broersma                                
0160Aschuldenlijst incasso                        
0100A100100000001890003916784340000238417         
0110BM.C. Havranek                                
0160Aschuldenlijst incasso                        
0100A100100000001874001265856950000238417         
0110BR. van der Hee                               
0160Aschuldenlijst incasso                        
0100A100100000000687001329043810000238417         
0110BLM Ophey                                     
0160Asaldolijst                                   
0100A100100000000013301040823720000238417         
0110BSHLCG Vermeulen                              
0160Aschuldenlijst incasso                        
9990A00000000000005215311468058800000005          
9999A                                             
";

        private List<string> memberLines;
        private string membersText = @"Amber;??????;PrePaid;0;4399335;A.M. Leeman;2012-07-23;3404
BWB;??????;Incasso;5000;390362913;B.W. Broersma;2012-07-23;5000
Junior;??????;Incasso;0;126585695;R. van der Hee;2012-07-23;0
Lotte;??????;Incasso;0;132904381;L. Ophey;2012-07-23;-13426
Martin;??????;Incasso;0;391678434;M.C. Havranek;2012-07-23;-13426
Trui;??????;Incasso;0;104082372;S. Vermeulen;2012-07-23;-13426";

    }
}
