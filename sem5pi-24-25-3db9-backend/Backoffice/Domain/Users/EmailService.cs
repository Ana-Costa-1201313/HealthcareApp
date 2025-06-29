using Microsoft.Extensions.Configuration;
using Backoffice.Domain.Shared;
using System;
using System.Net;
using System.Net.Mail;

namespace Backoffice.Domain.Users
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<bool> SendEmail(string emailAddress, string message, string subject)
        {
            try
            {
                // SMTP server configuration
                var smtpClient = new SmtpClient(_config["EmailSmtp:SmtpServer"])
                {
                    Port = int.Parse(_config["EmailSmtp:SmtpPort"]),
                    Credentials = new NetworkCredential(_config["EmailSmtp:SmtpEmail"], _config["EmailSmtp:SmtpKey"]),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_config["EmailSmtp:SmtpEmail"]),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = false,
                };
                mailMessage.To.Add(emailAddress);
                try
                {
                    await smtpClient.SendMailAsync(mailMessage);
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending email: {ex.Message}");
                    throw new SmtpException(ex.Message);
                }
            }
            catch (Exception e)
            {
                throw new SmtpException(e.Message);
            }
        }
    }
}
