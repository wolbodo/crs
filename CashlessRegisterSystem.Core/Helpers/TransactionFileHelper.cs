using System.IO;

namespace CashlessRegisterSystemCore.Helpers
{
    public static class TransactionFileHelper
    {
        public static int GetMonth(string fileName)
        {
            fileName = fileName.Replace(Path.GetExtension(fileName), string.Empty);
            var splits = fileName.Split('-');
            return int.Parse(splits[2]);
        }

        public static int GetYear(string fileName)
        {
            fileName = fileName.Replace(Path.GetExtension(fileName), string.Empty);
            var splits = fileName.Split('-');
            return int.Parse(splits[1]);
        } 
    }
}
