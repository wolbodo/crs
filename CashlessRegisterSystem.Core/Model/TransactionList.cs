using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using CashlessRegisterSystemCore.Helpers;

namespace CashlessRegisterSystemCore.Model
{
    [DebuggerDisplay("TransactionList : {Year}-{Month} {All.Count}")]
    public class TransactionList : NotifyList
    {
        private const string TRANSACTION_LIST_PATH = "'transactions-'yyyy'-'MM'.txt'";
        public int Month { get; set; }
        public int Year { get; set; }
        
        public DateTime Date
        {
            get
            {
                return new DateTime(Year, Month, 1);
            }
        }

        public List<Transaction> All { get; private set; }
        public Dictionary<DateTime, Transaction> Corrected { get; private set; }
        private long ticksPerSecond = TimeSpan.FromSeconds(1).Ticks;

        // only used by Gui - not admin or test
        public static TransactionList LoadFromFile()
        {
            var list = new TransactionList();
            list.Init(ReadTransactionsLastTwoMonths());
            return list;
        }

        public static List<string> ReadTransactionsLastTwoMonths()
        {
            var transactionLines = new List<string>();
            DateTime transactionTimeThisMonth = DateTime.Now;
            string[] transactionFiles = { transactionTimeThisMonth.AddMonths(-1).ToString(TRANSACTION_LIST_PATH), transactionTimeThisMonth.ToString(TRANSACTION_LIST_PATH) };

            foreach (string transactionFile in transactionFiles)
            {
                if (!File.Exists(transactionFile))
                    continue;
                transactionLines.AddRange(File.ReadAllLines(transactionFile, Encoding.UTF8));
            }
            return transactionLines;
        }

        public void Init(List<string> transactionLines)
        {
            All = new List<Transaction>();
            Corrected = new Dictionary<DateTime, Transaction>();

            foreach (string line in transactionLines)
            {
                if (line.StartsWith("#") || string.IsNullOrEmpty(line)) continue;
                var transaction = Transaction.Parse(line);
                if (transaction == null) continue;
                All.Add(transaction);
                if (transaction.AmountInCents < 0)
                {
                    TryParseCorrected(transaction);
                }
            }
        }

        private void TryParseCorrected(Transaction transaction)
        {
            try
            {
                DateTime corrected = DateTime.ParseExact(transaction.Note, "'CR'yyyy'-'MM'-'dd'T'HH':'mm':'ss", DateTimeFormatInfo.InvariantInfo);
                Corrected.Add(corrected, transaction);
            }
            catch { }
        }



        public Transaction New(int amount, string memberName, MemberList members)
        {
            return New(amount, memberName, "", members);
        }

        public Transaction New(int amount, string memberName, string note, MemberList members)
        {
            return New(amount, memberName, "", note, members);
        }

        //public static Transaction NewWithKey(int amount, string key)
        //{
        //    return NewWithKey(amount, key, "");
        //}

        //public static Transaction NewWithKey(int amount, string key, string note)
        //{
        //    Member member;
        //    if (key.Length < 6 || !MemberList.FromKey.TryGetValue(key, out member))
        //    {
        //        throw new Exception("No member found for key");
        //    }
        //    return New(amount, member.Name, key, note);
        //}

        public Transaction Cancel(Transaction transaction, MemberList members)
        {
            return New(-transaction.AmountInCents, transaction.MemberName, string.Format("CR{0:yyyy'-'MM'-'dd'T'HH':'mm':'ss}", transaction.TransactionDate), members);
        }

        public Transaction New(int amount, string memberName, string key, string note, MemberList memberList)
        {
            DateTime transactionTime = new DateTime(((DateTime.Now.Ticks + ticksPerSecond - 1) / ticksPerSecond) * ticksPerSecond);

            string transactionFile = transactionTime.ToString(TRANSACTION_LIST_PATH);
            var transaction = new Transaction { TransactionDate = transactionTime, AmountInCents = amount, MemberName = memberName, Note = note };

            //try network save, but degrade without errors (but there should be a warning on the display!)
            //FIXME: part about the warning
            bool localWrite = false, networkWrite = false;
            try
            {
                File.AppendAllText(transactionFile, transaction.ToLogLine(), Encoding.UTF8);
                localWrite = true;
            }
            catch { }
            try
            {
                File.AppendAllText(Path.Combine(FileHelper.NETWORK_PATH_PREFIX, transactionFile), transaction.ToLogLine(), Encoding.UTF8);
                networkWrite = true;
            }
            catch { }

            if (localWrite || networkWrite)
            {
                // leaky abstraction...
                memberList.TryAddTransaction(transaction);
                All.Add(transaction);
                if (transaction.AmountInCents < 0)
                {
                    TryParseCorrected(transaction);
                }
                if (!localWrite)
                {
                    messageNotice(new MessageEventArgs { Type = MessageType.Warning, Message = string.Format("Kon de transactie ({0}) niet lokaal opslaan maar de transactie is wel op het netwerk opgeslagen.\r\n\r\nBreng z.s.m. Trui en Benjamin op de hoogte om naar de viltjeslaptop te kijken (waarschijnlijk een harddisk probleem)", transaction) });
                }
                if (!networkWrite)
                {
                    messageNotice(new MessageEventArgs { Type = MessageType.Warning, Message = string.Format("Kon de transactie ({0}) niet op het netwerk opslaan maar de transactie is wel lokaal opgeslagen.\r\n\r\nBreng z.s.m. Trui en Benjamin op de hoogte om naar het netwerk te kijken (netwerkprobleem of schrijfprobleem op Tommie).", transaction) });
                }
            }
            else
            {
                messageNotice(new MessageEventArgs { Type = MessageType.FatalError, Message = string.Format("Kon de transactie ({0}) niet lokaal noch op het netwerk opslaan, ofwel je transactie is NIET genoteerd.\r\n\r\nGebruik de baragenda om alsnog je transactie op te schrijven!\r\n\r\nBreng tevens z.s.m. Trui en Benjamin op de hoogte dat het volledige systeem plat ligt!", transaction) });
            }

            return transaction;
        }
    }
}
