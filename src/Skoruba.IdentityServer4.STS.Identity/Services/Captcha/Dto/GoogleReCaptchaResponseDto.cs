using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace Skoruba.IdentityServer4.STS.Identity.Services.Captcha.Dto
{
    [ExcludeFromCodeCoverage]
    public class GoogleReCaptchaResponseDto
    {
        public bool Success { get; set; }
        
        [JsonProperty("error-codes")]
        public string[] ErrorCodes { get; set; }
    }
}
