using System.Diagnostics.CodeAnalysis;

namespace Skoruba.IdentityServer4.Shared.Configuration.Configuration.TokenService
{
    [ExcludeFromCodeCoverage]
    public class LabsoftTokenRequest
    {
        public readonly string Authority;
        public readonly string ClientId;
        public readonly string ClientSecret;
        public readonly string Scope;

        public LabsoftTokenRequest(
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