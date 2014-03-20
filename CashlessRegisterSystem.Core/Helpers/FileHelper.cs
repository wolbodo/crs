using System.Collections.Generic;
using System.IO;

namespace CashlessRegisterSystemCore.Helpers
{
    public static class FileHelper
    {
        public const string NETWORK_PATH_PREFIX = @"\\tommie\music_upload\viltjessysteem\";

        public static List<string> SearchFiles(string pPath, string pPatterns)
        {
            //var fRoot = new DirectoryInfo(pPath);
            var extensions = new List<string>();
            var files = new List<string>();
            extensions.AddRange(pPatterns.Split(','));
            var filesInDir = Directory.GetFiles(pPath, pPatterns);
            foreach (var file in filesInDir)
            {
                var extension = Path.GetExtension(file);
                if (extension != null && extensions.Contains(extension.ToLower()))
                {
                    files.Add(file);
                }
            }
            return files;
        }
    }
}
