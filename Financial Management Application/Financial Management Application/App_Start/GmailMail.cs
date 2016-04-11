using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace Financial_Management_Application.App_Start
{
    public class GmailMail
    {
        static SmtpClient smtp = new SmtpClient();

        public static void send(string toEmail,string subject, string body)
        {
            var fromAddress = new MailAddress("GoldTeamTechnologies@gmail.com", "Gold Team Technologies");
            var toAddress = new MailAddress(toEmail, toEmail);
            const string fromPassword = "GoldTeam1";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
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