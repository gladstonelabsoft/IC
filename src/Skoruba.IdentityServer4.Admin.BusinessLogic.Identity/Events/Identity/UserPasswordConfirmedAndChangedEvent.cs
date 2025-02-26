using Skoruba.AuditLogging.Events;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Events.Identity
{
    public class UserPasswordConfirmedAndChangedEvent : AuditEvent
    {
        public string UserName { get; set; }

        public UserPasswordConfirmedAndChangedEvent(string userName)
        {
            UserName = userName;
        }
    }
}