using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace DevWeb_23774_25961.Services
{
    public class EmailSender
    {
        private readonly string _emailFrom = "devweblivros@sapo.pt";
        private readonly string _password = "JmF.2025";

        public async Task SendEmailAsync(string emailDestino, string subject, string body)
        {
            var fromAddress = new MailAddress(_emailFrom);
            var toAddress = new MailAddress(emailDestino);

            var smtp = new SmtpClient
            {
                Host = "smtp.sapo.pt",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_emailFrom, _password)
            };

            using var message = new MailMessage(fromAddress, toAddress);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = false;

            await smtp.SendMailAsync(message);
        }
    }
}