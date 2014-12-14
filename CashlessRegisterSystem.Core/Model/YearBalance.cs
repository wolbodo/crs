using System.Collections.Generic;

namespace CashlessRegisterSystemCore.Model
{
    public class YearBalance
    {
        public YearBalance(int year)
        {
            Year = year;
            MonthBalances = new Dictionary<int, MonthBalance>();
            Members = new List<Member>();
        }

        public int Year { get; private set; }
        public List<Member> Members { get; private set; } 
        public Dictionary<int, MonthBalance> MonthBalances { get; private set; } 
    }
}
