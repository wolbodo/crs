using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using System.Collections.ObjectModel;

namespace ViltjesSysteem
{
    [DebuggerDisplay("Transaction : {TransactionDate} {MemberName} {KeyCode} {AmountInCents} {Note}")]
    public class Transaction
    {
        private const string TRANSACTION_LIST_PATH = "'transactions-'yyyy'-'MM'.txt'";
        private const string NETWORK_PATH_PREFIX = @"\\tommie\music_upload\viltjessysteem\";
        // The private setters should not be changed, they force the creation of a transaction to go trough
        // either the parse (=read in of old transactions) or new method, with the last method the transaction
        // is always logged!
        public DateTime TransactionDate { get; private set; }
        public int AmountInCents { get; private set; } // in (euro)cents!
        public string MemberName { get; private set; }
        public string KeyCode { get; private set; }
        public string Note { get; private set; }
        public static List<Transaction> All { get; private set; }
        public static Dictionary<DateTime, Transaction> Corrected { get; private set; }
        private static long ticksPerSecond = TimeSpan.FromSeconds(1).Ticks;
        
        static Transaction()
        {
            LoadTransactions();
        }

        public static Transaction New(int amount, string memberName)
        {
            return New(amount, memberName, "");
        }

        public static Transaction New(int amount, string memberName, string note)
        {
            return New(amount, memberName, "", note);
        }

        public static Transaction NewWithKey(int amount, string key)
        {
            return NewWithKey(amount, key, "");
        }

        public static Transaction NewWithKey(int amount, string key, string note)
        {
            Member member;
            if (key.Length < 6 || !Member.FromKey.TryGetValue(key, out member))
            {
                throw new Exception("No member found for key");
            }
            return New(amount, member.MemberName, key, note);
        }

        public static Transaction Cancel(Transaction transaction)
        {
            return New(-transaction.AmountInCents, transaction.MemberName, string.Format("CR{0:yyyy'-'MM'-'dd'T'HH':'mm':'ss}", transaction.TransactionDate));
        }

        public static Transaction New(int amount, string memberName, string key, string note)
        {
            DateTime transactionTime = new DateTime(((DateTime.Now.Ticks + ticksPerSecond - 1) / ticksPerSecond) * ticksPerSecond); 
            
            String transactionFile = transactionTime.ToString(TRANSACTION_LIST_PATH);
            Transaction transaction = new Transaction { TransactionDate = transactionTime, AmountInCents = amount, MemberName = memberName, Note = note };

            //try network save, but decrade without errors (but there should be a warning on the display!)
            //FIXME: part about the warning
            bool localWrite = false, networkWrite = false;
            try
            {
                File.AppendAllText(transactionFile, transaction.ToLogLine(), ASCIIEncoding.UTF8);
                localWrite = true;
            }
            catch { }
            try
            {
                File.AppendAllText(Path.Combine(NETWORK_PATH_PREFIX, transactionFile), transaction.ToLogLine(), ASCIIEncoding.UTF8);
                networkWrite = true;
            }
            catch { }

            if (localWrite || networkWrite)
            {
                Member.TryAddTransaction(transaction);
                All.Add(transaction);
                if (transaction.AmountInCents < 0)
                {
                    TryParseCorrected(transaction);
                }
                if (!localWrite)
                {
                    GUI.messageNotice(new MessageEventArgs { Type = MessageType.Warning, Message = string.Format("Kon de transactie ({0}) niet lokaal opslaan maar de transactie is wel op het netwerk opgeslagen.\r\n\r\nBreng z.s.m. Trui en Benjamin op de hoogte om naar de viltjeslaptop te kijken (waarschijnlijk een harddisk probleem)", transaction) });
                }
                if (!networkWrite)
                {
                    GUI.messageNotice(new MessageEventArgs { Type = MessageType.Warning, Message = string.Format("Kon de transactie ({0}) niet op het netwerk opslaan maar de transactie is wel lokaal opgeslagen.\r\n\r\nBreng z.s.m. Trui en Benjamin op de hoogte om naar het netwerk te kijken (netwerkprobleem of schrijfprobleem op Tommie).", transaction) });
                }
            }
            else
            {
                GUI.messageNotice(new MessageEventArgs { Type = MessageType.FatalError, Message = string.Format("Kon de transactie ({0}) niet lokaal noch op het netwerk opslaan, ofwel je transactie is NIET genoteerd.\r\n\r\nGebruik de baragenda om alsnog je transactie op te schrijven!\r\n\r\nBreng tevens z.s.m. Trui en Benjamin op de hoogte dat het volledige systeem plat ligt!", transaction) });
            }

            return transaction;
        }
        
        public string ToLogLine()
        {
            return String.Format("{0:yyyy'-'MM'-'dd';'HH':'mm':'ss};{1};{2};{3};{4}\r\n", TransactionDate, MemberName, KeyCode, AmountInCents, Note);
        }

        override public string ToString()
        {
            return String.Format("{0} € {1:0.00} @{2:HH':'mm':'ss' 'dd'-'MM'-'yyyy}", MemberName, AmountInCents / 100.0, TransactionDate);
        }

        private static Transaction Parse(string line)
        {
            if (line.StartsWith("#"))
                return null;
            string[] values = line.Split(';');
            if (values.Length >= 5)
            {
                try
                {
                    DateTime date = DateTime.ParseExact(values[0], "yyyy'-'MM'-'dd", DateTimeFormatInfo.InvariantInfo);
                    //NOTE: the hh (lowercase is NOT a mistake, only hh is supported, HH gives Exceptions)
                    //see http://msdn.microsoft.com/en-us/library/dd783872.aspx
                    TimeSpan time = TimeSpan.ParseExact(values[1], "hh':'mm':'ss", DateTimeFormatInfo.InvariantInfo);
                    Transaction transaction =
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

        private static void TryParseCorrected(Transaction transaction)
        {
            try
            {
                DateTime corrected = DateTime.ParseExact(transaction.Note, "'CR'yyyy'-'MM'-'dd'T'HH':'mm':'ss", DateTimeFormatInfo.InvariantInfo);
                Corrected.Add(corrected, transaction);
            }
            catch { }
        }

        private static void LoadTransactions()
        {
            All = new List<Transaction>();
            Corrected = new Dictionary<DateTime, Transaction>();

            DateTime transactionTimeThisMonth = DateTime.Now;
            String[] transactionFiles = { transactionTimeThisMonth.AddMonths(-1).ToString(TRANSACTION_LIST_PATH), transactionTimeThisMonth.ToString(TRANSACTION_LIST_PATH)};

            foreach (String transactionFile in transactionFiles)
            {
                if (!File.Exists(transactionFile))
                    continue;
                string[] lines = File.ReadAllLines(transactionFile, ASCIIEncoding.UTF8);

                foreach (string line in lines)
                {
                    Transaction transaction = Transaction.Parse(line);
                    if (transaction != null)
                    {
                        All.Add(transaction);
                        if (transaction.AmountInCents < 0)
                        {
                            TryParseCorrected(transaction);
                        }
                    }
                }
            }

            Member.ClearAndAddTransactions(All);
        }
    }
}
