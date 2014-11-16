using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using CashlessRegisterSystemCore.Helpers;

namespace CashlessRegisterSystemCore.Model
{
    public class MemberList : NotifyList
    {
        //public const string MEMBER_LIST_PATH = "members.txt";

        public Dictionary<String, Member> FromKey { get; private set; }
        public SortedList<String, Member> All { get; private set; }
        public Dictionary<String, Member> FromName { get; private set; }
        public List<Member> AllList { get; private set; }
        private readonly bool readInvisible;
        private readonly string path;

        public MemberList(bool includeInvisible, string path, bool watch = true)
        {
            readInvisible = includeInvisible;
            this.path = path;
            Init(ReadMemberLines(path));
            if (watch)
            {
                var memberWatcher = new FileSystemWatcher(path, Settings.MembersFile);
                memberWatcher.Changed += OnChanged;
                memberWatcher.Created += OnChanged;
                memberWatcher.EnableRaisingEvents = true;
            }
        }

        //public static MemberList LoadAndWatchFromFile(string path, bool includeInvisible)
        //{
        //    var list = new MemberList(includeInvisible, path);
        //    list.Init(ReadMemberLines(path));

        //    return list;
        //}

        public static List<string> ReadMemberLines(string path)
        {
            var result = new List<string>();
            result.AddRange(File.ReadAllLines(Path.Combine(path, Settings.MembersFile), Encoding.UTF8));
            result.RemoveRange(0, 4);
            return result;
        }

        public void Init(List<string> memberLines)
        {
            //TODO: only update member ref. (only possible if no members are removed!)
            All = new SortedList<String, Member>();
            FromKey = new Dictionary<string, Member>();
            FromName = new Dictionary<string, Member>();
            AllList = new List<Member>();
            foreach (string line in memberLines)
            {
                if(!readInvisible && line.StartsWith("#") || string.IsNullOrEmpty(line)) continue;
                var member = Member.Parse(line);
                if(readInvisible) member.Name = member.Name.Replace("#", string.Empty);
                if (member == null) continue;
                All.Add(member.Name, member);
                FromKey[member.KeyCode] = member;
                FromName[member.Name] = member;
                AllList.Add(member);
            }
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            messageNotice(new MessageEventArgs { Type = MessageType.Service, Message = "Momenteel wordt het ledenbestand bijgewerkt, mocht dit scherm onafgebroken voor langer dan 1 minuut op het scherm staan neem dan z.s.m. contact op met Trui en Benjamin." });
            bool ok = false;
            while (!ok)
            {
                try
                {
                    Init(ReadMemberLines(path));

                    ok = true;
                }
                catch
                {
                    Thread.Sleep(100);
                }
            }
            dataChange(source, e);
        }


        public void TryAddTransaction(Transaction transaction)
        {
            Member findMember;
            if (FromName.TryGetValue(transaction.MemberName, out findMember))
            {
                findMember.AddTransaction(transaction);
            }
        }

        public void ClearAndAddTransactions(List<Transaction> transactions)
        {
            foreach (var member in All.Values)
            {
                member.Transactions = new List<Transaction>();
                member.TransactionAmountInCents = 0;
            }            
            foreach (var transaction in transactions)
            {
                TryAddTransaction(transaction);
            }
        }
 
        public void ClearAndAddTransfers(List<Transfer> transfers)
        {
            foreach (var member in All.Values)
            {
                member.Transfers = new List<Transfer>();
                member.TransferAmountInCents = 0;
            }            
            foreach (var transfer in transfers)
            {
                Member findMember;
                if (FromName.TryGetValue(transfer.MemberName, out findMember))
                {
                    findMember.AddTransfer(transfer);
                }
            }
        }

        public string CreateCsv()
        {
            var result = new StringBuilder();
            result.AppendLine("#MemberName[string];KeyCode[(hex)string];PaymentMethod[enum:Incasso|PrePaid];IncassoLevel[int in cents];AccountNumber[string];AccountName[string];BalanceDate[date:yyyy-MM-dd];Balance[int in cents];");
            result.AppendLine("#Wikkert;A1B2C3;Incasso;50000;123456;W. Olbodo;2014-01-13;1250;");
            result.AppendLine("#This means: Wikkert has key A1B2C3; pays with Incasso (level to € 50.00 positive); has the bank account 123456 with the name W. Olbodo; last time (2013-01-14) he had a balace of ? 12.50;;;;;");
            result.AppendLine("#Wally bier;??????;PrePaid;0;0;0;2012-12-01;0;");
            foreach (var member in AllList)
            {
                result.AppendLine(member.CreateCsv());
            }
            return result.ToString();
        }
    }
}
