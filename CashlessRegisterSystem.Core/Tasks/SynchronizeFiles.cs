using System;
using System.Collections.Generic;
using System.IO;
using CashlessRegisterSystemCore.Helpers;
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
            foreach (var sourceFile in files)
            {
                var remoteFile = Path.Combine(destinationPath, Path.GetFileName(sourceFile));
                try
                {
                    if (File.Exists(remoteFile)) File.Delete(remoteFile);
                    File.Copy(sourceFile, remoteFile);
                }
                catch (Exception e)
                {
                    return string.Format("Could not synchronize file {0} to {1}: {2}", sourceFile, remoteFile, e.Message);
                }
            }
            return string.Empty;
        }
    }
}
