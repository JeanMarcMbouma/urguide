using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UrGuide.Shared.Contracts;
using UrGuide.Model.Messages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity.UI.Services;
using UrGuide.Services.Email;

namespace UrGuide.WebApp.Services
{
    /// <summary>
    /// SMTP-based email delivery service (MailKit) that integrates with the proprietary
    /// admin-managed template engine for rendering. Falls back to raw content when no
    /// template name is supplied, preserving backwards-compatibility with existing callers.
    /// </summary>
    public partial class EmailService : IEmailService, IEmailSender
    {
        /// <summary>Default BCP 47 language tag used when the caller does not specify one.</summary>
        public const string DefaultLanguage = "en";

        private readonly IConfiguration _configuration;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IConfiguration configuration,
            IEmailTemplateService emailTemplateService,
            ILogger<EmailService> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _emailTemplateService = emailTemplateService ?? throw new ArgumentNullException(nameof(emailTemplateService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SendAsync(SendDirectMessageCommand message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            var subject = message.Subject;
            var htmlBody = message.Content;
            string plainTextBody = null;

            // Use the proprietary DB template engine when a template name is provided
            if (!string.IsNullOrEmpty(message.TemplateName))
            {
                var language = string.IsNullOrEmpty(message.Language) ? DefaultLanguage : message.Language;
                var variables = message.TemplateVariables != null
                    ? new Dictionary<string, string>(message.TemplateVariables)
                    : new Dictionary<string, string>();

                // Inject well-known variables from the command so templates can reference them
                if (!string.IsNullOrEmpty(message.ToName))
                    variables.TryAdd("ToName", message.ToName);
                if (!string.IsNullOrEmpty(message.Link))
                    variables.TryAdd("Link", message.Link);
                if (!string.IsNullOrEmpty(message.LinkText))
                    variables.TryAdd("LinkText", message.LinkText);
                if (!string.IsNullOrEmpty(message.Content))
                    variables.TryAdd("Content", message.Content);

                var renderResult = await _emailTemplateService.RenderEmailAsync(
                    message.TemplateName, language, variables);

                if (!renderResult.IsError)
                {
                    subject = renderResult.Value.Subject;
                    htmlBody = renderResult.Value.HtmlBody;
                    plainTextBody = renderResult.Value.PlainTextBody;
                }
                else
                {
                    _logger.LogWarning(
                        "Template '{TemplateName}' not found for language '{Language}'. Falling back to raw content.",
                        message.TemplateName, language);
                }
            }

            await SendSmtpAsync(message.To, message.ToName, subject, htmlBody, plainTextBody);
        }

        /// <summary>
        /// Implements <see cref="IEmailSender"/> for ASP.NET Core Identity.
        /// </summary>
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return SendAsync(new SendDirectMessageCommand
            {
                Content = htmlMessage,
                To = email,
                Subject = subject
            });
        }

        // ------------------------------------------------------------------ //
        // Internal SMTP delivery (MailKit)                                   //
        // ------------------------------------------------------------------ //

        private async Task SendSmtpAsync(string to, string toName, string subject, string htmlBody, string plainTextBody = null)
        {
            var smtpSection = _configuration.GetSection("Smtp");
            var host = smtpSection.GetValue<string>("Host") ?? "localhost";
            var port = smtpSection.GetValue<int>("Port", 587);
            var username = smtpSection.GetValue<string>("Username");
            var password = smtpSection.GetValue<string>("Password");
            var fromEmail = smtpSection.GetValue<string>("FromEmail") ?? "noreply@urguide.org";
            var fromName = smtpSection.GetValue<string>("FromName") ?? "UrGuide";
            var enableSsl = smtpSection.GetValue<bool>("EnableSsl", true);

            var mail = new MimeMessage();
            mail.From.Add(new MailboxAddress(fromName, fromEmail));
            mail.To.Add(new MailboxAddress(string.IsNullOrEmpty(toName) ? to : toName, to));
            mail.Subject = subject ?? string.Empty;

            var bodyBuilder = new BodyBuilder { HtmlBody = htmlBody ?? string.Empty };
            if (!string.IsNullOrEmpty(plainTextBody))
            {
                bodyBuilder.TextBody = plainTextBody;
            }
            mail.Body = bodyBuilder.ToMessageBody();

            try
            {
                using var client = new SmtpClient();
                var secureSocketOptions = enableSsl
                    ? SecureSocketOptions.StartTls
                    : SecureSocketOptions.None;

                await client.ConnectAsync(host, port, secureSocketOptions);

                if (!string.IsNullOrEmpty(username) && string.IsNullOrEmpty(password))
                {
                    throw new InvalidOperationException("SMTP configuration is invalid: 'Smtp:Password' must be provided when 'Smtp:Username' is set.");
                }

                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                {
                    await client.AuthenticateAsync(username, password);
                }

                await client.SendAsync(mail);
                await client.DisconnectAsync(true);

                _logger.LogInformation("Email sent to {To} via SMTP {Host}:{Port}", to, host, port);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {To} via SMTP {Host}:{Port}", to, host, port);
                throw;
            }
        }
    }
}
