using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using CashlessRegisterSystemCore.Helpers;

namespace CashlessRegisterSystemCore.Model
{
    [DebuggerDisplay("Member : {Name} {KeyCode} {Payment} ({IncassoLevel}) {AccountNumber} {AccountName} {BalanceDate} {StartBalanceAmountInCents}")]
    public class Member
    {
        /**
         * Following fields are from the txt
         */
        public string Name { get; set; }
        public string KeyCode { get; set; }
        public enum PaymentMethod { INCASSO, PREPAID };
        public PaymentMethod Payment { get; set; }
        public int IncassoLevel { get; set; }
        public bool Visible { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public DateTime BalanceDate { get; set; }
        public int StartBalanceAmountInCents { get; set; }

        //Next are 'dynamic'
        public int TransactionAmountInCents { get; set; }
        public int TransferAmountInCents { get; set; }
        public int CurrentBalanceAmountInCents { get {return StartBalanceAmountInCents - TransactionAmountInCents + TransferAmountInCents;} }
        public List<Transaction> Transactions { get; set; }
        public List<Transfer> Transfers { get; set; }

        public void AddTransfer(Transfer transfer)
        {
            Transfers.Add(transfer);
            if (BalanceDate <= transfer.Date)
            {
                TransferAmountInCents += (int)transfer.Amount * 100;
            }
        }

        public void AddTransaction(Transaction transaction)
        {
            Transactions.Add(transaction);
            if (BalanceDate <= transaction.TransactionDate)
            {
                TransactionAmountInCents += transaction.AmountInCents;
            }
        }
        
        public static Member Parse(string line)
        {
            string[] values = line.Split(';');
            if (values.Length >= 8)
            {
                try
                {
                    return
                        new Member
                        {
                            Name = values[0],
                            KeyCode = values[1],
                            Payment = (PaymentMethod)Enum.Parse(typeof(PaymentMethod), values[2], true),
                            IncassoLevel = int.Parse(values[3]),
                            AccountNumber = values[4],
                            AccountName = values[5],
                            BalanceDate = DateTime.ParseExact(values[6], "yyyy'-'MM'-'dd", DateTimeFormatInfo.InvariantInfo),
                            StartBalanceAmountInCents = int.Parse(values[7])
                        };
                }
                catch (Exception e)
                {
                    Logger.Error(e, line);
                }
            }
            return null;
        }

        public string CreateCsv()
        {
            var result = new StringBuilder();
            result.Append(Name + ";");
            result.Append(KeyCode + ";");
            result.Append(Payment + ";");
            result.Append(IncassoLevel + ";");
            result.Append(AccountNumber + ";");
            result.Append(AccountName + ";");
            result.Append(BalanceDate.ToString("yyyy-MM-dd") + ";");
            result.Append(StartBalanceAmountInCents + ";");
            return result.ToString();
        }
    }
}
