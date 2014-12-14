using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CashlessRegisterSystemCore.Helpers;
using CashlessRegisterSystemCore.Model;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace CashlessRegisterSystemCore.Tasks
{
    public static class GenerateMonthTransactionsExcel
    {
        static DateTimeFormatInfo dateTimeInfo = DateTimeFormatInfo.GetInstance(CultureInfo.GetCultureInfo("nl-NL"));

        public static void Execute()
        {
             var files = new List<string>();
            files.AddRange(Directory.GetFiles(Settings.LocalTransactionsPath, "transactions-*.txt"));
            foreach (var sourceFile in files)
            {
                ExecuteSingle(sourceFile);
            }
        }

        private static void ExecuteSingle(string transactionFile)
        {
            var info = new FileInfo(transactionFile);
            int monthFile = TransactionFileHelper.GetMonth(info.Name);
            int yearFile = TransactionFileHelper.GetYear(info.Name);
            var name = "Streeplijst Wolbodo " + yearFile + "-" + monthFile.ToString("00");
            var overviewFile = Path.Combine(Settings.LocalTransactionsPath, name + ".xlsx");
            if (File.Exists(overviewFile) || (DateTime.Now.Month == monthFile && DateTime.Now.Year == yearFile)) return;
            string[] lines = File.ReadAllLines(transactionFile);
            var transactionList = new TransactionList();
            transactionList.Init(lines.ToList());
            // create member list based on transactions
            var monthMembers = new List<Member>();
            var monthBalance = new MonthBalance { Month = monthFile, Year = yearFile, Transactions = transactionList.All };
            foreach (var transaction in transactionList.All)
            {
                Transaction transaction1 = transaction;
                // get member
                var member = monthMembers.SingleOrDefault(x => x.Name == transaction1.MemberName);
                if (member == null)
                {
                    member = new Member {Name = transaction1.MemberName};
                    monthMembers.Add(member);
                }
                // get balance
                var memberBalance = monthBalance.MemberBalances.SingleOrDefault(x => x.Member.Name.ToLower() == transaction.MemberName.ToLower());
                if (memberBalance == null)
                {
                    memberBalance = new MemberBalance { Month = monthFile, Year = yearFile, Member = member };
                    monthBalance.AddBalance(memberBalance);
                }
                // add transaction
                memberBalance.ProcessTransaction(transaction);
            }
            Generate(overviewFile, monthBalance);
        }

        public static void Generate(string fileName, MonthBalance monthBalance)
        {
            var newFile = new FileInfo(fileName);
            if (newFile.Exists)
            {
                newFile.Delete();
                newFile = new FileInfo(fileName);
            }
            using (var package = new ExcelPackage(newFile))
            {
                string monthName = dateTimeInfo.MonthNames[monthBalance.Month - 1] + " " + monthBalance.Year;
                string titleWorkbook = "Streeplijst Wolbodo " + monthName + " " + monthBalance.Year;
                ExcelHelper.InitWorkbook(package.Workbook, titleWorkbook);
                var header = new[] {"Naam", "Totaal"};
                string titleSheet = "Streeplijst " + monthName;
                var worksheet = package.Workbook.Worksheets.Add("Totaal " + monthName);

                ExcelHelper.InitWorkSheet(worksheet);
                ExcelHelper.AddTitle(worksheet, titleSheet);
                ExcelHelper.AddHeader(worksheet, header, 2);
                int nrRows = 2;
                
                foreach (var memberBalance in monthBalance.MemberBalances)
                {
                    nrRows++;
                    AddBalanceRow(worksheet, nrRows, memberBalance);
                    //GenerateMemberSheet(package.Workbook, memberBalance, monthName);
                }
                //if (monthBalance.MemberBalances.Count > 0) worksheet.Cells[3, 1, nrRows, header.Count].Style.Numberformat.Format = ExcelHelper.EuroFormat;
                GenerateTransactionSheet(package.Workbook, monthBalance, monthName);

                // sum
                int lastRow = nrRows;
                nrRows++;
                //worksheet.Column(0).Width = 200;
                //worksheet.Column(1).Width = 120;
                worksheet.Cells[nrRows, 1].Value = "Totaal";
                worksheet.Cells[nrRows, 2].Formula = "SUM(" + Nr2Letter(2) + 2 + ":" + Nr2Letter(2) + lastRow + ")";
                //worksheet.Cells[nrRows, 3].Formula = "SUM(" + Nr2Letter(3) + 2 + ":" + Nr2Letter(3) + lastRow + ")";
                //worksheet.Cells[nrRows, 4].Formula = "SUM(" + Nr2Letter(4) + 2 + ":" + Nr2Letter(4) + lastRow + ")";
                //worksheet.Cells[nrRows, 5].Formula = "SUM(" + Nr2Letter(5) + 2 + ":" + Nr2Letter(5) + lastRow + ")";
                //worksheet.Cells[nrRows, 6].Formula = "SUM(" + Nr2Letter(6) + 2 + ":" + Nr2Letter(6) + lastRow + ")";
                //worksheet.Cells[nrRows, 7].Formula = "SUM(" + Nr2Letter(7) + 2 + ":" + Nr2Letter(7) + lastRow + ")";
                //worksheet.Cells[nrRows, 8].Formula = "SUM(" + Nr2Letter(8) + 2 + ":" + Nr2Letter(8) + lastRow + ")";
                //worksheet.Cells[nrRows, 9].Formula = "SUM(" + Nr2Letter(9) + 2 + ":" + Nr2Letter(9) + lastRow + ")";
                //worksheet.Cells[nrRows, 10].Formula = "SUM(" + Nr2Letter(10) + 2 + ":" + Nr2Letter(10) + lastRow + ")";
                using (var range = worksheet.Cells[nrRows, 1, nrRows, 2])
                {
                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    range.Style.Font.Bold = true;
                }
                worksheet.Cells[3, 2, nrRows + 3, 2].Style.Numberformat.Format = ExcelHelper.EuroFormat;
                worksheet.Column(1).Width = 30;
                worksheet.Column(2).Width = 12;
                //worksheet.Cells[1, 1, nrRows + 1, header.Length].AutoFitColumns();
                //worksheet.PrinterSettings.FitToPage = true;
                //worksheet.PrinterSettings.Scale = 67;
                package.Save();
            }
        }

        internal static string Nr2Letter(int number)
        {
            bool isCaps = true;
            var c = (Char)((isCaps ? 65 : 97) + (number - 1));
            return c.ToString();
        }

        private static void GenerateTransactionSheet(ExcelWorkbook workbook, MonthBalance balance, string monthName)
        {
            var sheet = workbook.Worksheets.Add("Streeplijst " + monthName);
            string titleSheet = "Streeplijst " + monthName;
            string[] headerBalance = { "Datum", "Tijdstip", "Naam", "Bedrag" };
            ExcelHelper.InitWorkSheet(sheet);
            ExcelHelper.AddTitle(sheet, titleSheet);
            ExcelHelper.AddHeader(sheet, headerBalance, 2);
            sheet.Cells[3, 4, balance.Transactions.Count + 2, 4].Style.Numberformat.Format = ExcelHelper.EuroFormat;
            int transactionRowNr = 2;
            for (int i = 0; i < balance.Transactions.Count; i++)
            {
                transactionRowNr++;
                AddTransactionRow(sheet, transactionRowNr, balance.Transactions[i]);
            }
            sheet.Cells[2, 1, balance.Transactions.Count, 4].AutoFilter = true;
            //sheet.Cells[1, 1, transactionRowNr, 8].AutoFitColumns();
            //sheet.PrinterSettings.FitToPage = false;
            sheet.Column(1).Width = 12;
            sheet.Column(2).Width = 12;
            sheet.Column(3).Width = 30;
            sheet.Column(4).Width = 14;
        }

        //private static void GenerateMemberSheet(ExcelWorkbook workbook, MemberBalance memberBalance, string monthName)
        //{
        //    string memberName = memberBalance.Member.Name;
        //    var sheet = workbook.Worksheets.Add(memberName + " " + memberBalance.Year + "-" + memberBalance.Month);
        //    string titleSheet = "Schuldenlijst " + memberName + " " + monthName;
        //    string[] headerBalance = new[] { "Naam", "Saldo start", "Viltjes", "Incasso", "Bank", "Cash", "Bonnetjes", "Contributie", "Activiteiten", "Saldo eind" };
        //    ExcelHelper.InitWorkSheet(sheet);
        //    ExcelHelper.AddTitle(sheet, titleSheet);
        //    ExcelHelper.AddHeader(sheet, headerBalance, 2);
        //    AddBalanceRow(sheet, 2, memberBalance);
        //    sheet.Cells[2, 2, 2, 10].Style.Numberformat.Format = ExcelHelper.EuroFormat;
        //    // datum, tijd, bedrag .. datum, type, bedrag, melding
        //    string[] transactionsHeader = new[] { "Viltjes", "", "", "", "Transacties", "", "", "" };
        //    ExcelHelper.AddHeader(sheet, transactionsHeader, 4);
        //    int transactionRowNr = 4;
        //    for (int i = 0; i < memberBalance.Transactions.Count; i++)
        //    {
        //        transactionRowNr++;
        //        AddTransactionRow(sheet, transactionRowNr, memberBalance.Transactions[i]);
        //    }
        //    if (memberBalance.Transactions.Count > 0) sheet.Cells[4, 3, transactionRowNr + memberBalance.Transactions.Count, 3].Style.Numberformat.Format = ExcelHelper.EuroFormat;
        //    int transferRowNr = 4;
        //    for (int i = 0; i < memberBalance.Transfers.Count; i++)
        //    {
        //        transferRowNr++;
        //        AddTransferRow(sheet, transferRowNr, 5, memberBalance.Transfers[i]);
        //    }
        //    if (memberBalance.Transactions.Count > 0) sheet.Cells[4, 7, transactionRowNr + memberBalance.Transfers.Count, 7].Style.Numberformat.Format = ExcelHelper.EuroFormat;
        //    int nrRows = (transactionRowNr > transferRowNr) ? transactionRowNr : transferRowNr;
        //    sheet.Cells[1, 1, nrRows, 8].AutoFitColumns();
        //    sheet.PrinterSettings.FitToPage = true;
        //    sheet.PrinterSettings.Scale = 67;
        //}

        private static void AddBalanceRow(ExcelWorksheet sheet, int rowNr, MemberBalance memberBalance)
        {
            sheet.Cells[rowNr, 1].Value = memberBalance.Member.Name;
            //sheet.Cells[rowNr, 2].Value = memberBalance.StartBalance;
            //var colorStart = (memberBalance.StartBalance >= 0) ? Color.Black : Color.Red;
            //sheet.Cells[rowNr, 2].Style.Font.Color.SetColor(colorStart); 
            sheet.Cells[rowNr, 2].Value = memberBalance.Coasters;
            //sheet.Cells[rowNr, 4].Value = memberBalance.Incasso;
            //sheet.Cells[rowNr, 5].Value = memberBalance.Deposit;
            //sheet.Cells[rowNr, 6].Value = memberBalance.Cash;
            //sheet.Cells[rowNr, 7].Value = memberBalance.Receipt;
            //sheet.Cells[rowNr, 8].Value = memberBalance.Contribution;
            //sheet.Cells[rowNr, 9].Value = memberBalance.Activity;
            //sheet.Cells[rowNr, 10].Value = memberBalance.EndBalance;
            //var colorEnd = (memberBalance.EndBalance >= 0) ? Color.Black : Color.Red;
            //sheet.Cells[rowNr, 10].Style.Font.Color.SetColor(colorEnd); 
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
