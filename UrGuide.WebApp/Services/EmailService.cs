using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;
using SendGrid.Helpers.Mail;
using UrGuide.Shared.Contracts;
using UrGuide.Model.Messages;
using UrGuide.Shared;

namespace UrGuide.WebApp.Services
{
    public partial class EmailService : IEmailService
    {
        public IConfiguration Configuration { get; }

        public EmailService(IConfiguration configuration)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public Task SendAsync(SendDirectMessageCommand message)
        {
            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_URGUIDE_API_KEY");

            var client = new SendGrid.SendGridClient(apiKey);
            var mail = MailHelper.CreateSingleEmail(new EmailAddress("noreply@urguide.org", "UrGuide"), 
                new EmailAddress(message.To), message.Subject, null, null);
            mail.TemplateId = "d-eee7f1abc3a94f13a49ab087f6268be5";
            mail.SetTemplateData(new { message.Subject, message.Content, message.Link, message.ToName, message.LinkText });
            return client.SendEmailAsync(mail);
        }
    }
}
