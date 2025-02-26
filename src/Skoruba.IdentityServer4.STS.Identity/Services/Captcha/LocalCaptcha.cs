using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.STS.Identity.Services.Captcha
{
    [ExcludeFromCodeCoverage]
    public class LocalCaptcha : ICaptchaService
    {
        public Task<(bool, string)> Validate(string captchaResponse)
        {
            return Task.FromResult((true, string.Empty));
        }
    }
}
