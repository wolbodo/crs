using System;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace CashlessRegisterSystemCore.Helpers
{
    public static class ExcelHelper
    {
        public const string EuroFormat = @"_ €\ * #,##0.00_ ;_ €\ * \-#,##0.00_ ;_ €\ * -??_ ;_ @_ ";

        public static void InitWorkbook(ExcelWorkbook workBook, string title)
        {
            workBook.Properties.Title = title;
            workBook.Properties.Author = "ViltjesSysteem";
            workBook.Properties.Company = "Wolbodo Inc.";
        }

        public static void InitWorkSheet(ExcelWorksheet sheet)
        {
            sheet.HeaderFooter.OddFooter.CenteredText = ExcelHeaderFooter.SheetName;
            sheet.HeaderFooter.OddFooter.LeftAlignedText = "Gegenereerd: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static void AddTitle(ExcelWorksheet sheet, string title)
        {            
            // title
            var cellTitle = sheet.Cells["A1"];
            cellTitle.Style.Font.Bold = true;
            cellTitle.Style.Font.Size = 14;
            cellTitle.Value = title;
            sheet.Cells["A1:F1"].Merge = true;
        }

        public static void AddHeader(ExcelWorksheet sheet, string[] header, int rowNum)
        {
            // table header
            using (var range = sheet.Cells[rowNum, 1, rowNum, header.Length])
            {
                range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                range.Style.Font.Bold = true;
            }
            for (int i = 1; i < header.Length + 1; i++)
            {
                sheet.Column(i).Width = 10;
                sheet.Cells[rowNum, i].Value = header[i - 1];
            }
            //sheet.Column(0).Width = 20;
        }
    }
}
