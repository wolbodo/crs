using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using CashlessRegisterSystemCore.Helpers;
using CashlessRegisterSystemCore.Model;
using NUnit.Framework;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace CashlessRegisterSystemCore.Tasks
{
    public static class GenerateYearTransactionsExcel
    {
        public static void Execute(int year)
        {
            try
            {
                var yearFiles = new List<string>();
                yearFiles.AddRange(Directory.GetFiles(Settings.LocalTransactionsPath, "transactions-" + year + "*.txt").OrderBy(x => x));
                var yearBalance = new YearBalance(year);
                foreach (var sourceFile in yearFiles)
                {
                    var info = new FileInfo(sourceFile);
                    int transactionFileMonth = TransactionFileHelper.GetMonth(sourceFile);
                    int transactionFileYear = TransactionFileHelper.GetYear(sourceFile);
                    // skip for the current month
                    if (DateTime.Now.Year == transactionFileYear && DateTime.Now.Month == transactionFileMonth) continue;
                    var transactionList = TransactionList.LoadFromFile(info);
                    //var monthBalances = new MonthBalance { Month = transactionList.Month, Year = transactionList.Year, Transactions = transactionList.All };
                    foreach (var transaction in transactionList.All)
                    {
                        Transaction transaction1 = transaction;
                        int month = transaction.TransactionDate.Month;
                        // get member
                        var member = yearBalance.Members.SingleOrDefault(x => x.Name == transaction1.MemberName);
                        if (member == null)
                        {
                            member = new Member {Name = transaction1.MemberName};
                            yearBalance.Members.Add(member);
                        }
                        // get month yearBalance
                        MonthBalance monthBalance;
                        if (!yearBalance.MonthBalances.ContainsKey(transaction.TransactionDate.Month))
                        {
                            monthBalance = new MonthBalance {Month = month, Year = year};
                            yearBalance.MonthBalances.Add(transaction.TransactionDate.Month, monthBalance);
                        }
                        else
                        {
                            monthBalance = yearBalance.MonthBalances[transaction.TransactionDate.Month];
                        }
                        // get member yearBalance
                        var memberBalance = monthBalance.MemberBalances.SingleOrDefault(x => x.Member.Name.ToLower() == transaction1.MemberName.ToLower());
                        if (memberBalance == null)
                        {
                            memberBalance = new MemberBalance {Month = month, Year = year, Member = member};
                            monthBalance.AddBalance(memberBalance);
                        }
                        // add transaction
                        memberBalance.ProcessTransaction(transaction);
                    }
                }
                CreateExcel(yearBalance);
            }
            catch (Exception e)
            {
                
            }
        }

        private static void CreateExcel(YearBalance yearBalance)
        {
            var name = "Streeplijst Wolbodo " + yearBalance.Year;
            var fileName = Path.Combine(Settings.LocalTransactionsPath, name + ".xlsx");
            // create member list based on transactions
            var newFile = new FileInfo(fileName);
            if (newFile.Exists)
            {
                newFile.Delete();
            }
            using (var excelPackage = new ExcelPackage(newFile))
            {
                string titleWorkbook = "Streeplijst Wolbodo " + yearBalance.Year;
                ExcelHelper.InitWorkbook(excelPackage.Workbook, titleWorkbook);
                var header = new List<string> {"Naam"};
                foreach (var month in Settings.DateTimeInfo.MonthNames)
                {
                    header.Add(month);
                }
                header.Add("Totaal");
                string titleSheet = "Streeplijst " + yearBalance.Year;
                var worksheet = excelPackage.Workbook.Worksheets.Add("Totaal " + yearBalance.Year);

                ExcelHelper.InitWorkSheet(worksheet);
                ExcelHelper.AddTitle(worksheet, titleSheet);
                ExcelHelper.AddHeader(worksheet, header.ToArray(), 2);
                int nrRows = 2;
                foreach (var member in yearBalance.Members.OrderBy(x=>x.Name))
                {
                    nrRows++;
                    AddMemberRow(worksheet, nrRows, yearBalance, member);
                }
                //if (monthBalance.MemberBalances.Count > 0) worksheet.Cells[3, 1, nrRows, header.Count].Style.Numberformat.Format = ExcelHelper.EuroFormat;
                //GenerateTransactionSheet(package.Workbook, yearBalance);

                // sum
                int lastRow = nrRows;
                nrRows++;
                //worksheet.Column(0).Width = 200;
                //worksheet.Column(1).Width = 120;
                worksheet.Cells[nrRows, 1].Value = "Totaal";
                worksheet.Cells[nrRows, 2].Formula = "SUM(" + Nr2Letter(2) + 2 + ":" + Nr2Letter(2) + lastRow + ")";
                worksheet.Cells[nrRows, 3].Formula = "SUM(" + Nr2Letter(3) + 2 + ":" + Nr2Letter(3) + lastRow + ")";
                worksheet.Cells[nrRows, 4].Formula = "SUM(" + Nr2Letter(4) + 2 + ":" + Nr2Letter(4) + lastRow + ")";
                worksheet.Cells[nrRows, 5].Formula = "SUM(" + Nr2Letter(5) + 2 + ":" + Nr2Letter(5) + lastRow + ")";
                worksheet.Cells[nrRows, 6].Formula = "SUM(" + Nr2Letter(6) + 2 + ":" + Nr2Letter(6) + lastRow + ")";
                worksheet.Cells[nrRows, 7].Formula = "SUM(" + Nr2Letter(7) + 2 + ":" + Nr2Letter(7) + lastRow + ")";
                worksheet.Cells[nrRows, 8].Formula = "SUM(" + Nr2Letter(8) + 2 + ":" + Nr2Letter(8) + lastRow + ")";
                worksheet.Cells[nrRows, 9].Formula = "SUM(" + Nr2Letter(9) + 2 + ":" + Nr2Letter(9) + lastRow + ")";
                worksheet.Cells[nrRows, 10].Formula = "SUM(" + Nr2Letter(10) + 2 + ":" + Nr2Letter(10) + lastRow + ")";
                worksheet.Cells[nrRows, 11].Formula = "SUM(" + Nr2Letter(11) + 2 + ":" + Nr2Letter(10) + lastRow + ")";
                worksheet.Cells[nrRows, 12].Formula = "SUM(" + Nr2Letter(12) + 2 + ":" + Nr2Letter(10) + lastRow + ")";
                worksheet.Cells[nrRows, 13].Formula = "SUM(" + Nr2Letter(13) + 2 + ":" + Nr2Letter(10) + lastRow + ")";
                using (var range = worksheet.Cells[nrRows, 1, nrRows, 13])
                {
                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    range.Style.Font.Bold = true;
                }
                worksheet.Cells[3, 2, nrRows + 3, 14].Style.Numberformat.Format = ExcelHelper.EuroFormat;
                worksheet.Column(1).Width = 30;
                worksheet.Column(2).Width = 12;
                worksheet.Column(3).Width = 12;
                worksheet.Column(4).Width = 12;
                worksheet.Column(5).Width = 12;
                worksheet.Column(6).Width = 12;
                worksheet.Column(7).Width = 12;
                worksheet.Column(8).Width = 12; 
                worksheet.Column(9).Width = 12;
                worksheet.Column(10).Width = 12;
                worksheet.Column(11).Width = 12;
                worksheet.Column(12).Width = 12;
                worksheet.Column(13).Width = 12;
                worksheet.Column(14).Width = 15;
                //worksheet.Cells[1, 1, nrRows + 1, header.Length].AutoFitColumns();
                //worksheet.PrinterSettings.FitToPage = true;
                //worksheet.PrinterSettings.Scale = 67;
                excelPackage.Save();
            }
        }

        internal static string Nr2Letter(int number)
        {
            bool isCaps = true;
            var c = (Char)((isCaps ? 65 : 97) + (number - 1));
            return c.ToString();
        }

        private static void GenerateTransactionSheet(ExcelWorkbook workbook, YearBalance balance)
        {
            //var sheet = workbook.Worksheets.Add("Streeplijst " + monthName);
            //string titleSheet = "Streeplijst " + monthName;
            //string[] headerBalance = { "Datum", "Tijdstip", "Naam", "Bedrag" };
            //ExcelHelper.InitWorkSheet(sheet);
            //ExcelHelper.AddTitle(sheet, titleSheet);
            //ExcelHelper.AddHeader(sheet, headerBalance, 2);
            //sheet.Cells[3, 4, balance.Transactions.Count + 2, 4].Style.Numberformat.Format = ExcelHelper.EuroFormat;
            //int transactionRowNr = 2;
            //for (int i = 0; i < balance.Transactions.Count; i++)
            //{
            //    transactionRowNr++;
            //    AddTransactionRow(sheet, transactionRowNr, balance.Transactions[i]);
            //}
            //sheet.Cells[2, 1, balance.Transactions.Count, 4].AutoFilter = true;
            ////sheet.Cells[1, 1, transactionRowNr, 8].AutoFitColumns();
            ////sheet.PrinterSettings.FitToPage = false;
            //sheet.Column(1).Width = 12;
            //sheet.Column(2).Width = 12;
            //sheet.Column(3).Width = 30;
            //sheet.Column(4).Width = 14;
        }

        private static void AddMemberRow(ExcelWorksheet sheet, int rowNr, YearBalance yearBalance,  Member member)
        {
            sheet.Cells[rowNr, 1].Value = member.Name;
            foreach (var monthBalance in yearBalance.MonthBalances)
            {
                var memberBalance = monthBalance.Value.MemberBalances.SingleOrDefault(x => x.Member.Name == member.Name);
                sheet.Cells[rowNr, 1 + monthBalance.Key].Value = memberBalance != null ? memberBalance.Coasters : 0;
                sheet.Cells[rowNr, 14].Formula = "SUM(" + Nr2Letter(2) + rowNr + ":" + Nr2Letter(13) + rowNr + ")";
            }
        }

        private static void AddTransactionRow(ExcelWorksheet sheet, int rowNr, Transaction transaction)
        {
            sheet.Cells[rowNr, 1].Value = transaction.TransactionDate.ToString("dd-MM-yyyy");
            sheet.Cells[rowNr, 2].Value = transaction.TransactionDate.ToString("HH:mm:ss");
            sheet.Cells[rowNr, 3].Value = transaction.MemberName;
            sheet.Cells[rowNr, 4].Value = (decimal)(transaction.AmountInCents / 100d);
        }

        private static void AddTransferRow(ExcelWorksheet sheet, int rowNr, int startColNr, Transfer transfer)
        {
            sheet.Cells[rowNr, startColNr].Value = transfer.Date.ToString("dd-MM-yyyy");
            sheet.Cells[rowNr, startColNr + 1].Value = transfer.Type;
            sheet.Cells[rowNr, startColNr + 2].Value = transfer.Amount;
            sheet.Cells[rowNr, startColNr + 3].Value = transfer.Note;
        }
    }
}
