using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;
using SendGrid.Helpers.Mail;
using UrGuide.Shared.Contracts;
using UrGuide.Model.Messages;
using UrGuide.Shared;
using Microsoft.Extensions.Logging;

namespace UrGuide.WebApp.Services
{
    public partial class EmailService : IEmailService
    {
        public IConfiguration Configuration { get; }
        public ILogger<EmailService> Logger { get; }

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SendAsync(SendDirectMessageCommand message)
        {
            var apiKey = Configuration.GetValue<string>("SENDGRID_URGUIDE_API_KEY");

            var client = new SendGrid.SendGridClient(apiKey);
            var mail = MailHelper.CreateSingleEmail(new EmailAddress("noreply@urguide.org", "UrGuide"), 
                new EmailAddress(message.To), message.Subject, null, message.Content);
            mail.TemplateId = "d-eee7f1abc3a94f13a49ab087f6268be5";
            mail.SetTemplateData(new { message.Subject, message.Content, message.Link, message.ToName, message.LinkText });
            var response  = await client.SendEmailAsync(mail);
            if(response.StatusCode != System.Net.HttpStatusCode.Accepted)
            {
                var error = await response.Body.ReadAsStringAsync();
                Logger.LogError(error);
            }
        }
    }
}
