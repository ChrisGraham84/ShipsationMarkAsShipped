using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ShipStationMarkAsShipped.Helpers
{
    public static class MailHelpers
    {
        public static void SendEmailWithOrders(string CompanyName)
        {
            Console.WriteLine("Mailling...");
            MailMessage mail = new MailMessage();
            SmtpClient server = new SmtpClient("smtp.gmail.com");
            mail.From = new MailAddress("resources@email.com");
            mail.To.Add("admin@email.com,admin2@email.com");
            mail.Bcc.Add("resources@email.com");
            mail.Subject = CompanyName + " Orders From Ship Station";
            mail.Body = CompanyName + " Shipstation file. Sent From Shipstation Managment Tool";

            System.Net.Mail.Attachment attachment;
            attachment = new System.Net.Mail.Attachment(System.Configuration.ConfigurationManager.AppSettings["Path"] + CompanyName + " orders.txt");
            mail.Attachments.Add(attachment);

            server.Port = 587;
            server.Credentials = new System.Net.NetworkCredential("resources@email.com", "5HB7c9ut");
            server.EnableSsl = true;

            server.Send(mail);
            Console.WriteLine("Email Sent");
        }
    }
}

