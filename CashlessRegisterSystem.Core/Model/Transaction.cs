using System;
using System.Diagnostics;
using System.Globalization;

namespace CashlessRegisterSystemCore.Model
{
    [DebuggerDisplay("Transaction : {TransactionDate} {Name} {KeyCode} {AmountInCents} {Note}")]
    public class Transaction
    {
        // The private setters should not be changed, they force the creation of a transaction to go trough
        // either the parse (=read in of old transactions) or new method, with the last method the transaction
        // is always logged!
        public DateTime TransactionDate { get; internal set; }
        public int AmountInCents { get; internal set; } // in (euro)cents!
        public string MemberName { get; internal set; }
        public string KeyCode { get; private set; }
        public string Note { get; internal set; }

        public string ToLogLine()
        {
            return String.Format("{0:yyyy'-'MM'-'dd';'HH':'mm':'ss};{1};{2};{3};{4}\r\n", TransactionDate, MemberName, KeyCode, AmountInCents, Note);
        }

        override public string ToString()
        {
            return String.Format("{0} € {1:0.00} @{2:HH':'mm':'ss' 'dd'-'MM'-'yyyy}", MemberName, AmountInCents / 100.0, TransactionDate);
        }

        public static Transaction Parse(string line)
        {
            string[] values = line.Split(';');
            if (values.Length >= 5)
            {
                try
                {
                    DateTime date = DateTime.ParseExact(values[0], "yyyy'-'MM'-'dd", DateTimeFormatInfo.InvariantInfo);
                    //NOTE: the hh (lowercase is NOT a mistake, only hh is supported, HH gives Exceptions)
                    //see http://msdn.microsoft.com/en-us/library/dd783872.aspx
                    TimeSpan time = TimeSpan.ParseExact(values[1], "hh':'mm':'ss", DateTimeFormatInfo.InvariantInfo);
                    var transaction =
                        new Transaction
                        {
                            TransactionDate = date.Add(time),
                            MemberName = values[2],
                            KeyCode = values[3],
                            AmountInCents = int.Parse(values[4]),
                            Note = values.Length == 6 ? values[5] : values.Length > 5 ? "" : String.Join(";", values, 5, values.Length - 5)
                        };
                    return transaction;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            return null;
        }
    }
}
