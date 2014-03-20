using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using CashlessRegisterSystemCore.Helpers;

namespace CashlessRegisterSystemCore.Model
{
    [DebuggerDisplay("TransferList : {Year}-{Month} {All.Count}")]
    public class TransferList : NotifyList
    {
        private const string TRANSFERS_LIST_PATH = "'transfers-'yyyy'-'MM'.txt'";
        public List<Transfer> All { get; private set; }
        public int Month { get; set; }
        public int Year { get; set; }

        public DateTime Date
        {
            get
            {
                return new DateTime(Year, Month, 1);
            }
        }

        public TransferList()
        {
            All = new List<Transfer>();
        }

        public void Add(Transfer transfer)
        {
            All.Add(transfer);
        }

        public static TransferList LoadAndWatchFromFile()
        {
            var list = new TransferList();
            list.Init(ReadTransfersLastTwoMonths());
            var transferWatcher = new FileSystemWatcher(Environment.CurrentDirectory, "transfers-*.txt");
            //transferWatcher.Changed += OnChanged;
            //transferWatcher.Created += OnChanged;
            transferWatcher.EnableRaisingEvents = true;
            return list;
        }

        public static List<string> ReadTransfersLastTwoMonths()
        {
            var transferLines = new List<string>();
            DateTime transfersTimeThisMonth = DateTime.Now;
            string[] transfersFiles = {transfersTimeThisMonth.AddMonths(-1).ToString(TRANSFERS_LIST_PATH), transfersTimeThisMonth.ToString(TRANSFERS_LIST_PATH)};

            foreach (string transfersFile in transfersFiles)
            {
                if (!File.Exists(transfersFile)) continue;
                transferLines.AddRange(File.ReadAllLines(transfersFile, Encoding.UTF8));
            }
            return transferLines;
        }

        public void Init(List<string> lines)
        {
            All = new List<Transfer>();
            foreach (string line in lines)
            {
                if (line.StartsWith("#") || string.IsNullOrEmpty(line)) continue;
                var transfer = Transfer.Parse(line);
                if (transfer != null)
                {
                    All.Add(transfer);
                }
            }
        }

        public string CreateCsv()
        {
            var result = new StringBuilder();
            result.AppendLine(string.Format("#Transfer voor maand: {0}-{1}", Year, Month));
            foreach (var transfer in All)
            {
                result.AppendLine(transfer.Date.ToString("yyyy-MM-dd") + ";" + transfer.Member.Name + ";" + transfer.Amount + ";" + transfer.Type + ";" + transfer.Note);
            }
            result.AppendLine();
            return result.ToString();
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            var watcher = source as FileSystemWatcher;
            messageNotice(new MessageEventArgs {Type = MessageType.Service, Message = "Momenteel worden de overschrijvingen bijgewerkt, mocht dit scherm onafgebroken voor langer dan 1 minuut op het scherm staan neem dan z.s.m. contact op met Trui en Benjamin."});
            bool ok = false;
            while (!ok)
            {
                try
                {
                    ReadTransfersLastTwoMonths();
                    ok = true;
                }
                catch
                {
                    Thread.Sleep(100);
                }
            }
            dataChange(source, e);
        }

    }
}
