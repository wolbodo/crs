using System;

namespace CashlessRegisterSystemCore.Helpers
{
    public static class DateTimeExtensions
    {
        public static int MonthDiff(this DateTime d1, DateTime d2)
        {
            if (d1 < d2) throw new Exception(string.Format("Date 1 is smaller than date 2: {0} vs {1}", d1, d2));
            return ((d1.Year - d2.Year) * 12) + d1.Month - d2.Month;
        }
    }
}
