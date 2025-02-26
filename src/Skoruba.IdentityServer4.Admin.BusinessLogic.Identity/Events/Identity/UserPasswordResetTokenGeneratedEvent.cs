using Skoruba.AuditLogging.Events;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Events.Identity
{
    public class UserPasswordResetTokenGeneratedEvent : AuditEvent
    {
        public string UserName { get; set; }

        public UserPasswordResetTokenGeneratedEvent(string userName)
        {
            UserName = userName;
        }
    }
}