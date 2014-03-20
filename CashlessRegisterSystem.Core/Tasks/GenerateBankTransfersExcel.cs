using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CashlessRegisterSystemCore.Helpers;
using CashlessRegisterSystemCore.Model;
using OfficeOpenXml;

namespace CashlessRegisterSystemCore.Tasks
{
    public static class GenerateBankTransfersExcel
    {
        public static void Generate(string fileName, List<BankTransfer> memberTransfers, List<BankTransfer> filteredTransfers)
        {
            if (memberTransfers.Count == 0) return;
            var newFile = new FileInfo(fileName);
            if (newFile.Exists)
            {
                newFile.Delete();  // ensures we create a new workbook
                newFile = new FileInfo(fileName);
            }
            using (var package = new ExcelPackage(newFile))
            {
                var sortedMemberTransfers = memberTransfers.OrderByDescending(x => x.PaymentDate).ToList();
                var sortedFilteredTransfers = filteredTransfers.OrderByDescending(x => x.PaymentDate).ToList();

                string titleWorkbook = "BankTransacties Wolbodo " + DateTime.Now.Date.ToString("yyyy-MM-dd");
                ExcelHelper.InitWorkbook(package.Workbook, titleWorkbook);
                
                GenerateWorksheet("Valide", memberTransfers, package, sortedMemberTransfers);
                GenerateWorksheet("Gefilterd", memberTransfers, package, sortedFilteredTransfers);
                package.Save();
            }
        }

        private static void GenerateWorksheet(string name, List<BankTransfer> memberTransfers, ExcelPackage package, List<BankTransfer> sortedTransfers)
        {
            var worksheet = package.Workbook.Worksheets.Add(name);
            ExcelHelper.InitWorkSheet(worksheet);
            string titleSheet = "BankTransacties Wolbodo t/m" + memberTransfers[0].PaymentDate;
            string[] header = new[] {"Datum", "BankType", "TransferType", "Lid", "Naam", "Rekening", "Bedrag", "Melding"};
            ExcelHelper.AddTitle(worksheet, titleSheet);
            ExcelHelper.AddHeader(worksheet, header, 2);
            int nrRows = 2;
            foreach (var transfer in sortedTransfers)
            {
                nrRows++;
                AddRow(worksheet, nrRows, transfer);
            }
            worksheet.Cells[3, 6, nrRows, 6].Style.Numberformat.Format = ExcelHelper.EuroFormat;
            worksheet.Cells[3, 1, nrRows, 1].Style.Numberformat.Format = "yyyy-MM-dd";
            worksheet.Cells[1, 1, nrRows, 10].AutoFitColumns();
            worksheet.PrinterSettings.FitToPage = true;
            worksheet.PrinterSettings.Scale = 67;
        }

        private static void AddRow(ExcelWorksheet sheet, int rowNr, BankTransfer transfer)
        {
            sheet.Cells[rowNr, 1].Value = transfer.PaymentDate;
            sheet.Cells[rowNr, 2].Value = transfer.TypeEnum;
            sheet.Cells[rowNr, 3].Value = TransferTypeEnum.Bank;
            if(transfer.AssociatedMember != null) sheet.Cells[rowNr, 4].Value = transfer.AssociatedMember.Name;
            sheet.Cells[rowNr, 5].Value = transfer.AccountName;
            sheet.Cells[rowNr, 6].Value = transfer.AccountNumber;
            sheet.Cells[rowNr, 7].Value = transfer.Amount;
            sheet.Cells[rowNr, 8].Value = transfer.Note;
        }
    }
}
