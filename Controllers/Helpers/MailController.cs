using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Web.Configuration;
using System.Net;

namespace Ark.App_Code
{
    public class MailController
    {
        public static string MailServer
        {
            get;
            set;
        }

        public static void SendMail(string from, string to, string cc, string bcc, string subject, string body)
        {
            SmtpClient client = new SmtpClient();

            MailMessage m = new MailMessage();
            m.From = new MailAddress(from);
            m.Subject = subject;
            m.Body = body;

            if (!string.IsNullOrEmpty(to))
            {
                string[] toAddresses = to.Trim(',').Split(',');

                foreach (string toAddress in toAddresses)
                {
                    m.To.Add(new MailAddress(toAddress));
                }
            }

            if (!string.IsNullOrEmpty(cc))
            {
                string[] ccAddresses = cc.Trim(',').Split(',');

                foreach (string ccAddress in ccAddresses)
                {
                    if (!string.IsNullOrEmpty(ccAddress))
                        m.CC.Add(new MailAddress(ccAddress));
                }
            }

            if (!string.IsNullOrEmpty(bcc))
            {
                string[] bccAddresses = bcc.Trim(',').Split(',');

                foreach (string bccAddress in bccAddresses)
                {
                    if (!string.IsNullOrEmpty(bccAddress))
                        m.Bcc.Add(new MailAddress(bccAddress));
                }
            }

            string smtpHost = WebConfigurationManager.AppSettings["SmtpHost"];
            int smtpPort = int.Parse(WebConfigurationManager.AppSettings["SmtpPort"]);
            string userName = WebConfigurationManager.AppSettings["SmtpUsername"];
            string password = WebConfigurationManager.AppSettings["SmtpPassword"];

            m.IsBodyHtml = true;
            client.Host = smtpHost;
            client.Port = smtpPort;

            if (!string.IsNullOrEmpty(userName))
            {
                NetworkCredential credentials = new NetworkCredential(userName, password);
                client.Credentials = credentials;
                client.EnableSsl = true;
            }

            client.Send(m);

        }
    }
}
