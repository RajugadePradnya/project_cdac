using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace RapidReachApi.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var host = _config["SmtpSettings:Host"];
            var port = int.Parse(_config["SmtpSettings:Port"]);
            var enableSsl = bool.Parse(_config["SmtpSettings:EnableSsl"]);
            var fromEmail = _config["SmtpSettings:FromEmail"];
            var username = _config["SmtpSettings:Username"];
            var password = _config["SmtpSettings:Password"];

            using var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = enableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false
            };

            var mail = new MailMessage(fromEmail, toEmail, subject, body)
            {
                IsBodyHtml = true // Allows HTML formatting (for links, etc.)
            };

            // Optional: Add error handling/logging here
            try
            {
                await client.SendMailAsync(mail);
            }
            catch (SmtpException ex)
            {
                // You can log ex.Message or throw for controller/global error handling
                throw new SmtpException("Email sending failed: " + ex.Message, ex);
            }
        }
    }
}
