using Microsoft.Extensions.Configuration;
using System;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SendGrid.Helpers.Mail;

namespace UrGuide.WebApp.Services
{
    public class EmailService : IEmailService
    {
        public IConfiguration Configuration { get; }

        public enum MessageTypes
        {
            Confirmation = 1,
            PasswordReset = 2,
            ChangePassword = 3
        }

        public EmailService(IConfiguration configuration)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        public async Task Send(string email, string messageBody, MessageTypes messageType, CancellationToken cancellationToken)
        {
            var subject = messageType switch
            {
                MessageTypes.Confirmation => "UrGuide - Email confirmation",
                MessageTypes.PasswordReset => "UrGuide - Password reset",
                MessageTypes.ChangePassword => "UrGuide - Change password",
                _ => "UrGuide - Automatic email",
            };

            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_URGUIDE_API_KEY");
            var message = new MailMessage("noreply@urguide.org", email);
            message.Subject = subject;
            message.Body = messageBody;
            message.BodyEncoding = Encoding.UTF8;
            message.IsBodyHtml = true;
            message.Priority = MailPriority.High;
            var client = new SendGrid.SendGridClient(apiKey);
            var mail = MailHelper.CreateSingleEmail(new EmailAddress("noreply@urguide.org", "UrGuide"), new EmailAddress(email), subject, messageBody, messageBody);
            var response = await client.SendEmailAsync(mail, cancellationToken);
           
        }
    }
}
