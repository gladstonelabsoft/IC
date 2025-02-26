using IdentityModel.Client;
using Skoruba.IdentityServer4.Shared.Configuration.Configuration.TokenService;
using Skoruba.IdentityServer4.Shared.Configuration.Services.Interfaces;
using System.Net.Http;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Shared.Configuration.Services
{
    public class RequestTokenService : IRequestTokenService
    {
        private readonly HttpClient _httpClient;

        public RequestTokenService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<LabsoftTokenResponse> GetTokenByClientCredentialsAsync(LabsoftTokenRequest labsoftTokenRequest)
        {
            var discoveryDocument = await _httpClient.GetDiscoveryDocumentAsync(labsoftTokenRequest.Authority);
            if (discoveryDocument.IsError)
            {
                return new LabsoftTokenResponse(
                    accessToken: null,
                    errorMessage: $"Discovery Document Error: {discoveryDocument.Error}");
            }

            var tokenResponse = await _httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = discoveryDocument.TokenEndpoint,
                ClientId = labsoftTokenRequest.ClientId,
                ClientSecret = labsoftTokenRequest.ClientSecret,
                Scope = labsoftTokenRequest.Scope
            });

            if (tokenResponse.IsError)
            {
                return new LabsoftTokenResponse(
                    accessToken: null,
                    errorMessage: $"Token Response Error: {tokenResponse.Error}");
            }

            return new LabsoftTokenResponse(
                accessToken: tokenResponse.AccessToken,
                errorMessage: null);
        }
    }
}