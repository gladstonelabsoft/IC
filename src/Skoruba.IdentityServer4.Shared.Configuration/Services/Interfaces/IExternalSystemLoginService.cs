using Skoruba.IdentityServer4.Shared.Configuration.MyLIMSwebLoginService;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Shared.Configuration.Services.Interfaces
{
    public interface IExternalSystemLoginService
    {
        Task<ExternalLoginServiceResponse> ValidateLoginAsync(
            string username,
            string password,
            string requesterClient);

        string GetFormattedBaseUrl(string RequesterClient);
    }
}
