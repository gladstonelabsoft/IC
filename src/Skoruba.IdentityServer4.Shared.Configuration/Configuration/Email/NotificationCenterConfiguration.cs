namespace Skoruba.IdentityServer4.Shared.Configuration.Configuration.Email
{
    public class NotificationCenterConfiguration
    {
        public string FromEmailAddress { get; set; }
        public ServiceBusConfiguration ServiceBus { get; set; }
    }
}