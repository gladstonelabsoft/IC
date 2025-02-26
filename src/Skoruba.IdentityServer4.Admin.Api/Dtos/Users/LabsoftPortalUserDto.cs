namespace Skoruba.IdentityServer4.Admin.Api.Dtos.Users
{
    public class LabsoftPortalUserDto<TUserDto, TKey>
    {
        public TUserDto User { get; set;}
        public string Password { get; set;}
        public string ClaimNameValue { get; set;}
        public string ClaimPersonalCodeValue { get; set;}
        public string Company { get; set; }
    }
}
