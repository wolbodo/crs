using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CashlessRegisterSystemCore.Model;

namespace CashlessRegisterSystemCore.Tasks
{
    public static class ProcessIncassoTransfers
    {
        public static List<Transfer> ParseIncassoTransferLines(string directory, string startsWithFiles, List<Member> members, int month, int year)
        {
            var lines = ReadIncassoTransferLines(directory, startsWithFiles);
            return ParseIncassoTransferLines(lines, members);
        }

        public static List<Transfer> ParseIncassoTransferLines(string file, List<Member> members, int month, int year)
        {
            var lines = ReadIncassoTransferLines(file);
            return ParseIncassoTransferLines(lines, members);
        }

        public static List<string> ReadIncassoTransferLines(string directory, string startsWithFiles)
        {
            var result = new List<string>();
            var files = Directory.GetFiles(directory, startsWithFiles + "*");
            foreach (var file in files)
            {
                ReadIncassoTransferLines(file);
            }
            return result;
        }

        public static List<string> ReadIncassoTransferLines(string fileName)
        {
            var result = new List<string>();
            string[] lines = File.ReadAllLines(fileName, Encoding.UTF8);
            result.AddRange(lines);
            return result;
        } 

        public static List<Transfer> ParseIncassoTransferLines(List<string> transferLines, List<Member> members)
        {
            var list = new List<Transfer>();
            DateTime date = DateTime.MinValue;
            for (int i = 0; i < transferLines.Count; i++)
            {
                // get date
                if (transferLines[i].StartsWith("0030B1"))
                {
                    string dateText = transferLines[i].Replace("0030B1", string.Empty).Substring(0, 6);
                    date = DateTime.ParseExact(dateText, "ddMMyy", DateTimeFormatInfo.InvariantInfo);
                }
                // incasso lines
                if (transferLines[i].StartsWith("0100A1001"))
                {
                    // parse incasso
                    if (date == DateTime.MinValue) throw new Exception("Date of incasso not yet parsed!!! " + transferLines[i]);
                    string line = transferLines[i].Replace("0100A1001", string.Empty); // amount, accountNr
                    string amountText = line.Substring(0, 12);
                    decimal amount = decimal.Parse(amountText)/100;
                    string accountNr = line.Replace(amountText, string.Empty).Substring(0, 10).TrimStart('0');
                    string note = transferLines[i + 1].Substring(5); // name 
                    note += " " + transferLines[i + 2].Substring(5); // description
                    i = i + 2;

                    // get member
                    var member = members.SingleOrDefault(x => x.AccountNumber == accountNr);
                    if(member == null) throw new Exception("Could not associate member for incasso: " + line);
                    // create line
                    string transferLine = date.ToString("yyyy-MM-dd") + ";" + member.Name + ";" + amount + ";" + TransferTypeEnum.Incasso + ";" + note;
                    
                    // convert to transfer
                    var transfer = Transfer.Parse(transferLine);
                    if (transfer == null) continue;
                    transfer.Member = member;
                    list.Add(transfer);
                }
            }
            return list;
        }

       
    }
}
