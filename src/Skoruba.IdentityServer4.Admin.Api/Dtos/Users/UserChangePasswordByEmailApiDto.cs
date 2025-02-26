namespace Skoruba.IdentityServer4.Admin.Api.Dtos.Users
{
    public class UserChangePasswordByEmailApiDto
    {
        public string Email { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
