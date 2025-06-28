using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace DevWeb_23774_25961.Services
{
    public class EmailSender
    {
        private readonly EmailSettings _settings;

        public EmailSender(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public async Task SendEmailAsync(string emailDestino, string subject, string body)
        {
            var fromAddress = new MailAddress(_settings.EmailFrom);
            var toAddress = new MailAddress(emailDestino);

            var smtp = new SmtpClient
            {
                Host = _settings.Host,
                Port = _settings.Port,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_settings.EmailFrom, _settings.Password)
            };

            using var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };

            await smtp.SendMailAsync(message);
        }
    }

    // Merged Settings Class ✨
    public class EmailSettings
    {
        public string EmailFrom { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }
}