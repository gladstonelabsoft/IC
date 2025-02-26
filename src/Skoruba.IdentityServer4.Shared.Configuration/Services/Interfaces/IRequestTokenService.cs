using Skoruba.IdentityServer4.Shared.Configuration.Configuration.TokenService;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Shared.Configuration.Services.Interfaces
{
    public interface IRequestTokenService
    {
        Task<LabsoftTokenResponse> GetTokenByClientCredentialsAsync(LabsoftTokenRequest labsoftTokenRequest);
    }
}
