using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CashlessRegisterSystemCore.Helpers;
using CashlessRegisterSystemCore.Model;
using OfficeOpenXml;

namespace CashlessRegisterSystemCore.Tasks
{
    public static class ProcessBankTransfers
    {
        public static List<string> ReadBankTransferLines(string directory, string startsWithFiles)
        {
            var result = new List<string>();
            var files = Directory.GetFiles(directory, startsWithFiles + "*");
            foreach (var file in files)
            {
                result.AddRange(ReadBankTransferLines(file));
            }
            return result;
        }

        public static List<string> ReadBankTransferLines(string fileName)
        {
            var result = new List<string>();
            string[] lines = File.ReadAllLines(fileName, Encoding.UTF8);
            for (int i = 0; i < lines.Length; i++)
            {
                result.Add(lines[i].Replace("\"", ""));
            }
            return result;
        } 

        public static List<BankTransfer> ParseBankTransferLines(List<string> transferLines)
        {
            var list = new List<BankTransfer>();
            bool skipFirst = true;
            foreach (string line in transferLines)
            {
                if (skipFirst)
                {
                    skipFirst = false; 
                    continue;
                }
                if (line.StartsWith("#") || string.IsNullOrEmpty(line)) continue;
                try
                {
                    var transfer = BankTransfer.Parse(line);
                    if (transfer == null) continue;
                    list.Add(transfer);
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Error parsing bank transfer: " + line);
                }
            }
            return list;
        }

        public static List<BankTransfer> FilterAssociateMembers(List<BankTransfer> transferList, List<Member> members, out List<BankTransfer> filteredTransfers)
        {
            var result = new List<BankTransfer>();
            var filteredResult = new List<BankTransfer>();
            foreach (var bankTransfer in transferList)
            {
                var member = members.SingleOrDefault(x => x.AccountNumber != string.Empty && x.AccountNumber.ToLower() == bankTransfer.AccountNumber.ToLower());
                if (member != null)
                {
                    result.Add(bankTransfer);
                }
                else
                {
                    filteredResult.Add(bankTransfer);
                }
                bankTransfer.AssociatedMember = member;
            }
            filteredTransfers = filteredResult;
            return result;
        }

        public static List<string> ReadFilteredTransfersFromExcel(string file)
        {
            if (!File.Exists(file)) throw new IOException("File does not exist: " + file);
            var results = new List<string>();
            var existingFile = new FileInfo(file);
            using (var package = new ExcelPackage(existingFile))
            {
                var workBook = package.Workbook;
                if (workBook == null) throw new Exception("Workbook does not exist");
                if (workBook.Worksheets.Count == 0) throw new Exception("Workbook does not contain any sheets: " + file);

                // only read first sheet
                var sheet = workBook.Worksheets.First();

                var dimension = sheet.Dimension;
                if (dimension == null) throw new Exception("Sheet is empty: " + file);
                for (var rowNum = 1; rowNum <= sheet.Dimension.End.Row; rowNum++)
                {
                    var wsRow = sheet.Cells[rowNum, 1, rowNum, sheet.Dimension.End.Column];
                    if (!wsRow.Any()) continue; // skip empty
                    string firstColumn = wsRow[rowNum, 1].Text;
                    DateTime date; // check date
                    bool isDate = DateTime.TryParseExact(firstColumn, "yyyy'-'MM'-'dd", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out date);
                    if (!isDate) continue;
                    string line = string.Empty;
                    if (sheet.Dimension.End.Column < 8) throw new Exception("Invalid number of columns for row " + rowNum);
                    line += wsRow[rowNum, 1].Text + ";";
                    string type = wsRow[rowNum, 3].Text;
                    if(string.IsNullOrEmpty(type)) throw new Exception("Transfer type is empty for row: " + rowNum);
                    line += wsRow[rowNum, 4].Text + ";"; // member
                    line += wsRow[rowNum, 7].Text + ";"; // amount
                    line += type + ";";
                    line += wsRow[rowNum, 8].Text; // note
                    if (!string.IsNullOrEmpty(line)) results.Add(line);
                }
            }
            return results;
        }

        public static List<Transfer> ReadBankTransfers(string file, List<Member> members, int month, int year)
        {
            if (!File.Exists(file)) throw new IOException("File does not exist: " + file);
            var result = new List<Transfer>();
            var lines = ReadFilteredTransfersFromExcel(file);
            foreach (string line in lines)
            {
                if (line.StartsWith("#") || string.IsNullOrEmpty(line)) continue;
                var transfer = Transfer.Parse(line);
                if (transfer != null)
                {
                    var member = members.SingleOrDefault(x => x.Name.ToLower() == transfer.MemberName.ToLower());
                    if (member != null)
                    {
                        transfer.Member = member;
                        result.Add(transfer);
                    }
                    else
                    {
                        Console.WriteLine("Could not find member for banktransfer: " + line);
                        //throw new Exception("Could not find member for banktransfer: " + line);
                    }

                }
            }
            return result;
        }
    }
}
