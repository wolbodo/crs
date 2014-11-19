using System;
using System.Collections.Generic;
using System.IO;
using CashlessRegisterSystemCore.Helpers;
using CashlessRegisterSystemCore.Model;
using NUnit.Framework;

namespace CashlessRegisterSystemCore.Tasks
{
    public static class SynchronizeFiles
    {
        public static string Execute(string sourcePath, string destinationPath)
        {
            if (string.IsNullOrEmpty(sourcePath)) sourcePath = Environment.CurrentDirectory;
            if (!Directory.Exists(sourcePath)) return "Source path does not exist, fix the config";
            if (string.IsNullOrEmpty(destinationPath)) return "Destination path cannot be empty, fix the config";
            if (!string.IsNullOrEmpty(sourcePath) && sourcePath == destinationPath) return "Source path cannot be the same as destination path, fix the config"; 
            if (!Directory.Exists(destinationPath))
            {
                try
                {
                    Directory.CreateDirectory(destinationPath);
                }
                catch (Exception e)
                {
                    return e.Message;
                }
            }
            // transactions
            var files = new List<string>();
            files.AddRange(Directory.GetFiles(sourcePath, "transactions-*.txt"));
            // add the members file for synchronisation
            files.Add(Path.Combine(sourcePath, Settings.MembersFile));
            foreach (var localFile in files)
            {
                var remoteFile = Path.Combine(destinationPath, Path.GetFileName(localFile));
                try
                {
                    if (File.Exists(remoteFile))
                    {
                        //SynchronizeTransactionFiles(localFile, remoteFile);
                        //File.Delete(remoteFile);
                    }
                    else
                    {
                        File.Copy(localFile, remoteFile);
                    }
                }
                catch (Exception e)
                {
                    return string.Format("Could not synchronize file {0} to {1}: {2}", localFile, remoteFile, e.Message);
                }
            }
            //read in netorkqueue
            //var localList = TransactionList.LoadFromFile(localInfo);
            
            return string.Empty;
        }

        private static void SynchronizeTransactionFiles(string localFile, string remoteFile)
        {
            var localInfo = new FileInfo(localFile);
            var remoteInfo = new FileInfo(remoteFile);

            // skip if lastwrite time and size is the same
            if (localInfo.LastWriteTimeUtc == remoteInfo.LastWriteTimeUtc && localInfo.Length == remoteInfo.Length) return;

            var localList = TransactionList.LoadFromFile(localInfo);
            var remoteList = TransactionList.LoadFromFile(remoteInfo);
            bool changed = SynchronizeTransactionList(localList, remoteList);
            if (changed)
            {
                localList.Save(localInfo);
                remoteList.Save(remoteInfo);
            }
        }

        private static bool SynchronizeTransactionList(TransactionList localList, TransactionList remoteList)
        {
            var remoteMissing = new List<Transaction>();
            var localMissing = new List<Transaction>();

            remoteList.AddRange(remoteMissing);
            localList.AddRange(localMissing);

            return remoteMissing.Count > 0 || localMissing.Count > 0;
        }
    }
}
