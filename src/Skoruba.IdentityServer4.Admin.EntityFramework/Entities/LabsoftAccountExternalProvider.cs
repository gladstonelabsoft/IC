using System;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Entities
{
    public class LabsoftAccountExternalProvider
    {
        public Guid Id { get; set; }
        public string AccountDomain { get; set; }
        public string ExternalProviderName { get; set; }
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string SecretId { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public bool Enabled { get; set; }
    }
}
