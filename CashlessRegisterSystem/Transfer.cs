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
    [DebuggerDisplay("Transfer : {MemberName} {PaymentDate} {AmountInCents} {AccountNumber} {AccountName} {Note}")]
    public class Transfer
    {
        private const string TRANSFERS_LIST_PATH = "'transfers-'yyyy'-'MM'.txt'";
        private const string ERRORS_PATH = "'errors-'yyyy'-'MM'.txt'";

        public string MemberName { get; set; }
        public DateTime PaymentDate { get; set; }
        public int AmountInCents { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string Note { get; set; }
        
        public static List<Transfer> All { get; private set; }

        static Transfer()
        {
            LoadTransfers();
            FileSystemWatcher transferWatcher = new FileSystemWatcher(Environment.CurrentDirectory, "transfers-*.txt");
            transferWatcher.Changed += new FileSystemEventHandler(OnChanged);
            transferWatcher.Created += new FileSystemEventHandler(OnChanged);
            transferWatcher.EnableRaisingEvents = true;
        }

        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            GUI.messageNotice(new MessageEventArgs { Type = MessageType.Service, Message = "Momenteel worden de overschrijvingen bijgewerkt, mocht dit scherm onafgebroken voor langer dan 1 minuut op het scherm staan neem dan z.s.m. contact op met Trui en Benjamin." });
            bool ok = false;
            while (!ok)
            {
                try
                {
                    LoadTransfers();
                    ok = true;
                }
                catch
                {
                    Thread.Sleep(100);
                }
            }
            GUI.dataChange(source, e);
        }

        public static Transfer Parse(string line)
        {
            if (line.StartsWith("#"))
                return null;
            string[] values = line.Split(';');
            if (values.Length >= 6)
            {
                try
                {
                    return
                        new Transfer
                        {
                            MemberName = values[0],
                            PaymentDate = DateTime.ParseExact(values[1], "yyyy'-'MM'-'dd", DateTimeFormatInfo.InvariantInfo),
                            AmountInCents = int.Parse(values[2]),
                            AccountNumber = values[3],
                            AccountName = values[4],
                            Note = values[5]
                        };
                }
                catch (Exception)
                {
                    File.AppendAllText(DateTime.Now.ToString(ERRORS_PATH), string.Format("{0:yyyy'-'MM'-'dd';'HH':'mm':'ss} Read error: {1}\r\n", DateTime.Now, line));
                }
            }
            return null;
        }


        private static void LoadTransfers()
        {
            All = new List<Transfer>();

            DateTime transfersTimeThisMonth = DateTime.Now;
            String[] transfersFiles = { transfersTimeThisMonth.AddMonths(-1).ToString(TRANSFERS_LIST_PATH), transfersTimeThisMonth.ToString(TRANSFERS_LIST_PATH) };

            foreach (String transfersFile in transfersFiles)
            {
                if (!File.Exists(transfersFile))
                    continue;
                string[] lines = File.ReadAllLines(transfersFile, ASCIIEncoding.UTF8);

                foreach (string line in lines)
                {
                    Transfer transfer = Transfer.Parse(line);
                    if (transfer != null)
                    {
                        All.Add(transfer);
                    }
                }
            }

            Member.ClearAndAddTransfers(All);
        }
    }
}
