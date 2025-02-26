using Skoruba.IdentityServer4.Shared.Configuration.Configuration.HttpRequestService;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Shared.Configuration.Services.Interfaces
{
    public interface IHttpRequestService
    {
        Task<LabsoftHttpResponse> GetAsync(
            string requestUri,
            string token);

        Task<LabsoftHttpResponse> PostAsync(
            string requestUri,
            string objectBody);

        Task<LabsoftHttpResponse> PostAsync(
            string requestUri,
            string objectBody,
            string accessToken,
            string company = "");

        Task<LabsoftHttpResponse> PostFormAsync(
            string requestUri,
            string clientId,
            string scope,
            string clientSecret,
            string grantType,
            string resource);
    }
}
