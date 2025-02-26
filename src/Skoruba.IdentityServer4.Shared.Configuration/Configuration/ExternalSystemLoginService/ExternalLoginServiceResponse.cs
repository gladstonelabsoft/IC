using Microsoft.AspNetCore.Identity;
using System.Diagnostics.CodeAnalysis;

namespace Skoruba.IdentityServer4.Shared.Configuration.MyLIMSwebLoginService
{
    [ExcludeFromCodeCoverage]
    public class ExternalLoginServiceResponse
    {
        public readonly SignInResult SignInResult;
        public readonly ExternalLoginServiceTokenConfigResponse Response;

        public ExternalLoginServiceResponse(
            SignInResult signInResult,
            ExternalLoginServiceTokenConfigResponse response)
        {
            SignInResult = signInResult;
            Response = response;
        }
    }
}