using System;
using System.Diagnostics;
using System.Globalization;

namespace CashlessRegisterSystemCore.Model
{
    public enum BankTransferTypeEnum
    {
        Incasso,
        Overschrijving,
        Internetbankieren
    }

    [DebuggerDisplay("BankTransfer : {PaymentDate} {AccountName} {AccountNumber} {Amount} {TypeEnum} {Note}")]
    public class BankTransfer
    {
        public Member AssociatedMember { get; set; }
        public DateTime PaymentDate { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public decimal Amount { get; set; }
        public BankTransferTypeEnum TypeEnum { get; set; }
        public string Note { get; set; }

        public static BankTransfer Parse(string line)
        {
            string[] values = line.Split(',');
            if (values.Length < 8) throw new Exception("Invalid nr of elements for line: " + line);
            if (values[5] != "Bij") return null; // only add debet mutations
            var transfer = new BankTransfer();
            transfer.PaymentDate = DateTime.Parse(values[0]);
            transfer.AccountName = values[1];
            transfer.AccountNumber = values[3];
            transfer.Amount = decimal.Parse(values[6] + "," + values[7], NumberStyles.Currency);
            string type = values[8];
            switch (type)
            {
                case "Incasso":
                case "Overschrijving":
                case "Internetbankieren":
                    transfer.TypeEnum = (BankTransferTypeEnum) Enum.Parse(typeof(BankTransferTypeEnum), type);
                    break;  
                default:
                    // filter undesired transfers
                    return null;
            }
            transfer.Note = values[9];
            return transfer;
        }
    }
}
