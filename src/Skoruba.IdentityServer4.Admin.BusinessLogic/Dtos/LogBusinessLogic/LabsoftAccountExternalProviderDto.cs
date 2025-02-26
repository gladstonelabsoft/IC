namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.LogBusinessLogic
{
    public class LabsoftAccountExternalProviderDto
    {
        public LabsoftAccountExternalProviderDto(
            string accountDomain, 
            string externalProviderName, 
            string tenantId, 
            string clientId, 
            string secretId)
        {
            AccountDomain = accountDomain;
            ExternalProviderName = externalProviderName;
            TenantId = tenantId;
            ClientId = clientId;
            SecretId = secretId;
        }

        public string AccountDomain { get; }
        public string ExternalProviderName { get; }
        public string TenantId { get; }
        public string ClientId { get; }
        public string SecretId { get; }
    }
}
