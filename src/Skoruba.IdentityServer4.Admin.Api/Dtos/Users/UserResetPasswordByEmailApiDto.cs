namespace Skoruba.IdentityServer4.Admin.Api.Dtos.Users
{
    public class UserResetPasswordByEmailApiDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Code { get; set; }
    }
}
