using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Skoruba.IdentityServer4.Shared.Configuration.Configuration.HttpRequestService;
using Skoruba.IdentityServer4.Shared.Configuration.Configuration.MyLIMSwebLoginService;
using Skoruba.IdentityServer4.Shared.Configuration.MyLIMSwebLoginService;
using Skoruba.IdentityServer4.Shared.Configuration.Services.Interfaces;
using System.Net;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Shared.Configuration.Services
{
    public class MyLIMSwebLoginService : IExternalSystemLoginService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpRequestService _httpRequestService;

        public MyLIMSwebLoginService(
            IConfiguration configuration,
            IHttpRequestService httpRequestService)
        {
            _configuration = configuration;
            _httpRequestService = httpRequestService;
        }

        public async Task<ExternalLoginServiceResponse> ValidateLoginAsync(string username, string password, string requesterClient)
        {
            var requestUri = BuildMyLIMSwebValidateLoginRequestUri(requesterClient);
            var requestBody = CreateMyLIMSwebValidateLoginRequestBody(username, password);

            var result = await _httpRequestService.PostAsync(requestUri, requestBody);

            return DetermineSignInResult(result);
        }

        private string BuildMyLIMSwebValidateLoginRequestUri(string requesterClient)
        {
            var baseUrl = GetFormattedBaseUrl(requesterClient);
            var validateLoginUri = GetConfigurationValue("myLIMSweb:ValidateLoginUri");

            return $"{baseUrl}{validateLoginUri}";
        }

        public string GetFormattedBaseUrl(string requesterClient)
        {
            var baseUrl = GetConfigurationValue("myLIMSweb:BaseUrl");
            var baseUrlWithTrailingSlash = EnsureTrailingSlash(baseUrl);

            return baseUrlWithTrailingSlash.Replace("{company}", requesterClient);
        }

        private string GetConfigurationValue(string key)
        {
            return _configuration[key] ?? string.Empty;
        }

        private string CreateMyLIMSwebValidateLoginRequestBody(string username, string password)
        {
            var externalLoginValidation = new ExternalLoginValidation
            {
                Username = username,
                Password = password
            };

            return JsonConvert.SerializeObject(externalLoginValidation);
        }

        private ExternalLoginServiceResponse DetermineSignInResult(LabsoftHttpResponse response)
        {
            if (response.IsSuccessStatusCode)
            {
                var tokenConfigResponse = JsonConvert.DeserializeObject<ExternalLoginServiceTokenConfigResponse>(response.Content);

                return new ExternalLoginServiceResponse(
                    signInResult: SignInResult.Success,
                    response: tokenConfigResponse);
            }

            if (response.StatusCode == (int)HttpStatusCode.Unauthorized)
            {
                return new ExternalLoginServiceResponse(
                    signInResult: SignInResult.NotAllowed,
                    response: null);
            }

            return new ExternalLoginServiceResponse(
                signInResult: SignInResult.Failed,
                response: null);
        }

        public static string EnsureTrailingSlash(string input)
        {
            if (!input.EndsWith('/'))
            {
                return $"{input}/";
            }
            return input;
        }
    }
}
