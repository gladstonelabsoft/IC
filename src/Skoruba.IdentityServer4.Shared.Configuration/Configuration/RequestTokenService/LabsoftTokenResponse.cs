using System.Diagnostics.CodeAnalysis;

namespace Skoruba.IdentityServer4.Shared.Configuration.Configuration.TokenService
{
    [ExcludeFromCodeCoverage]
    public class LabsoftTokenResponse
    {
        public readonly string AccessToken;
        public readonly string ErrorMessage;

        public LabsoftTokenResponse(
            string accessToken,
            string errorMessage)
        {
            AccessToken = accessToken;
            ErrorMessage = errorMessage;
        }
    }
}