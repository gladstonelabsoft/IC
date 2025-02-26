namespace Skoruba.IdentityServer4.STS.Identity.Controllers.Dtos
{
    public class RequestPasswordResetDto
    {
        public string UsernameOrEmail;

        public RequestPasswordResetDto(string login)
        {
            UsernameOrEmail = login;
        }
    }
}
