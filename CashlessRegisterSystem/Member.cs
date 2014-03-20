using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading;

namespace ViltjesSysteem
{
    [DebuggerDisplay("Member : {MemberName} {KeyCode} {Payment} ({IncassoLevel}) {AccountNumber} {AccountName} {BalanceDate} {StartBalanceAmountInCents}")]
    public class Member
    {
        private const string MEMBER_LIST_PATH = "members.txt";
        private const string ERRORS_PATH = "'errors-'yyyy'-'MM'.txt'";

        public static Dictionary<String, Member> FromKey { get; private set; }
        public static SortedList<String, Member> All { get; private set; }
        public static Dictionary<String, Member> FromName { get; private set; }

        /**
         * Following fields are from the txt
         */
        public string MemberName { get; set; }
        public string KeyCode { get; set; }
        public enum PaymentMethod { INCASSO, PREPAID };
        public PaymentMethod Payment { get; set; }
        public int IncassoLevel { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public DateTime BalanceDate { get; set; }
        public int StartBalanceAmountInCents { get; set; }

        //Next are 'dynamic'
        private int TransactionAmountInCents { get; set; }
        private int TransferAmountInCents { get; set; }
        public int CurrentBalanceAmountInCents { get {return StartBalanceAmountInCents - TransactionAmountInCents + TransferAmountInCents;} }
        public List<Transaction> Transactions { get; set; }
        public List<Transfer> Transfers { get; set; }

        static Member()
        {
            LoadMembers();
            FileSystemWatcher memberWatcher = new FileSystemWatcher(Environment.CurrentDirectory, MEMBER_LIST_PATH);
            memberWatcher.Changed += new FileSystemEventHandler(OnChanged);
            memberWatcher.Created += new FileSystemEventHandler(OnChanged);
            memberWatcher.EnableRaisingEvents = true;
        }

        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            GUI.messageNotice(new MessageEventArgs { Type = MessageType.Service, Message = "Momenteel wordt het ledenbestand bijgewerkt, mocht dit scherm onafgebroken voor langer dan 1 minuut op het scherm staan neem dan z.s.m. contact op met Trui en Benjamin." });
            bool ok = false;
            while (!ok)
            {
                try
                {
                    LoadMembers();
                    ClearAndAddTransactions(Transaction.All);
                    ClearAndAddTransfers(Transfer.All);
                    ok = true;
                }
                catch
                {
                    Thread.Sleep(100);
                }
            }
            GUI.dataChange(source, e);
        }

        public static Member Parse(string line)
        {
            if (line.StartsWith("#"))
                return null;
            string[] values = line.Split(';');
            if (values.Length >= 8)
            {
                try
                {
                    return
                        new Member
                        {
                            MemberName = values[0],
                            KeyCode = values[1],
                            Payment = (PaymentMethod)Enum.Parse(typeof(PaymentMethod), values[2], true),
                            IncassoLevel = int.Parse(values[3]),
                            AccountNumber = values[4],
                            AccountName = values[5],
                            BalanceDate = DateTime.ParseExact(values[6], "yyyy'-'MM'-'dd", DateTimeFormatInfo.InvariantInfo),
                            StartBalanceAmountInCents = int.Parse(values[7])
                        };
                }
                catch (Exception)
                {
                    File.AppendAllText(DateTime.Now.ToString(ERRORS_PATH), string.Format("{0:yyyy'-'MM'-'dd';'HH':'mm':'ss} Read error: {1}\r\n", DateTime.Now, line));
                }
            }
            return null;
        }

        private static void LoadMembers()
        {
            string[] lines = File.ReadAllLines(MEMBER_LIST_PATH, ASCIIEncoding.UTF8);
            
            //TODO: only update member ref. (only possible if no members are removed!)
            All = new SortedList<String, Member>();
            FromKey = new Dictionary<string, Member>();
            FromName = new Dictionary<string, Member>();

            foreach (string line in lines)
            {
                Member member = Member.Parse(line);
                if (member != null)
                {
                    All.Add(member.MemberName, member);
                    FromKey[member.KeyCode] = member;
                    FromName[member.MemberName] = member;
                }
            }
        }

        public static void ClearAndAddTransactions(List<Transaction> transactions)
        {
            foreach (Member member in All.Values)
            {
                member.Transactions = new List<Transaction>();
                member.TransactionAmountInCents = 0;
            }            
            foreach (Transaction transaction in transactions)
            {
                TryAddTransaction(transaction);
            }
        }

        public static void TryAddTransaction(Transaction transaction)
        {
            Member findMember;
            if (FromName.TryGetValue(transaction.MemberName, out findMember))
            {
                findMember.AddTransaction(transaction);
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

        public static void ClearAndAddTransfers(List<Transfer> transfers)
        {
            foreach (Member member in All.Values)
            {
                member.Transfers = new List<Transfer>();
                member.TransferAmountInCents = 0;
            }            
            foreach (Transfer transfer in transfers)
            {
                TryAddTransfer(transfer);
            }
        }

        public static void TryAddTransfer(Transfer transfer)
        {
            Member findMember;
            if (FromName.TryGetValue(transfer.MemberName, out findMember))
            {
                findMember.AddTransfer(transfer);
            }
        }
        
        public void AddTransfer(Transfer transfer)
        {
            Transfers.Add(transfer);
            if (BalanceDate <= transfer.PaymentDate)
            {
                TransferAmountInCents += transfer.AmountInCents;
            }
        }
    }
}
