using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace CashlessRegisterSystemCore.Tasks
{
    public static  class EmailTransactionOverview
    {
        public static void Send(string recipient, string subject, string body, string attachmentFilename)
        {
            var smtpClient = new SmtpClient();
            var basicCredential = new NetworkCredential(Settings.SmtpLogin, Settings.SmtpPassword);
            var message = new MailMessage();
            var fromAddress = new MailAddress(Settings.SmtpLogin);

            smtpClient.Host = Settings.SmtpServer;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = basicCredential;
            smtpClient.Timeout = (60 * 5 * 1000);
            smtpClient.EnableSsl = Settings.SmtpUseSSL;

            message.From = fromAddress;
            message.Subject = subject;
            message.IsBodyHtml = false;
            message.Body = body;
            message.To.Add(recipient);

            if (attachmentFilename != null)
                message.Attachments.Add(new Attachment(attachmentFilename));

            smtpClient.Send(message);
        }
    }
}
