using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CashlessRegisterSystemCore.Model;

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
                if (localFile == null) continue;
                var remoteFile = Path.Combine(destinationPath, Path.GetFileName(localFile));
                try
                {
                    if (!File.Exists(remoteFile))
                    {
                        File.Copy(localFile, remoteFile);
                    }
                }
                catch (Exception e)
                {
                    return string.Format("Could not synchronize file {0} to {1}: {2}", localFile, remoteFile, e.Message);
                }
            }

            for (;;)
            {
                TransactionList queue;
                //Lock queue to get all transactions from the queue
                lock (TransactionList.SERVER_QUEUE_PATH)
                {
                    queue = TransactionList.LoadFromFile(new FileInfo(TransactionList.SERVER_QUEUE_PATH), initMonthYear:false); //Time A
                }
                if (queue.All.Count > 0) //We have transactions to wrote to the remote server
                {
                    var transaction = queue.All[0]; //only get one transaction
                    string transactionFile = Path.Combine(destinationPath, transaction.TransactionDate.ToString(TransactionList.TRANSACTION_LIST_PATH)); //determine remote path based on transaction date
                    try
                    {
                        // this is a slow or failing network write
                        File.AppendAllText(transactionFile, transaction.ToFileLine() + Environment.NewLine, Encoding.UTF8); //append the (one) transaction to the remote file (or create file)
                    }
                    catch (Exception e)
                    {
                        return string.Format("Could not process queue to server: {0}", e.Message);
                    }
                    RemoveFirstFromQueue();
                }
                else
                {
                    //done
                    return string.Empty;
                }
            }
        }

        public static void RemoveFirstFromQueue()
        {
            lock (TransactionList.SERVER_QUEUE_PATH) //Time B //lock the remote queue so we don't get bitten by a transaction write in another thread because another transaction could be appended between A & B
            {
                TransactionList queue = TransactionList.LoadFromFile(new FileInfo(TransactionList.SERVER_QUEUE_PATH), initMonthYear:false); //reload all transactions
                queue.All.RemoveAt(0); //remove the transaction we have written
                File.WriteAllText(TransactionList.ATOMIC_NEW_SERVER_QUEUE_PATH, "");
                foreach (var t in queue.All) //loop over all queued transactions
                {
                    File.AppendAllText(TransactionList.ATOMIC_NEW_SERVER_QUEUE_PATH, t.ToFileLine() + Environment.NewLine, Encoding.UTF8); //append to local file
                }
                File.Delete(TransactionList.SERVER_QUEUE_PATH); //Delete the old queue
                File.Move(TransactionList.ATOMIC_NEW_SERVER_QUEUE_PATH, TransactionList.SERVER_QUEUE_PATH); //Moment C //move the new temp file into place
            }
        }
        
        public static void CrashRecovery()
        {
            if (File.Exists(TransactionList.ATOMIC_NEW_SERVER_QUEUE_PATH)) //we crashed between moment B and C
            {
                if (!File.Exists(TransactionList.SERVER_QUEUE_PATH))
                {
                    File.Move(TransactionList.ATOMIC_NEW_SERVER_QUEUE_PATH, TransactionList.SERVER_QUEUE_PATH); //Moment C //move the new temp file into place
                }
                else
                {
                    RemoveFirstFromQueue();
                }
            }
        }
    }
}