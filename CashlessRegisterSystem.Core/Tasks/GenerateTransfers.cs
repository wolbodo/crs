using System.Collections.Generic;
using System.IO;
using System.Linq;
using CashlessRegisterSystemCore.Model;

namespace CashlessRegisterSystemCore.Tasks
{
    public static class GenerateTransfers
    {
        public static void WriteTransferFiles(string path, List<Member> members, List<Transfer> transfers)
        {
            if (transfers.Count == 0) return;
            var sortedTransfers = transfers.OrderByDescending(x => x.Date).ToList();
            var transferLists = new List<TransferList>();
            foreach (var transfer in sortedTransfers)
            {
                var list = transferLists.SingleOrDefault(x => x.Year == transfer.Date.Year && x.Month == transfer.Date.Month);
                if (list == null)
                {
                    list = new TransferList {Year = transfer.Date.Year, Month = transfer.Date.Month};
                    transferLists.Add(list);
                } 
                list.Add(transfer);
            }

            foreach (var transferList in transferLists)
            {
                string file = Path.Combine(path, "transfers-" + transferList.Year + "-" + transferList.Month + ".csv");
                File.WriteAllText(file, transferList.CreateCsv());
            }
        }
    }
}
