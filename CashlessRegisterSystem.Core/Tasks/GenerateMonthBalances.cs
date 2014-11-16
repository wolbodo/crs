using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CashlessRegisterSystemCore.Helpers;
using CashlessRegisterSystemCore.Model;

namespace CashlessRegisterSystemCore.Tasks
{
    public static class GenerateMonthBalances
    {
        public static MonthBalance Generate(int month, int year, List<Member> members, string filesDir, string lastMonthBalance)
        {
            var transactions = ReadTransactionList(filesDir, month, year);
            var transfers = ReadTranserList(filesDir, month, year);
            var monthBalance = InitMonthBalance(year, month, members);
            var previousMonth = new DateTime(year, month, 1).AddMonths(-1);
            var previousBalance = GetMonthBalance(previousMonth.Month, previousMonth.Year, lastMonthBalance, members);
            if (previousBalance != null)
            {
                previousBalance.Print();
                monthBalance.Adjust(previousBalance);
            }
            monthBalance.ProcessTransactions(transactions);
            monthBalance.ProcessTransfers(transfers);
            monthBalance.Print();
            return monthBalance;
        }

        public static void UpdateMembersBalance(MonthBalance balance)
        {
            foreach (var memberBalance in balance.MemberBalances)
            {
                var member = memberBalance.Member;
                if(member == null) throw new Exception("Member not set on balance");
                member.BalanceDate = balance.Date.AddMonths(1); // use 1st of next month, because it is updated to this point
                member.StartBalanceAmountInCents = (int) memberBalance.EndBalance*100;
            }
        }

        private static MonthBalance InitMonthBalance(int year, int month, List<Member> members)
        {
            var result = new MonthBalance {Month = month, Year = year};
            foreach (var member in members)
            {
                var memberBalance = new MemberBalance {Month = month, Year = year, Member = member};
                result.AddBalance(memberBalance);
            }
            return result;
        }

        public static void WriteMonthBalanceFile(string path, MonthBalance monthBalance)
        {
            string file = Path.Combine(path, "memberbalance-" + monthBalance.Year + "-" + monthBalance.Month + ".csv");
            File.WriteAllText(file, monthBalance.CreateCsv());
        }

        public static TransactionList ReadTransactionList(string path, int month, int year)
        {
            var transactionList = new TransactionList { Month = month, Year = year };
            var files = Directory.GetFiles(path, "transactions-*.txt");
            foreach (var fileName in files)
            {
                var info = new FileInfo(fileName);
                int monthFile = TransactionFileHelper.GetMonth(info.Name);
                int yearFile = TransactionFileHelper.GetYear(info.Name);
                if (monthFile != month || yearFile != year) continue;
                string[] lines = File.ReadAllLines(Path.Combine(path, fileName));
                transactionList.Init(lines.ToList());
            }
            return transactionList;
        }

        public static TransferList ReadTranserList(string path, int month, int year)
        {
            var transferList = new TransferList { Month = month, Year = year };
            var files = Directory.GetFiles(path, "transfers-*.csv");
            foreach (var fileName in files)
            {
                var info = new FileInfo(fileName);
                int monthFile = TransactionFileHelper.GetMonth(info.Name);
                int yearFile = TransactionFileHelper.GetYear(info.Name);
                if (monthFile != month || yearFile != year) continue;
                string[] lines = File.ReadAllLines(Path.Combine(path, fileName));
                transferList.Init(lines.ToList());
            }
            return transferList;
        }

        public static DateTime GetValidBalanceDate(List<DateTime> balances)
        {
            if (balances.Count == 0) return DateTime.MinValue;
            DateTime lastMonth = DateTime.MinValue;
            foreach (var memberBalance in balances)
            {
                if (lastMonth == DateTime.MinValue)
                {
                    lastMonth = memberBalance;
                    continue;
                }
                // check gap
                int gap = memberBalance.MonthDiff(lastMonth);
                if (gap > 1) return lastMonth;
                lastMonth = memberBalance;
            }
            return lastMonth;
        }

        public static MonthBalance GetMonthBalance(int month, int year, string lastMonthBalanceFile, List<Member> members)
        {
            string expectedFileName = string.Format("memberbalance-{0}-{1}.csv", year, month);
            if (Path.GetFileName(lastMonthBalanceFile) != expectedFileName) throw new Exception("last month balance unexpected name, should be: " + expectedFileName);
            if(!File.Exists(lastMonthBalanceFile)) throw new Exception("Cannot load balance for date: " + year + " " + month);
            var lines = new List<string>();
            lines.AddRange(File.ReadAllLines(lastMonthBalanceFile));
            var balance = MonthBalance.Parse(month, year, lines, members);
            return balance;
        }

        public static string[] GetBalanceFiles(string path)
        {
            return Directory.GetFiles(path, "memberbalance-*.csv");
        } 

        public static List<DateTime> GetMemberBalanceDates(string[] balanceFiles)
        {
            var list = new List<DateTime>();
            foreach (var fileInfo in balanceFiles)
            {
                int month = TransactionFileHelper.GetMonth(fileInfo);
                int year = TransactionFileHelper.GetYear(fileInfo);
                DateTime date = new DateTime(year, month, 1);
                list.Add(date);
            }
            return list;
        }

    }
}
