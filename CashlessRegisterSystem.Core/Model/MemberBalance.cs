using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace CashlessRegisterSystemCore.Model
{
    [DebuggerDisplay("MemberBalance : {Member.Name} {Year}-{Month} {StartBalance} {Coasters} {Incasso} {Cash} {Deposit} {Receipt} {Contribution} {Activity} {EndBalance}")]
    public class MemberBalance
    {
        public MemberBalance()
        {
            Transactions = new List<Transaction>();
            Transfers = new List<Transfer>();
        }

        public Member Member { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public List<Transaction> Transactions { get; private set; }
        public List<Transfer> Transfers { get; private set; } 

        public DateTime Date
        {
            get
            {
                return new DateTime(Year, Month, 1);
            }
        }

        public decimal StartBalance { get; set; }
        public decimal Coasters { get; set; } // :-;
        public decimal Incasso { get; set; }
        public decimal Cash { get; set; }
        public decimal Deposit { get; set; }
        public decimal Receipt { get; set; }
        public decimal Contribution { get; set; }
        public decimal Activity { get; set; }

        public decimal EndBalance
        {
            get { return StartBalance + Coasters + Incasso + Cash + Deposit + Receipt + Contribution + Activity; }
        }

        public static MemberBalance Parse(int month, int year, string line)
        {
            string[] values = line.Split(';');
            if (values.Length < 9) throw new Exception("MemberBalance cannot be parsed from line: " + line);
            var balance = new MemberBalance {Month = month, Year = year};
            balance.StartBalance = decimal.Parse(values[1], NumberStyles.Currency);
            balance.Coasters = decimal.Parse(values[2], NumberStyles.Currency);
            balance.Incasso = decimal.Parse(values[3], NumberStyles.Currency);
            balance.Cash = decimal.Parse(values[4], NumberStyles.Currency);
            balance.Deposit = decimal.Parse(values[5], NumberStyles.Currency);
            balance.Receipt = decimal.Parse(values[6], NumberStyles.Currency);
            balance.Contribution = decimal.Parse(values[7], NumberStyles.Currency);
            balance.Activity = decimal.Parse(values[8], NumberStyles.Currency);
            return balance;
        }

        public void ProcessTransaction(Transaction transaction)
        {
            Transactions.Add(transaction);
            Coasters -= (decimal) transaction.AmountInCents/100;
        }

        public void ProcessTransfer(Transfer transfer)
        {
            Transfers.Add(transfer);
            switch (transfer.Type)
            {
                case TransferTypeEnum.Bank:
                    Deposit += transfer.Amount;
                    break;
                case TransferTypeEnum.Incasso:
                    Incasso += transfer.Amount;
                    break;
                case TransferTypeEnum.Cash:
                    Cash += transfer.Amount;
                    break;
                case TransferTypeEnum.Bonnetje:
                    Receipt += transfer.Amount;
                    break;
                case TransferTypeEnum.Contributie:
                    Contribution -= transfer.Amount;
                    break;
                case TransferTypeEnum.Activiteit:
                case TransferTypeEnum.Activiteiten:
                    Activity -= transfer.Amount;
                    break;
                default: throw new Exception("Unknown transfer type: " + transfer.Type);
            }
        }

        public void Adjust(MemberBalance lastMemberBalance)
        {
            StartBalance = lastMemberBalance.EndBalance;
        }

        public void Print()
        {
            Console.WriteLine(Member.Name.PadRight(20) + StartBalance.ToString("N2").PadLeft(10) + Coasters.ToString("N2").PadLeft(10) + Incasso.ToString("N2").PadLeft(10)
                + Cash.ToString("N2").PadLeft(10) + Deposit.ToString("N2").PadLeft(10) + Receipt.ToString("N2").PadLeft(10)
                + Contribution.ToString("N2").PadLeft(10)+ Activity.ToString("N2").PadLeft(10)+ EndBalance.ToString("N2").PadLeft(10));
        }
    }
}
