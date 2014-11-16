using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CashlessRegisterSystemCore.Model
{
    public class MonthBalance
    {
        public MonthBalance()
        {
            MemberBalances = new List<MemberBalance>();
        }

        public void AddBalance(MemberBalance balance)
        {
            MemberBalances.Add(balance);
            TotalStartBalance += balance.StartBalance;
            TotalCoasters += balance.Coasters;
            TotalIncasso += balance.Incasso;
            TotalDeposit += balance.Deposit;
            TotalReceipt += balance.Receipt;
            TotalContribution += balance.Contribution;
            TotalActivity += balance.Activity;
            TotalEndBalance += balance.EndBalance;
            MemberBalances = MemberBalances.OrderBy(x => x.Member.Name).ToList();
        }

        public List<MemberBalance> MemberBalances { get; set; }
        public List<Transaction> Transactions { get; set; } 
        public int Month { get; set; }
        public int Year { get; set; }

        public DateTime Date
        {
            get { return new DateTime(Year, Month, 1); }
        }

        public void CalculateTotals()
        {
            TotalStartBalance = 0;
            TotalIncasso = 0;
            TotalDeposit = 0;
            TotalCash = 0;
            TotalReceipt = 0;
            TotalContribution = 0;
            TotalActivity = 0;
            foreach (var memberBalance in MemberBalances)
            {
                TotalStartBalance += memberBalance.StartBalance;
                TotalCoasters += memberBalance.Coasters;
                TotalIncasso += memberBalance.Incasso;
                TotalCash += memberBalance.Cash;
                TotalDeposit += memberBalance.Deposit;
                TotalReceipt += memberBalance.Receipt;
                TotalContribution += memberBalance.Contribution;
                TotalActivity += memberBalance.Activity;
            }
        }

        public decimal TotalStartBalance { get; set; }
        public decimal TotalCoasters { get; set; }
        public decimal TotalIncasso { get; set; }
        public decimal TotalCash { get; set; }
        public decimal TotalDeposit { get; set; }
        public decimal TotalReceipt { get; set; }
        public decimal TotalContribution { get; set; }
        public decimal TotalActivity { get; set; }
        public decimal TotalEndBalance { get; set; }

        public void ProcessTransactions(TransactionList transactionsMonth)
        {
            if (transactionsMonth.Year != Year || transactionsMonth.Month != Month) throw new Exception("Invalid transactions for month: " + Year + "-" + Month);
            var transactions = transactionsMonth.All.Where(x => x.TransactionDate.Year == Year && x.TransactionDate.Month == Month);
            foreach (var transaction in transactions)
            {
                var memberBalance = MemberBalances.SingleOrDefault(x => x.Member.Name.ToLower() == transaction.MemberName.ToLower());
                if (memberBalance == null)
                {
                    Console.WriteLine("Could not process transactions for member : " + transaction.MemberName + " in month " + transactionsMonth.Year + "-" + transactionsMonth.Month);
                    continue;
                    //throw new Exception("Could not process transactions for member : " + transaction.MemberName + " in month " + transactionsMonth.Year + "-" + transactionsMonth.Month);
                }
                memberBalance.ProcessTransaction(transaction);
            }
        }

        public void ProcessTransfers(TransferList transfersMonth)
        {
            if (transfersMonth.Year != Year || transfersMonth.Month != Month) throw new Exception("Invalid transfers for month: " + Year + "-" + Month);
            foreach (var transfer in transfersMonth.All)
            {
                var memberBalance = MemberBalances.SingleOrDefault(x => x.Member.Name.ToLower() == transfer.MemberName.ToLower());
                if (memberBalance == null) throw new Exception("Could not process transfers for member : " + transfer.MemberName + " in month " + transfersMonth.Year + "-" + transfersMonth.Month);
                memberBalance.ProcessTransfer(transfer);
            }
        }
        
        public void Adjust(MonthBalance lastMonthBalance)
        {
            foreach (var memberBalance in MemberBalances)
            {
                var lastMemberBalance = lastMonthBalance.MemberBalances.SingleOrDefault(x => x.Member == memberBalance.Member);
                if(lastMemberBalance == null) throw new Exception("Could not find balance for last month for member: " + memberBalance.Member.Name + " " + memberBalance.Year + "-" + memberBalance.Month);
                memberBalance.Adjust(lastMemberBalance);
            }
        }

        public string CreateCsv()
        {
            var result = new StringBuilder();
            result.AppendLine(string.Format("#Maandbalans voor maand: {0}-{1}", Year, Month));
            foreach (var balance in MemberBalances)
            {
                result.AppendLine(balance.Member.Name + ";" + balance.StartBalance + ";" + balance.Coasters + ";" + balance.Incasso + ";" + balance.Cash + ";" + balance.Deposit + ";" + balance.Receipt + ";" + balance.Contribution + ";" + balance.Activity + ";" + balance.EndBalance);
            }
            result.AppendLine();
            result.AppendLine("Totaal;" + TotalStartBalance + ";" + TotalCoasters + ";" + TotalIncasso + ";" + TotalCash + ";" + TotalDeposit + ";" + TotalReceipt + ";" + TotalContribution + ";" + TotalActivity + ";" + TotalEndBalance);
            return result.ToString();
        }

        public static MonthBalance Parse(int month, int year, List<string> lines, List<Member> members)
        {
            var result = new MonthBalance {Month = month, Year = year};
            foreach (var line in lines)
            {
                string[] values = line.Split(';');
                if(line.StartsWith("#") || line.StartsWith("Totaal") || string.IsNullOrEmpty(line)) continue;
                
                string memberName = values[0];
                if (string.IsNullOrEmpty(memberName)) continue;
                var member = members.SingleOrDefault(x => x.Name.ToLower() == memberName.ToLower());
                if(member == null) throw new Exception("Could not find member for line: " + line);
                var memberBalance = MemberBalance.Parse(month, year, line);
                memberBalance.Member = member;
                result.AddBalance(memberBalance);
            }
            result.CalculateTotals();
            return result;
        }

        public void Print()
        {
            Console.WriteLine("MonthBalance " + Year + "-" + Month);
            foreach (var memberBalance in MemberBalances)
            {
                memberBalance.Print();
            }
        }
    }
}
