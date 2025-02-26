using Labsoft.Notification.Center.Contracts;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Skoruba.IdentityServer4.Shared.Configuration.Configuration.Email;
using Skoruba.IdentityServer4.Shared.Configuration.Configuration.Queue;
using Skoruba.IdentityServer4.Shared.Configuration.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Shared.Configuration.Email
{
    public class NotificationCenterEmailSender : IEmailSender
    {
        private readonly ILogger<LogEmailSender> _logger;
        private readonly NotificationCenterConfiguration _notificationCenterConfiguration;
        private readonly IQueueService _queueService;

        public NotificationCenterEmailSender(
            ILogger<LogEmailSender> logger, 
            NotificationCenterConfiguration notificationCenterConfiguration,
            IQueueService queueService)
        {
            _logger = logger;
            _notificationCenterConfiguration = notificationCenterConfiguration;
            _queueService = queueService;
        }

        public async Task SendEmailAsync(
            string email, 
            string subject, 
            string htmlMessage)
        {
            try
            {
                var emailNotificationToSend = new EmailNotificationToSend
                {
                    FromEmailAddress = _notificationCenterConfiguration.FromEmailAddress,
                    ToEmailAddress = email,
                    Subject = subject,
                    TextContent = string.Empty,
                    HtmlContent = htmlMessage
                };

                var queueMessage = new QueueMessage<EmailNotificationToSend>
                {
                    Body = emailNotificationToSend
                };

                await _queueService.Send(queueMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception {ex} during sending email: {email}, subject: {subject}");
                throw;
            }
        }
    }
}