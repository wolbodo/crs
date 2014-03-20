using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CashlessRegisterSystemCore.Model;
using OfficeOpenXml;

namespace CashlessRegisterSystemCore.Tasks
{
    public class ProcessManualTransfers
    {
        public static List<string> ReadTransfersFromExcel(string file)
        {
            if(!File.Exists(file)) throw new IOException("File does not exist: " + file);
            var results = new List<string>();
            var existingFile = new FileInfo(file);
            using (var package = new ExcelPackage(existingFile))
            {
                var workBook = package.Workbook;
                if (workBook == null) throw new Exception("Workbook does not exist"); 
                if(workBook.Worksheets.Count == 0) throw new Exception("Workbook does not contain any sheets: " + file);

                // only read first sheet
                var sheet = workBook.Worksheets.First();

                var dimension = sheet.Dimension;
                if(dimension == null) throw new Exception("Sheet is empty: " + file);
                for (var rowNum = 1; rowNum <= sheet.Dimension.End.Row; rowNum++)
                {
                    var wsRow = sheet.Cells[rowNum, 1, rowNum, sheet.Dimension.End.Column];
                    string row = String.Empty;
                    if(!wsRow.Any()) continue; // skip empty
                    string firstColumn = wsRow[rowNum, 1].Text;
                    DateTime date; // check date
                    bool isDate = DateTime.TryParseExact(firstColumn, "dd-MM-yyyy", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out date);
                    if(!isDate) continue;
                    string line = string.Empty;
                    if (sheet.Dimension.End.Column < 4) throw new Exception("Invalid number of columns for row " + rowNum);
                    line += date.ToString("yyyy-MM-dd") + ";"; // date
                    string type = wsRow[rowNum, 2].Text; 
                    if (string.IsNullOrEmpty(type)) throw new Exception("Transfer type is empty for row: " + rowNum);
                    line += wsRow[rowNum, 3].Text + ";"; // member
                    line += decimal.Parse(wsRow[rowNum, 4].Text, NumberStyles.Currency).ToString() + ";"; // amount
                    line += type + ";"; // type
                    line += wsRow[rowNum, 5].Text; // note
                    if (!string.IsNullOrEmpty(line)) results.Add(line);
                }
            }
            return results;
        }

        public static List<Transfer> ReadBonnetjesTransfers(string file, List<Member> members, int month, int year)
        {
            if (!File.Exists(file)) throw new IOException("File does not exist: " + file);
            var result = new List<Transfer>();
            var lines = ReadTransfersFromExcel(file);
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
                        throw new Exception("Could not find member for banktransfer: " + line);
                    }
                }
            }
            return result;
        }
    }
}
