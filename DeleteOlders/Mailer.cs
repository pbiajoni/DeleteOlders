using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
using System.Text;
using System.Web.Configuration;

namespace DeleteOlders
{
    public class Mailer
    {

        public void Send(string tag, string subect, string message)
        {
            string host = ConfigurationManager.AppSettings["host"].ToString();
            string password = ConfigurationManager.AppSettings["password"].ToString();
            string username = ConfigurationManager.AppSettings["username"].ToString();
            int port = Convert.ToInt32(ConfigurationManager.AppSettings["port"].ToString());
            string to = ConfigurationManager.AppSettings["to"].ToString();

            //cria uma mensagem
            MailMessage mail = new MailMessage();
            mail.IsBodyHtml = false;

            //define os endereços
            mail.From = new MailAddress(username, "Delete Olders");
            mail.To.Add(to);

            //define o conteúdo
            mail.Subject = tag + " - " + subect;
            mail.Body = message;

            //envia a mensagem
            SmtpClient smtp = new SmtpClient(host, port);
            smtp.EnableSsl = false;
            smtp.UseDefaultCredentials = false;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Credentials = new NetworkCredential(username, password);
            smtp.Send(mail);
        }
    }
}
