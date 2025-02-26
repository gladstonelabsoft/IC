using AutoMapper;
using Humanizer;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Skoruba.IdentityServer4.Admin.Api.Configuration.Constants;
using Skoruba.IdentityServer4.Admin.Api.Dtos.Roles;
using Skoruba.IdentityServer4.Admin.Api.Dtos.Users;
using Skoruba.IdentityServer4.Admin.Api.ExceptionHandling;
using Skoruba.IdentityServer4.Admin.Api.Helpers.Localization;
using Skoruba.IdentityServer4.Admin.Api.Resources;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.ExceptionHandling;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Admin.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [TypeFilter(typeof(ControllerExceptionFilterAttribute))]
    [Produces("application/json", "application/problem+json")]
    public class UsersController<TUserDto, TRoleDto, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
            TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
            TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto> : ControllerBase
        where TUserDto : UserDto<TKey>, new()
        where TRoleDto : RoleDto<TKey>, new()
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>
        where TUserRole : IdentityUserRole<TKey>
        where TUserLogin : IdentityUserLogin<TKey>
        where TRoleClaim : IdentityRoleClaim<TKey>
        where TUserToken : IdentityUserToken<TKey>
        where TUsersDto : UsersDto<TUserDto, TKey>
        where TRolesDto : RolesDto<TRoleDto, TKey>
        where TUserRolesDto : UserRolesDto<TRoleDto, TKey>
        where TUserClaimsDto : UserClaimsDto<TUserClaimDto, TKey>, new()
        where TUserProviderDto : UserProviderDto<TKey>
        where TUserProvidersDto : UserProvidersDto<TUserProviderDto, TKey>
        where TUserChangePasswordDto : UserChangePasswordDto<TKey>
        where TRoleClaimsDto : RoleClaimsDto<TRoleClaimDto, TKey>
        where TUserClaimDto : UserClaimDto<TKey>
        where TRoleClaimDto : RoleClaimDto<TKey>
    {
        private readonly IIdentityService<TUserDto, TRoleDto, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
            TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
            TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto> _identityService;
        private readonly IGenericControllerLocalizer<UsersController<TUserDto, TRoleDto, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
            TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
            TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto>> _localizer;

        private readonly IMapper _mapper;
        private readonly IApiErrorResources _errorResources;
        private readonly UserManager<TUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<UsersController<TUserDto, TRoleDto, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
            TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
            TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto>> _logger;
        private readonly IConfiguration _configuration;
        private const string DOMAIN_NAME = "Skoruba.IdentityServer4.Admin.Api";

        public UsersController(IIdentityService<TUserDto, TRoleDto, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
                TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
                TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto> identityService,
            IGenericControllerLocalizer<UsersController<TUserDto, TRoleDto, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
                TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
                TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto>> localizer, IMapper mapper, IApiErrorResources errorResources,
                UserManager<TUser> userManager, IEmailSender emailSender, ILogger<UsersController<TUserDto, TRoleDto, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
                TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
                TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto>> logger,
                IConfiguration configuration)
        {
            _identityService = identityService;
            _localizer = localizer;
            _mapper = mapper;
            _errorResources = errorResources;
            _userManager = userManager;
            _emailSender = emailSender;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet("{id}")]
        [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
        public async Task<ActionResult<TUserDto>> Get(TKey id)
        {
            var user = await _identityService.GetUserAsync(id.ToString());

            return Ok(user);
        }

        [HttpGet("email/{email}")]
        [Authorize]
        public async Task<ActionResult<TUserDto>> GetByEmail(string email)
        {
            var user = await _identityService.GetUserByEmailAsync(email);

            if(user is null)
            {
                return NoContent();
            }

            return Ok(user);
        }

        [HttpGet("userName/{userName}")]
        [Authorize]
        public async Task<ActionResult<TUserDto>> GetByUserName(string userName)
        {
            var user = await _identityService.GetUserByUserNameAsync(userName);

            if (user is null)
            {
                return NoContent();
            }

            return Ok(user);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<TUsersDto>> Get(string searchText, int page = 1, int pageSize = 10)
        {
            var usersDto = await _identityService.GetUsersAsync(searchText, page, pageSize);

            return Ok(usersDto);
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [Authorize]
        public async Task<ActionResult<TUserDto>> Post([FromBody] TUserDto user)
        {
            if (!EqualityComparer<TKey>.Default.Equals(user.Id, default))
            {
                return BadRequest(_errorResources.CannotSetId());
            }

            var (identityResult, userId) = await _identityService.CreateUserAsync(user);
            var createdUser = await _identityService.GetUserAsync(userId.ToString());

            return CreatedAtAction(nameof(Get), new { id = createdUser.Id }, createdUser);
        }

        [HttpPost("LabsoftPortal")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [Authorize]
        public async Task<ActionResult<TUserDto>> AddPortalUser(
            [FromBody] LabsoftPortalUserDto<TUserDto, TKey> labsoftPortalUser)
        {
            try
            {
                var userDto = labsoftPortalUser.User;
                var company = labsoftPortalUser.Company;

                if (!EqualityComparer<TKey>.Default.Equals(userDto.Id, default))
                {
                    throw new LabsoftGenericException(
                        domainName: DOMAIN_NAME,
                        errorDescription: _errorResources.CannotSetId().Description);
                }

                var (identityResult, userId) = await _identityService.CreateUserAsync(userDto);

                var password = new UserChangePasswordApiDto<TKey>()
                {
                    UserId = userId,
                    Password = labsoftPortalUser.Password,
                    ConfirmPassword = labsoftPortalUser.Password
                };
                var userChangePasswordDto = _mapper.Map<TUserChangePasswordDto>(password);
                await _identityService.UserChangePasswordAsync(userChangePasswordDto);

                var claimName = new UserClaimApiDto<TKey>()
                {
                    UserId = userId,
                    ClaimType = "name",
                    ClaimValue = labsoftPortalUser.ClaimNameValue
                };
                var claimNameDto = _mapper.Map<TUserClaimsDto>(claimName);
                if (await AddUserClaim(claimNameDto) is false)
                {
                    throw new LabsoftGenericException(
                        domainName: DOMAIN_NAME,
                        errorDescription: _errorResources.CannotSetId().Description);
                }

                var personalCodeName = new UserClaimApiDto<TKey>()
                {
                    UserId = userId,
                    ClaimType = "personal_code",
                    ClaimValue = labsoftPortalUser.ClaimPersonalCodeValue
                };
                var personalCodeNameDto = _mapper.Map<TUserClaimsDto>(personalCodeName);
                if (await AddUserClaim(personalCodeNameDto) is false)
                {
                    throw new LabsoftGenericException(
                        domainName: DOMAIN_NAME,
                        errorDescription: _errorResources.CannotSetId().Description);
                }

                var createdUser = await _identityService.GetUserAsync(userId.ToString());
                var userManager = await _userManager.FindByEmailAsync(userDto.Email);

                if(userManager is null)
                {
                    throw new LabsoftGenericException(
                        domainName: DOMAIN_NAME,
                        errorDescription: _errorResources.UserNotFoundByEmailAddress().Description);
                }

                await SendConfirmationEmail(
                    userManager,
                    company);

                return CreatedAtAction(nameof(Get), new { id = createdUser.Id }, createdUser);
            }
            catch(LabsoftGenericException ex)
            {
                _logger.LogError(ex, "Endpoint POST ended run with system error.");
                Log.CloseAndFlush();
                return StatusCode((int)HttpStatusCode.BadRequest, ex.Message);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Endpoint POST ended run with system error.");
                Log.CloseAndFlush();
                return StatusCode((int)HttpStatusCode.InternalServerError, $"Message error: some server error has happened. Call the system administrator.");
            }
        }

        private async Task<bool> AddUserClaim(TUserClaimsDto userClaimDto)
        {
            userClaimDto = _mapper.Map<TUserClaimsDto>(userClaimDto);
            if (!userClaimDto.ClaimId.Equals(default))
            {
                return false;
            }
            await _identityService.CreateUserClaimsAsync(userClaimDto);
            return true;
        }

        private async Task SendConfirmationEmail(
            TUser user,
            string company)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = _configuration["SendConfirmationEmailCallBackUrl"] ?? string.Empty;
            callbackUrl += $"?userId={user.Id}&code={code}&company={company}&isLabsoftPortal=true";
            var subject = ApiTranslateResource.ConfirmEmailTitle;
            var message = ApiTranslateResource.ConfirmEmailBody.FormatWith(HtmlEncoder.Default.Encode(callbackUrl));
            await _emailSender.SendEmailAsync(
                email: user.Email,
                subject: subject,
                htmlMessage: message);
        }

        [HttpPut]
        [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
        public async Task<IActionResult> Put([FromBody] TUserDto user)
        {
            await _identityService.GetUserAsync(user.Id.ToString());
            await _identityService.UpdateUserAsync(user);

            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(TKey id)
        {
            if (IsDeleteForbidden(id))
                return StatusCode((int)System.Net.HttpStatusCode.Forbidden);

            var user = new TUserDto { Id = id };

            await _identityService.GetUserAsync(user.Id.ToString());
            await _identityService.DeleteUserAsync(user.Id.ToString(), user);

            return Ok();
        }

        private bool IsDeleteForbidden(TKey id)
        {
            var userId = User.FindFirst(JwtClaimTypes.Subject);

            return userId == null ? false : userId.Value == id.ToString();
        }

        [HttpGet("{id}/Roles")]
        [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
        public async Task<ActionResult<UserRolesApiDto<TRoleDto>>> GetUserRoles(TKey id, int page = 1, int pageSize = 10)
        {
            var userRoles = await _identityService.GetUserRolesAsync(id.ToString(), page, pageSize);
            var userRolesApiDto = _mapper.Map<UserRolesApiDto<TRoleDto>>(userRoles);

            return Ok(userRolesApiDto);
        }

        [HttpPost("Roles")]
        [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
        public async Task<IActionResult> PostUserRoles([FromBody] UserRoleApiDto<TKey> role)
        {
            var userRolesDto = _mapper.Map<TUserRolesDto>(role);

            await _identityService.GetUserAsync(userRolesDto.UserId.ToString());
            await _identityService.GetRoleAsync(userRolesDto.RoleId.ToString());

            await _identityService.CreateUserRoleAsync(userRolesDto);

            return Ok();
        }

        [HttpDelete("Roles")]
        [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
        public async Task<IActionResult> DeleteUserRoles([FromBody] UserRoleApiDto<TKey> role)
        {
            var userRolesDto = _mapper.Map<TUserRolesDto>(role);

            await _identityService.GetUserAsync(userRolesDto.UserId.ToString());
            await _identityService.GetRoleAsync(userRolesDto.RoleId.ToString());

            await _identityService.DeleteUserRoleAsync(userRolesDto);

            return Ok();
        }

        [HttpGet("{id}/Claims")]
        [Authorize]
        public async Task<ActionResult<UserClaimsApiDto<TKey>>> GetUserClaims(TKey id, int page = 1, int pageSize = 10)
        {
            var claims = await _identityService.GetUserClaimsAsync(id.ToString(), page, pageSize);

            var userClaimsApiDto = _mapper.Map<UserClaimsApiDto<TKey>>(claims);

            return Ok(userClaimsApiDto);
        }

        [HttpPost("Claims")]
        [Authorize]
        public async Task<IActionResult> PostUserClaims([FromBody] UserClaimApiDto<TKey> claim)
        {
            var userClaimDto = _mapper.Map<TUserClaimsDto>(claim);

            if (!userClaimDto.ClaimId.Equals(default))
            {
                return BadRequest(_errorResources.CannotSetId());
            }

            await _identityService.CreateUserClaimsAsync(userClaimDto);

            return Ok();
        }

        [HttpPut("Claims")]
        [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
        public async Task<IActionResult> PutUserClaims([FromBody] UserClaimApiDto<TKey> claim)
        {
            var userClaimDto = _mapper.Map<TUserClaimsDto>(claim);

            await _identityService.GetUserClaimAsync(userClaimDto.UserId.ToString(), userClaimDto.ClaimId);
            await _identityService.UpdateUserClaimsAsync(userClaimDto);

            return Ok();
        }

        [HttpDelete("{id}/Claims")]
        [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
        public async Task<IActionResult> DeleteUserClaims([FromRoute] TKey id, int claimId)
        {
            var userClaimsDto = new TUserClaimsDto
            {
                ClaimId = claimId,
                UserId = id
            };

            await _identityService.GetUserClaimAsync(id.ToString(), claimId);
            await _identityService.DeleteUserClaimAsync(userClaimsDto);

            return Ok();
        }

        [HttpGet("{id}/Providers")]
        [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
        public async Task<ActionResult<UserProvidersApiDto<TKey>>> GetUserProviders(TKey id)
        {
            var userProvidersDto = await _identityService.GetUserProvidersAsync(id.ToString());
            var userProvidersApiDto = _mapper.Map<UserProvidersApiDto<TKey>>(userProvidersDto);

            return Ok(userProvidersApiDto);
        }

        [HttpDelete("Providers")]
        [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
        public async Task<IActionResult> DeleteUserProviders([FromBody] UserProviderDeleteApiDto<TKey> provider)
        {
            var providerDto = _mapper.Map<TUserProviderDto>(provider);

            await _identityService.GetUserProviderAsync(providerDto.UserId.ToString(), providerDto.ProviderKey);
            await _identityService.DeleteUserProvidersAsync(providerDto);

            return Ok();
        }

        [HttpPost("ChangePassword")]
        [Authorize]
        public async Task<IActionResult> PostChangePassword([FromBody] UserChangePasswordApiDto<TKey> password)
        {
            var userChangePasswordDto = _mapper.Map<TUserChangePasswordDto>(password);

            await _identityService.UserChangePasswordAsync(userChangePasswordDto);

            return Ok();
        }

        [HttpPut("ResetTokenByEmail")]
        [ProducesResponseType(statusCode: (int)HttpStatusCode.OK, type: typeof(void))]
        [ProducesResponseType(statusCode: (int)HttpStatusCode.BadRequest, type: typeof(void))]
        [ProducesResponseType(statusCode: (int)HttpStatusCode.InternalServerError, type: typeof(void))]
        [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
        public async Task<IActionResult> PutResetTokenByEmail([FromBody] UserResetTokenByEmailApiDto data)
        {
            var code = await _identityService.UserResetTokenByEmailAsync(data.Email);

            return Ok(code);
        }

        [HttpPut("ResetPasswordByEmail")]
        [ProducesResponseType(statusCode: (int)HttpStatusCode.OK, type: typeof(void))]
        [ProducesResponseType(statusCode: (int)HttpStatusCode.BadRequest, type: typeof(void))]
        [ProducesResponseType(statusCode: (int)HttpStatusCode.InternalServerError, type: typeof(void))]
        [AllowAnonymous]
        public async Task<IActionResult> PutResetPasswordByEmail([FromBody] UserResetPasswordByEmailApiDto data)
        {
            try
            {
                await _identityService.UserResetPasswordByEmailAsync(data.Email, data.Password, data.Code);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error reseting password by email ({0}): {1} {2}", data.Email, ex.Message, ex is UserFriendlyViewException ? string.Join(". ", ((UserFriendlyViewException)ex).ErrorMessages.Select(x => x.ErrorMessage).ToList()) : "");
            }

            return Ok();
        }

        [HttpPut("ChangePasswordByEmail")]
        [ProducesResponseType(statusCode: (int)HttpStatusCode.OK, type: typeof(void))]
        [ProducesResponseType(statusCode: (int)HttpStatusCode.BadRequest, type: typeof(void))]
        [ProducesResponseType(statusCode: (int)HttpStatusCode.InternalServerError, type: typeof(void))]
        [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
        public async Task<IActionResult> PutChangePasswordByEmail([FromBody] UserChangePasswordByEmailApiDto data)
        {
            await _identityService.UserChangePasswordByEmailAsync(data.Email, data.CurrentPassword, data.NewPassword);

            return Ok();
        }

        [HttpPut("ForgotPasswordByEmail")]
        [ProducesResponseType(statusCode: (int)HttpStatusCode.OK, type: typeof(void))]
        [ProducesResponseType(statusCode: (int)HttpStatusCode.BadRequest, type: typeof(void))]
        [ProducesResponseType(statusCode: (int)HttpStatusCode.InternalServerError, type: typeof(void))]
        [AllowAnonymous]
        public async Task<IActionResult> PutForgotPasswordByEmail([FromBody] UserForgotPasswordByEmailApiDto data)
        {
            TUser user = null;
            try
            {
                user = await _userManager.FindByEmailAsync(data.Email);
            }
            catch (Exception ex)
            {
                // in case of multiple users with the same email this method would throw and reveal that the email is registered
                _logger.LogError("Error retrieving user by email ({0}) for forgot password by email functionality: {1}", data.Email, ex.Message);
                user = null;
            }

            if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
            {
                // Don't reveal that the user does not exist
                return Ok();
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var param = new Dictionary<string, string>()
            {
                { "email", user.Email.ToString() },
                { "code", code }
            };

            Uri callbackUrl = null;
            try
            {
                data.URL = data.URL.Replace("/#/", "/%23/");
                callbackUrl = new Uri(QueryHelpers.AddQueryString(data.URL, param));
            }
            catch (UriFormatException ex)
            {
                _logger.LogError("Error parsing uri ({0}) by email ({1}) for forgot password by email functionality: {2}", data.URL, data.Email, ex.Message);
                return BadRequest();
            }

            Thread.CurrentThread.CurrentCulture = new CultureInfo(data.Culture);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(data.Culture);

            string htmlMessage;

            if (string.IsNullOrEmpty(data.EmailBody))
            {
                htmlMessage = ApiTranslateResource.ResetPasswordBody.FormatWith(
                            HtmlEncoder.Default.Encode(callbackUrl.AbsoluteUri.Replace("/%23/", "/#/")));
            }
            else
            {
                htmlMessage = data.EmailBody.Replace("@url", $"{callbackUrl.AbsoluteUri.Replace("/%23/", "/#/")}");
            }

            await _emailSender.SendEmailAsync(
                email: user.Email,
                subject: ApiTranslateResource.ResetPasswordTitle,
                htmlMessage: htmlMessage);

            return Ok();
        }

        [HttpGet("{id}/RoleClaims")]
        [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
        public async Task<ActionResult<RoleClaimsApiDto<TKey>>> GetRoleClaims(TKey id, string claimSearchText, int page = 1, int pageSize = 10)
		{
			var roleClaimsDto = await _identityService.GetUserRoleClaimsAsync(id.ToString(), claimSearchText, page, pageSize);
			var roleClaimsApiDto = _mapper.Map<RoleClaimsApiDto<TKey>>(roleClaimsDto);

			return Ok(roleClaimsApiDto);
		}

        [HttpGet("ClaimType/{claimType}/ClaimValue/{claimValue}")]
        [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
        public async Task<ActionResult<TUsersDto>> GetClaimUsers(string claimType, string claimValue, int page = 1, int pageSize = 10)
        {
            var usersDto = await _identityService.GetClaimUsersAsync(claimType, claimValue, page, pageSize);

            return Ok(usersDto);
        }

        [HttpGet("ClaimType/{claimType}")]
        [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
        public async Task<ActionResult<TUsersDto>> GetClaimUsers(string claimType, int page = 1, int pageSize = 10)
        {
            var usersDto = await _identityService.GetClaimUsersAsync(claimType, null, page, pageSize);

            return Ok(usersDto);
        }
    }
}