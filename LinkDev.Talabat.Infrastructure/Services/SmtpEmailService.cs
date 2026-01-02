using LinkDev.Talabat.Core.Services.Contracts;
using LinkDev.Talabat.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace LinkDev.Talabat.Infrastructure.Services
{
    public class SmtpEmailService(IOptions<EmailSettings> emailSettingsOptions) : IEmailService
    {
        private readonly EmailSettings emailSettings = emailSettingsOptions.Value;
        public async Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
        {
            using var client = new SmtpClient(emailSettings.SmtpHost, emailSettings.SmtpPort);
            client.Credentials = new NetworkCredential(emailSettings.Username, emailSettings.Password);
            client.EnableSsl = emailSettings.UseSsl;
            using var message = new MailMessage();
            message.From = new MailAddress(emailSettings.From, emailSettings.DisplayName);
            message.To.Add(to);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;
            await client.SendMailAsync(message, cancellationToken);
        }
    }
}
