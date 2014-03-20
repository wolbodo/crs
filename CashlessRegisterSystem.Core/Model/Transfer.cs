using System;
using System.Diagnostics;
using System.Globalization;

namespace CashlessRegisterSystemCore.Model
{
    public enum TransferTypeEnum
    {
        Incasso,
        Bank,
        Cash,
        Bonnetje,
        Activiteit,
        Contributie,
        Activiteiten
    }

    [DebuggerDisplay("Transfer : {MemberName} {Date} {Amount} {Type} {Note")]
    public class Transfer
    {
        public string MemberName { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public TransferTypeEnum Type { get; set; }
        public string Note { get; set; }
        public Member Member { get; set; }

        public static Transfer Parse(string line)
        {
            string[] values = line.Split(';');
            if (values.Length >= 4)
            {
                var transfer = new Transfer();
                transfer.Date = DateTime.ParseExact(values[0], "yyyy'-'MM'-'dd", DateTimeFormatInfo.InvariantInfo);
                transfer.MemberName = values[1];
                transfer.Amount = decimal.Parse(values[2], NumberStyles.Currency);
                transfer.Type = (TransferTypeEnum) Enum.Parse(typeof (TransferTypeEnum), values[3]);
                transfer.Note = values[4];
                return transfer;
            }
            return null;
        }


    }
}
