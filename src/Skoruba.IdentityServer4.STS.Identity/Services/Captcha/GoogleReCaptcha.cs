using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Skoruba.IdentityServer4.STS.Identity.Services.Captcha.Dto;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.STS.Identity.Services.Captcha
{
    [ExcludeFromCodeCoverage]
    public class GoogleReCaptcha : ICaptchaService
    {
        private readonly IConfiguration _configuration;

        public GoogleReCaptcha(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<(bool, string)> Validate(string captchaResponse)
        {
            var reCaptchaSecretKey = _configuration["CaptchaConfiguration:SecretKey"];
            var googleReCaptchaUri = _configuration["CaptchaConfiguration:Uri"];
            var reCaptchaSiteKey = _configuration["CaptchaConfiguration:SiteKey"];
            var googlesiteScriptUri = _configuration["CaptchaConfiguration:SiteScriptUri"];

            if (string.IsNullOrEmpty(googleReCaptchaUri) || 
                string.IsNullOrEmpty(reCaptchaSecretKey) ||
                string.IsNullOrEmpty(reCaptchaSiteKey) ||
                string.IsNullOrEmpty(googlesiteScriptUri))
            {
                return (false, "Invalida reCaptchaV2 configuration.");
            }

            if (string.IsNullOrEmpty(captchaResponse))
            {
                return (false, "Invalida reCaptchaV2 client code.");
            }

            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"secret", reCaptchaSecretKey },
                {"response", captchaResponse }
            });

            var httpClient = new HttpClient();

            HttpResponseMessage response = null;

            try
            {
                response = await httpClient.PostAsync(googleReCaptchaUri, content);
            }
            catch (Exception ex)
            {
                return (false, "Invalid reCaptchaV2 response. | " + googleReCaptchaUri + " | " + ex.Message);
            }

            if (response is null)
            {
                return (false, "Invalid HTTP reCaptchaV2 response.");
            }

            var responseStr = JsonConvert.SerializeObject(response);

            if (!response.IsSuccessStatusCode)
            {
                return (false, "Invalid reCaptchaV2 response status code: " + response.StatusCode + " | " + responseStr);
            }

            var resultStr = await response.Content.ReadAsStringAsync();
            GoogleReCaptchaResponseDto result = null;

            try
            {
                result = JsonConvert.DeserializeObject<GoogleReCaptchaResponseDto>(resultStr);
                if (result is null)
                {
                    var errorMessage = "Invalid reCaptchaV2 response. | \"" + resultStr + "\"";
                    return (false, errorMessage);
                }
            }
            catch (Exception ex)
            {
                var errorMessage = "Invalid reCaptchaV2 response. | " + ex.Message + " | \"" + resultStr + "\"";
                return (false, errorMessage);
            }

            if (!result.Success)
            {
                var errorMessage = "reCaptchaV2 fail. | ";
                if (result.ErrorCodes is null || result.ErrorCodes.Length == 0)
                {
                    errorMessage += string.Join("| ", resultStr);
                }
                else
                {
                    errorMessage += string.Join("| ", result.ErrorCodes);
                }
                return (false, errorMessage);
            }

            return (true, string.Empty);
        }
    }
}
