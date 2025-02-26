
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using Skoruba.IdentityServer4.Shared.Configuration.Configuration.Email;

namespace Skoruba.IdentityServer4.Shared.Configuration.Email
{
    public class SendGridEmailSender : IEmailSender
    {
        private readonly ILogger<SendGridEmailSender> _logger;
        private readonly SendGridConfiguration _configuration;
        private readonly ISendGridClient _client;

        public SendGridEmailSender(ILogger<SendGridEmailSender> logger, SendGridConfiguration configuration, ISendGridClient client)
        {
            _logger = logger;
            _configuration = configuration;
            _client = client;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                var message = MailHelper.CreateSingleEmail(
                    new EmailAddress(_configuration.SourceEmail, _configuration.SourceName),
                    new EmailAddress(email),
                    subject,
                    null,
                    htmlMessage
                );

                message.SetClickTracking(_configuration.EnableClickTracking, _configuration.EnableClickTracking);

                var response = await _client.SendEmailAsync(message);
                
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Email sent successfully to {EmailAddress}", email);
                }
                else
                {
                    _logger.LogError("Failed to send email to {EmailAddress}. Status code: {StatusCode}", 
                        email, response.StatusCode);
                    throw new Exception($"Failed to send email. Status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {EmailAddress}", email);
                throw new Exception("Failed to send email due to an internal error", ex);
            }
        }
    }
}
