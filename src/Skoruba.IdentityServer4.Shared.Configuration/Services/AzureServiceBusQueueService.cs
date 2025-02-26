using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using Skoruba.IdentityServer4.Shared.Configuration.Configuration.Email;
using Skoruba.IdentityServer4.Shared.Configuration.Configuration.Queue;
using Skoruba.IdentityServer4.Shared.Configuration.Services.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Shared.Configuration.Services
{
    [ExcludeFromCodeCoverage]
    public class AzureServiceBusQueueService : IQueueService
    {
        private readonly NotificationCenterConfiguration _notificationCenterConfiguration;

        public AzureServiceBusQueueService(NotificationCenterConfiguration notificationCenterConfiguration)
        {
            _notificationCenterConfiguration = notificationCenterConfiguration;
        }

        public async Task Send<T>(
            QueueMessage<T> queueMessage)
        {
            var connectionString = _notificationCenterConfiguration.ServiceBus.ConnectionString ?? string.Empty;
            var queueName = _notificationCenterConfiguration.ServiceBus.QueueName ?? string.Empty;
            var topicClient = new TopicClient(connectionString, queueName);

            string messageBody = JsonConvert.SerializeObject(queueMessage.Body);
            var message = new Message(Encoding.UTF8.GetBytes(messageBody));
            await topicClient.SendAsync(message);
            await topicClient.CloseAsync();
        }
    }
}
