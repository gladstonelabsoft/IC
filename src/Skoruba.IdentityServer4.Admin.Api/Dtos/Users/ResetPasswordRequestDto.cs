
using System.ComponentModel.DataAnnotations;

namespace Skoruba.IdentityServer4.Admin.Api.Dtos.Users
{
    public class ResetPasswordRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
