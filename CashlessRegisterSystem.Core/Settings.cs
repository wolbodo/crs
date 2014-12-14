using System;
using System.Configuration;
using System.Globalization;

namespace CashlessRegisterSystemCore
{
    public static class Settings
    {
        public static DateTimeFormatInfo DateTimeInfo = DateTimeFormatInfo.GetInstance(CultureInfo.GetCultureInfo("nl-NL"));

        public static string MembersFile
        {
            get { return GetSetting("MembersFile", "members.txt"); }
        }

        public static string LocalTransactionsPath
        {
            get { return GetSetting("SourceTransactionsPath", Environment.CurrentDirectory); }
        }

        // our backup location
        public static string RemoteTransactionsPath
        {
            get { return GetSetting("RemoteTransactionsPath", @"\\tommie\music_upload\viltjessysteem\"); }
        }

        public static string SmtpServer
        {
            get { return GetSetting("SmtpServer", "smtp1.casema.nl"); }
        }

        public static int SmtpPort
        {
            get { return GetIntSetting("SmtpPort", 25); }
        }

        public static bool SmtpUseSSL
        {
            get { return GetBoolSetting("SmtpUseSSL", false); }
        }

        public static string SmtpLogin
        {
            get { return GetValue("SmtpLogin"); }
        }

        public static string SmtpPassword
        {
            get { return GetValue("SmtpPassword"); }
        }

        private static string GetSetting(string keyName, string @default = "")
        {
            var setting = GetValue(keyName) ?? @default;
            return setting;
        }

        private static bool GetBoolSetting(string keyName, bool @default = false)
        {
            var setting = GetValue(keyName);
            if (setting == null) return @default;
            return (setting == "1" || setting.ToLower() == "true");
        }

        private static int GetIntSetting(string keyName, int @default = 0)
        {
            var setting = GetValue(keyName);
            if (setting == null) return @default;
            int res;
            bool ok = int.TryParse(setting, out res);
            if (ok) return res;
            return @default;
        }

        private static string GetValue(string keyName)
        {
            string key = keyName + Environment.MachineName;
            string res = ConfigurationManager.AppSettings[key];
            if (String.IsNullOrEmpty(res))
            {
                res = ConfigurationManager.AppSettings[keyName];
            }
            return res;
        }
    }
}
