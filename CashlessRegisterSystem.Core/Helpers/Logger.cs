using System;
using System.IO;

namespace CashlessRegisterSystemCore.Helpers
{
    public static class Logger
    {
        private const string ERRORS_PATH = "'errors-'yyyy'-'MM'.txt'";

        public static void Error(Exception e, string text)
        {
            File.AppendAllText(DateTime.Now.ToString(ERRORS_PATH), string.Format("{0:yyyy'-'MM'-'dd';'HH':'mm':'ss} Read error: {1} for line {2}" + Environment.NewLine, DateTime.Now, e.Message, text));
        }
    }
}
