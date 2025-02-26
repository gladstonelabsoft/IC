using Skoruba.AuditLogging.Events;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Events.Identity
{
    public class UserPasswordResetedEvent : AuditEvent
    {
        public string UserName { get; set; }

        public UserPasswordResetedEvent(string userName)
        {
            UserName = userName;
        }
    }
}