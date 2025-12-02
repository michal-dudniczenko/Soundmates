using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Soundmates.Domain.Interfaces.Services;
using System;
using System.Threading.Tasks;

namespace Soundmates.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                var emailSettings = _configuration.GetSection("EmailSettings");
                var smtpServer = emailSettings["SmtpServer"];
                var port = emailSettings["Port"];
                var senderEmail = emailSettings["SenderEmail"];
                var password = emailSettings["Password"];
                var displayName = emailSettings["DisplayName"] ?? "Soundmates";

                // Check if email settings are configured
                if (string.IsNullOrEmpty(smtpServer) || smtpServer == "smtp.example.com")
                {
                    _logger.LogWarning("Email settings are not configured. Email to {To} with subject '{Subject}' was not sent.", to, subject);
                    _logger.LogInformation("Email body (not sent): {Body}", body);
                    return;
                }

                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(displayName, senderEmail));
                email.To.Add(MailboxAddress.Parse(to));
                email.Subject = subject;
                email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(smtpServer, int.Parse(port ?? "587"), MailKit.Security.SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(senderEmail, password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                _logger.LogInformation("Email sent successfully to {To} with subject '{Subject}'", to, subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {To} with subject '{Subject}'", to, subject);
                _logger.LogInformation("Email body (failed to send): {Body}", body);
                // Don't throw - log the report instead of failing the request
            }
        }
    }
}
