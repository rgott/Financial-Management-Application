using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace Financial_Management_Application
{
    public class Mail
    {
        static SmtpClient smtp = new SmtpClient();
        public static void send(string toEmail,string subject, string body)
        {
            var fromAddress = new MailAddress(AppSettings.Mail.email, AppSettings.Mail.displayName);
            var toAddress = new MailAddress(toEmail, toEmail);

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, AppSettings.Mail.password)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }
    }
}