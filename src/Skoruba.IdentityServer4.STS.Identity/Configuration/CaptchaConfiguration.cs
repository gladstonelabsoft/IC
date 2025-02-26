namespace Skoruba.IdentityServer4.STS.Identity.Configuration
{
    public class CaptchaConfiguration
    {
        public string Provider { get; set; }
        public string Uri { get; set; }
        public string SiteKey { get; set; }
        public string SecretKey { get; set; }
        public string SiteScriptUri { get; set; }
    }
}
