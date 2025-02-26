using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.STS.Identity.Services.Captcha
{
    public interface ICaptchaService 
    {
        Task<(bool, string)> Validate(string captchaResponse);
    }
}
