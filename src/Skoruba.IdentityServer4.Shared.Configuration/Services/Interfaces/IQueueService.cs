using Skoruba.IdentityServer4.Shared.Configuration.Configuration.Queue;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Shared.Configuration.Services.Interfaces
{
    public interface IQueueService
    {
        Task Send<T>(
            QueueMessage<T> queueMessage);
    }
}
