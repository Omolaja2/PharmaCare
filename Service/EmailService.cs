using Microsoft.Extensions.Options;
using PharmacyApp.Models;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace PharmacyApp.Services
{
    public class EmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            using (var client = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(_settings.FromEmail, _settings.Password);

                var mailMessage = new MailMessage(_settings.FromEmail, toEmail, subject, message)
                {
                    IsBodyHtml = true
                };

                await client.SendMailAsync(mailMessage);
            }
        }
    }
}
