using System.Diagnostics.CodeAnalysis;

namespace Skoruba.IdentityServer4.Shared.Configuration.Configuration.Queue
{
    [ExcludeFromCodeCoverage]
    public class QueueMessage<T>
    {
        public T Body { get; set; }
    }
}
