using System.Diagnostics.CodeAnalysis;

namespace Skoruba.IdentityServer4.Shared.Configuration.MyLIMSwebLoginService
{
    [ExcludeFromCodeCoverage]
    public class ExternalLoginServiceTokenConfigResponse
    {
        public readonly string Authority;
        public readonly string ClientId;
        public readonly string ClientSecret;
        public readonly string Scope;

        public ExternalLoginServiceTokenConfigResponse(
            string authority,
            string clientId,
            string clientSecret,
            string scope)
        {
            Authority = authority;
            ClientId = clientId;
            ClientSecret = clientSecret;
            Scope = scope;
        }
    }
}