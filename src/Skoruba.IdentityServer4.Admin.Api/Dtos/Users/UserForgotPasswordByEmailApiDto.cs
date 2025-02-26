namespace Skoruba.IdentityServer4.Admin.Api.Dtos.Users
{
    public class UserForgotPasswordByEmailApiDto
    {
        public string Email { get; set; }
        public string Culture { get; set; }
        public string URL { get; set; }
        public string EmailBody { get; set; }
    }
}
