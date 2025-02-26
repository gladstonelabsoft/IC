using IdentityModel;
using IdentityServer4;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.LogBusinessLogic;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces;
using Skoruba.IdentityServer4.Shared.Configuration.Configuration.HttpRequestService;
using Skoruba.IdentityServer4.Shared.Configuration.Configuration.Identity;
using Skoruba.IdentityServer4.Shared.Configuration.Configuration.TokenService;
using Skoruba.IdentityServer4.Shared.Configuration.MyLIMSwebLoginService;
using Skoruba.IdentityServer4.Shared.Configuration.Services.Interfaces;
using Skoruba.IdentityServer4.Shared.Enums;
using Skoruba.IdentityServer4.STS.Identity.Configuration;
using Skoruba.IdentityServer4.STS.Identity.Controllers.Dtos;
using Skoruba.IdentityServer4.STS.Identity.Helpers;
using Skoruba.IdentityServer4.STS.Identity.Helpers.Localization;
using Skoruba.IdentityServer4.STS.Identity.Services.Captcha;
using Skoruba.IdentityServer4.STS.Identity.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Skoruba.IdentityServer4.STS.Identity.Controllers
{
    [SecurityHeaders]
    [Authorize]
    public class AccountController<TUser, TKey> : Controller
        where TUser : IdentityUser<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        private readonly UserResolver<TUser> _userResolver;
        private readonly UserManager<TUser> _userManager;
        private readonly ApplicationSignInManager<TUser> _signInManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IEventService _events;
        private readonly IEmailSender _emailSender;
        private readonly IGenericControllerLocalizer<AccountController<TUser, TKey>> _localizer;
        private readonly LoginConfiguration _loginConfiguration;
        private readonly RegisterConfiguration _registerConfiguration;
        private readonly IdentityOptions _identityOptions;
        private readonly ILogger<AccountController<TUser, TKey>> _logger;
        private readonly AuthenticationBuilder _authenticationBuilder;
        private readonly ILabsoftAccountExternalProviderService _labsoftAccountExternalProviderService;
        private readonly IHttpRequestService _httpRequestService;
        private readonly IExternalSystemLoginService _myLIMSwebLoginService;
        private readonly IRequestTokenService _requestTokenService;
        private readonly IConfiguration _configuration;
        private readonly ICaptchaService _captchaService;
        private const string companyClaimType = "company";
        private const string GrantType = "client_credentials";

        public AccountController(
            UserResolver<TUser> userResolver,
            UserManager<TUser> userManager,
            ApplicationSignInManager<TUser> signInManager,
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IAuthenticationSchemeProvider schemeProvider,
            IEventService events,
            IEmailSender emailSender,
            IGenericControllerLocalizer<AccountController<TUser, TKey>> localizer,
            LoginConfiguration loginConfiguration,
            RegisterConfiguration registerConfiguration,
            IdentityOptions identityOptions,
            ILogger<AccountController<TUser, TKey>> logger,
            AuthenticationBuilder authenticationBuilder,
            ILabsoftAccountExternalProviderService labsoftAccountExternalProviderService,
            IHttpRequestService httpRequestService,
            IExternalSystemLoginService myLIMSwebLoginService,
            IRequestTokenService requestTokenService,
            IConfiguration configuration,
            ICaptchaService captchaService)
        {
            _userResolver = userResolver;
            _userManager = userManager;
            _signInManager = signInManager;
            _interaction = interaction;
            _clientStore = clientStore;
            _schemeProvider = schemeProvider;
            _events = events;
            _emailSender = emailSender;
            _localizer = localizer;
            _loginConfiguration = loginConfiguration;
            _registerConfiguration = registerConfiguration;
            _identityOptions = identityOptions;
            _logger = logger;
            _authenticationBuilder = authenticationBuilder;
            _labsoftAccountExternalProviderService = labsoftAccountExternalProviderService;
            _httpRequestService = httpRequestService;
            _myLIMSwebLoginService = myLIMSwebLoginService;
            _requestTokenService = requestTokenService;
            _configuration = configuration;
            _captchaService = captchaService;
        }

        /// <summary>
        /// Entry point into the login workflow
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl)
        {
            // build a model so we know what to show on the login page
            var loginViewModel = await BuildLoginViewModelAsync(returnUrl);
            var provider = loginViewModel.ExternalProviders.Count() <= 0 ? "OpenIdConnect" : loginViewModel.ExternalProviders.First().AuthenticationScheme;

            if (loginViewModel.EnableLocalLogin == false)
            {
                // only one option for logging in
                return await ExternalLogin(
                    provider: provider,
                    returnUrl: returnUrl,
                    requesterClient: loginViewModel.RequesterClient,
                    labsoftExternalProvider: loginViewModel.LabsoftExternalProvider);
            }
            return View(loginViewModel);
        }

        /// <summary>
        /// Handle postback from username/password login
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputModel model, string button)
        {
            // check if we are in the context of an authorization request
            var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);

            // the user clicked the "cancel" button
            if (button != "login")
            {
                if (context != null)
                {
                    // if the user cancels, send a result back into IdentityServer as if they 
                    // denied the consent (even if this client does not require consent).
                    // this will send back an access denied OIDC error response to the client.
                    await _interaction.DenyAuthorizationAsync(context, AuthorizationError.AccessDenied);

                    // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                    if (context.IsNativeClient())
                    {
                        // The client is native, so this change in how to
                        // return the response is for better UX for the end user.
                        return this.LoadingPage("Redirect", model.ReturnUrl);
                    }

                    return Redirect(model.ReturnUrl);
                }

                // since we don't have a valid context, then we just go back to the home page
                return Redirect("~/");
            }

            if (ModelState.IsValid)
            {

                if (!model.PreviousFormState)
                {
                    var (captchaResponse, captchaErrorMessage) = await _captchaService.Validate(model.GRecaptchaResponse);
                    if (!captchaResponse)
                    {
                        var captchaProvider = _configuration["CaptchaConfiguration:Provider"];
                        _logger.LogError("Unable to validate captcha ({0}): {1}", captchaProvider, captchaErrorMessage);
                        ModelState.AddModelError(string.Empty, _localizer["InvalidCaptcha"]);
                        return await ShowError(model);
                    }
                }

                var user = await _userResolver.GetUserAsync(model.Username);
                if (user != default(TUser))
                {
                    var result = await _signInManager.PasswordSignInAsync(
                        user.UserName,
                        model.Password,
                        model.RememberLogin,
                        lockoutOnFailure: true);

                    if (result.Succeeded)
                    {
                        await _events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id.ToString(), user.UserName));

                        if (context != null)
                        {
                            if (context.IsNativeClient())
                            {
                                // The client is native, so this change in how to
                                // return the response is for better UX for the end user.
                                return this.LoadingPage("Redirect", model.ReturnUrl);
                            }

                            // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                            return Redirect(model.ReturnUrl);
                        }

                        // request for a local page
                        if (Url.IsLocalUrl(model.ReturnUrl))
                        {
                            return Redirect(model.ReturnUrl);
                        }

                        if (string.IsNullOrEmpty(model.ReturnUrl))
                        {
                            return Redirect("~/");
                        }

                        // user might have clicked on a malicious link - should be logged
                        throw new Exception("invalid return URL");
                    }

                    if (result.RequiresTwoFactor)
                    {
                        return RedirectToAction(nameof(LoginWith2fa), new { model.ReturnUrl, RememberMe = model.RememberLogin });
                    }

                    if (result.IsLockedOut)
                    {
                        return View("Lockout");
                    }
                }
                else
                {
                    var requesterClient = GetRequesterClient(model.ReturnUrl);
                    if (!string.IsNullOrEmpty(requesterClient))
                    {
                        var externalLoginService = await ValidateLoginWithMyLIMSwebServiceAsync(model);
                        if (externalLoginService.SignInResult.Succeeded)
                        {
                            var labsoftTokenResponse = await GetAccessTokenAsync(externalLoginService);
                            if (labsoftTokenResponse.AccessToken != null)
                            {
                                var myLIMSwebBaseUrlFormatted = _myLIMSwebLoginService.GetFormattedBaseUrl(requesterClient);
                                var usernameEncoded = HttpUtility.UrlEncode(model.Username);
                                return Redirect($"{myLIMSwebBaseUrlFormatted}?login={usernameEncoded}&token={labsoftTokenResponse.AccessToken}");
                            }
                            else
                            {
                                _logger.LogError("Unable to generate token via Client Credentials flow: {0}", labsoftTokenResponse.ErrorMessage);
                                await _events.RaiseAsync(new UserLoginFailureEvent(model.Username, "unauthorized system", clientId: context?.Client.ClientId));
                                ModelState.AddModelError(string.Empty, _localizer["UnauthorizedSystem"]);
                                return await ShowError(model);
                            }
                        }
                    }
                }

                await _events.RaiseAsync(new UserLoginFailureEvent(model.Username, "invalid credentials", clientId: context?.Client.ClientId));
                ModelState.AddModelError(string.Empty, _localizer["IncorrectUsernameOrPassword"]);
            }

            return await ShowError(model);
        }

        private async Task<IActionResult> ShowError(LoginInputModel model)
        {
            // something went wrong, show form with error
            var vm = await BuildLoginViewModelAsync(model);
            return View(vm);
        }

        private async Task<ExternalLoginServiceResponse> ValidateLoginWithMyLIMSwebServiceAsync(LoginInputModel model)
        {
            var requesterClient = GetRequesterClient(model.ReturnUrl);

            var result = await _myLIMSwebLoginService.ValidateLoginAsync(
                model.Username,
                model.Password,
                requesterClient);

            return result;
        }

        private async Task<LabsoftTokenResponse> GetAccessTokenAsync(ExternalLoginServiceResponse externalLoginService)
        {
            return await _requestTokenService.GetTokenByClientCredentialsAsync(new LabsoftTokenRequest(
                authority: externalLoginService.Response.Authority,
                clientId: externalLoginService.Response.ClientId,
                clientSecret: externalLoginService.Response.ClientSecret,
                scope: externalLoginService.Response.Scope));
        }

        /// <summary>
        /// Show logout page
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Logout(string logoutId)
        {
            // build a model so the logout page knows what to display
            var vm = await BuildLogoutViewModelAsync(logoutId);

            if (vm.ShowLogoutPrompt == false)
            {
                // if the request for logout was properly authenticated from IdentityServer, then
                // we don't need to show the prompt and can just log the user out directly.
                return await Logout(vm);
            }

            return View(vm);
        }

        /// <summary>
        /// Handle logout page postback
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutInputModel model)
        {
            // build a model so the logged out page knows what to display
            var loggedOutViewModel = await BuildLoggedOutViewModelAsync(model.LogoutId);

            if (User?.Identity.IsAuthenticated == true)
            {
                // delete local authentication cookie
                await _signInManager.SignOutAsync();

                await _events.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetName()));
            }

            // check if we need to trigger sign-out at an upstream identity provider
            if (loggedOutViewModel.TriggerExternalSignout)
            {
                // build a return URL so the upstream provider will redirect back
                // to us after the user has logged out. this allows us to then
                // complete our single sign-out processing.
                string url = Url.Action("Logout", new { logoutId = loggedOutViewModel.LogoutId });

                // this triggers a redirect to the external provider for sign-out
                return SignOut(new Microsoft.AspNetCore.Authentication.AuthenticationProperties { RedirectUri = url }, loggedOutViewModel.ExternalAuthenticationScheme);
            }

            if (!string.IsNullOrEmpty(loggedOutViewModel.PostLogoutRedirectUri))
                return Redirect(loggedOutViewModel.PostLogoutRedirectUri);

            return Redirect("~/");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(
            string userId, 
            string code, 
            string company,
            bool isLabsoftPortal = false)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

            var result = await _userManager.ConfirmEmailAsync(user, code);

            if(isLabsoftPortal is false)
            {
                return View(result.Succeeded ? "ConfirmEmail" : "Error");
            }

            var claimUser = await _userManager.GetClaimsAsync(user); 
            if (claimUser == null)
            {
                return View("Error");
            }

            var claimUserData = claimUser.ToDictionary(x => x.Type, x => x.Value);

            var personalCodeValue = claimUserData.GetValueOrDefault("personal_code");
            var userName = claimUserData.GetValueOrDefault("name");

            await CallPortalApi(user, company, personalCodeValue, userName);

            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        private async Task CallPortalApi(
            TUser user,
            string company,
            string personalCodeValue,
            string userName)
        {
            var token = await GetPortalToken();
            await PostPortalAccount(token, user, company, personalCodeValue, userName);
        }

        private async Task<string> GetPortalToken()
        {
            var clientId = _configuration["Portal:ClientId"] ?? string.Empty;
            var scope = _configuration["Portal:Scope"] ?? string.Empty;
            var clientSecret = _configuration["Portal:ClienteSecret"] ?? string.Empty;
            var grantType = GrantType;
            var resource = string.Empty;
            var authUrl = _configuration["Portal:AuthUrl"] ?? string.Empty;

            var result = await _httpRequestService.PostFormAsync(authUrl,
                                                                 clientId,
                                                                 scope,
                                                                 clientSecret,
                                                                 grantType,
                                                                 resource);

            var parsedObject = JObject.Parse(result.Content);
            var token = parsedObject?["access_token"]?.ToString() ?? string.Empty;

            return token;
        }

        private static T DeserializeApiReturn<T>(string content)
        {
            var result = JsonConvert.DeserializeObject<T>(content);
            return result!;
        }

        private async Task PostPortalAccount(
            string token,
            TUser user,
            string company,
            string personalCodeValue,
            string userName)
        {
            var portalAccountDto = MapPortalAccountDto(user, personalCodeValue, userName);
            var body = JsonConvert.SerializeObject(portalAccountDto);
            var baseUrl = _configuration["Portal:BaseUrl"] ?? string.Empty;
            baseUrl += "Account";

            var result = await _httpRequestService.PostAsync(
                requestUri: baseUrl,
                objectBody: body,
                accessToken: token,
                company: company);
        }

        private static PortalAccountDto MapPortalAccountDto(
            TUser user,
            string personalCodeValue,
            string userName)
        {
            var portalAccountDto = new PortalAccountDto()
            {
                AccountName = userName,
                AccountEmail = user.Email,
                CompanyCode = personalCodeValue
            };

            return portalAccountDto;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword(string requesterClient)
        {
            var forgotPasswordViewModel = new ForgotPasswordViewModel()
            {
                RequesterClient = requesterClient
            };
            return View(forgotPasswordViewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                TUser user = null;
                switch (model.Policy)
                {
                    case LoginResolutionPolicy.Email:
                        try
                        {
                            user = await _userManager.FindByEmailAsync(model.Email);
                        }
                        catch (Exception ex)
                        {
                            // in case of multiple users with the same email this method would throw and reveal that the email is registered
                            _logger.LogError("Error retrieving user by email ({0}) for forgot password functionality: {1}", model.Email, ex.Message);
                            user = null;
                        }
                        break;
                    case LoginResolutionPolicy.Username:
                        try
                        {
                            user = await _userManager.FindByNameAsync(model.Username);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError("Error retrieving user by userName ({0}) for forgot password functionality: {1}", model.Username, ex.Message);
                            user = null;
                        }
                        break;
                }

                if (user == null &&
                    !string.IsNullOrEmpty(model.Username) &&
                    !string.IsNullOrEmpty(model.RequesterClient))
                {
                    var response = await ForgotPasswordByExternalSystem(model);
                    if (response.IsSuccessStatusCode)
                    {
                        return View("ForgotPasswordConfirmation");
                    }
                    else
                    {
                        _logger.LogError("Error requesting forgot password confirmation from myLIMSweb for username ({0}). Message: {1} Content: {2}", 
                            model.Username, 
                            response.Message, 
                            response.Content);
                    }
                }

                if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
                {
                    // Don't reveal that the user does not exist
                    return View("ForgotPasswordConfirmation");
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code }, HttpContext.Request.Scheme);

                await _emailSender.SendEmailAsync(user.Email, _localizer["ResetPasswordTitle"], _localizer["ResetPasswordBody", HtmlEncoder.Default.Encode(callbackUrl)]);

                return View("ForgotPasswordConfirmation");
            }

            return View(model);
        }

        private async Task<LabsoftHttpResponse> ForgotPasswordByExternalSystem(ForgotPasswordViewModel model)
        {
            var requestPasswordResetDto = new RequestPasswordResetDto(model.Username);
            var body = JsonConvert.SerializeObject(requestPasswordResetDto);
            var baseUrl = _configuration["myLIMSweb:BaseUrl"] ?? string.Empty;
            baseUrl = baseUrl.Replace("{company}", model.RequesterClient);
            var url = baseUrl + _configuration["myLIMSweb:RequestPasswordResetUri"] ?? string.Empty;

            var response = await _httpRequestService.PostAsync(url, body);
            return response;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null ? View("Error") : View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(ResetPasswordConfirmation), "Account");
            }

            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Code));
            var result = await _userManager.ResetPasswordAsync(user, code, model.Password);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation), "Account");
            }

            AddErrors(result);

            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, _localizer["ErrorExternalProvider", remoteError]);

                return View(nameof(Login));
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            if (string.IsNullOrEmpty(info.Principal.Identity.Name))
            {
                return RedirectToAction(nameof(Login));
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (result.Succeeded)
            {
                return RedirectToLocal(returnUrl);
            }
            if (result.RequiresTwoFactor)
            {
                return RedirectToAction(nameof(LoginWith2fa), new { ReturnUrl = returnUrl });
            }
            if (result.IsLockedOut)
            {
                return View("Lockout");
            }

            var email = GetEmailFromExternalLoginInfo(info);
            var userName = GetEmailFromExternalLoginInfo(info); // the userName in the Identity Center always will be the email address

            return await CreateUserAndRedirect(
                userName: userName,
                email: email,
                returnUrl: returnUrl,
                info: info);
        }

        [HttpPost]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLogin(
            string provider, 
            string returnUrl = null,
            string requesterClient = null,
            string labsoftExternalProvider = null)
        {
            var isUpdated = await IsExternalProviderAsyncUpdated(requesterClient, labsoftExternalProvider);
            if(isUpdated is false)
            {
                var logoutInputModel = new LogoutInputModel();
                return await Logout(logoutInputModel);
            }

            var labsoftAccountExternalProviderDto = await GetLabsoftAccountExternalProviderDto(requesterClient, labsoftExternalProvider);

            var externalProviderForRegister = labsoftAccountExternalProviderDto.ExternalProviderName;
            if (externalProviderForRegister == EExternalProvider.gmail.ToString())
            {
                provider = GoogleDefaults.AuthenticationScheme;
                TakeGmailAuthentication(labsoftAccountExternalProviderDto);
            }

            if (externalProviderForRegister == EExternalProvider.azure.ToString())
            {
                TakeAzureAuthentication(labsoftAccountExternalProviderDto);
            }

            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            return Challenge(properties, provider);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            // Get the information about the user from the external login provider
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return View("ExternalLoginFailure");
            }

            if (ModelState.IsValid)
            {
                return await CreateUserAndRedirect(
                userName: model.UserName,
                email: model.UserName,
                returnUrl: returnUrl,
                info: info);
            }

            ViewData["LoginProvider"] = info.LoginProvider;
            ViewData["ReturnUrl"] = returnUrl;

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithRecoveryCode(string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new InvalidOperationException(_localizer["Unable2FA"]);
            }

            var model = new LoginWithRecoveryCodeViewModel()
            {
                ReturnUrl = returnUrl
            };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithRecoveryCode(LoginWithRecoveryCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new InvalidOperationException(_localizer["Unable2FA"]);
            }

            var recoveryCode = model.RecoveryCode.Replace(" ", string.Empty);

            var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

            if (result.Succeeded)
            {
                return LocalRedirect(string.IsNullOrEmpty(model.ReturnUrl) ? "~/" : model.ReturnUrl);
            }

            if (result.IsLockedOut)
            {
                return View("Lockout");
            }

            ModelState.AddModelError(string.Empty, _localizer["InvalidRecoveryCode"]);

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWith2fa(bool rememberMe, string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                throw new InvalidOperationException(_localizer["Unable2FA"]);
            }

            var model = new LoginWith2faViewModel()
            {
                ReturnUrl = returnUrl,
                RememberMe = rememberMe
            };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWith2fa(LoginWith2faViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new InvalidOperationException(_localizer["Unable2FA"]);
            }

            var authenticatorCode = model.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, model.RememberMe, model.RememberMachine);

            if (result.Succeeded)
            {
                return LocalRedirect(string.IsNullOrEmpty(model.ReturnUrl) ? "~/" : model.ReturnUrl);
            }

            if (result.IsLockedOut)
            {
                return View("Lockout");
            }

            ModelState.AddModelError(string.Empty, _localizer["InvalidAuthenticatorCode"]);

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            if (!_registerConfiguration.Enabled) return View("RegisterFailure");

            ViewData["ReturnUrl"] = returnUrl;

            return _loginConfiguration.ResolutionPolicy switch
            {
                LoginResolutionPolicy.Username => View(),
                LoginResolutionPolicy.Email => View("RegisterWithoutUsername"),
                _ => View("RegisterFailure")
            };
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null, bool IsCalledFromRegisterWithoutUsername = false)
        {
            if (!_registerConfiguration.Enabled) return View("RegisterFailure");

            returnUrl ??= Url.Content("~/");

            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid) return View(model);

            var user = new TUser
            {
                UserName = model.UserName,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code }, HttpContext.Request.Scheme);

                await _emailSender.SendEmailAsync(model.Email, _localizer["ConfirmEmailTitle"], _localizer["ConfirmEmailBody", HtmlEncoder.Default.Encode(callbackUrl)]);

                if (_identityOptions.SignIn.RequireConfirmedAccount)
                {
                    return View("RegisterConfirmation");
                }
                else
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
            }

            AddErrors(result);

            // If we got this far, something failed, redisplay form
            if (IsCalledFromRegisterWithoutUsername)
            {
                var registerWithoutUsernameModel = new RegisterWithoutUsernameViewModel
                {
                    Email = model.Email,
                    Password = model.Password,
                    ConfirmPassword = model.ConfirmPassword
                };

                return View("RegisterWithoutUsername", registerWithoutUsernameModel);
            }
            else
            {
                return View(model);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterWithoutUsername(RegisterWithoutUsernameViewModel model, string returnUrl = null)
        {
            var registerModel = new RegisterViewModel
            {
                UserName = model.Email,
                Email = model.Email,
                Password = model.Password,
                ConfirmPassword = model.ConfirmPassword
            };

            return await Register(registerModel, returnUrl, true);
        }

        /*****************************************/
        /* helper APIs for the AccountController */
        /*****************************************/
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private async Task<LoginViewModel> BuildLoginViewModelAsync(string returnUrl)
        {
            var RequesterClientAndLabsoftExternalProvider = GetRequesterClientAndLabsoftPortalExternalProvider(returnUrl);

            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            if (context?.IdP != null && await _schemeProvider.GetSchemeAsync(context.IdP) != null)
            {
                var local = context.IdP == IdentityServerConstants.LocalIdentityProvider;

                // this is meant to short circuit the UI and only trigger the one external IdP
                var vm = new LoginViewModel
                {
                    EnableLocalLogin = local,
                    ReturnUrl = returnUrl,
                    Username = context?.LoginHint,
                };

                if (!local)
                {
                    vm.ExternalProviders = new[] { new ExternalProvider { AuthenticationScheme = context.IdP } };
                }

                return vm;
            }

            var schemes = await _schemeProvider.GetAllSchemesAsync();
            var providers = schemes
                .Where(x => x.DisplayName != null)
                .Select(x => new ExternalProvider
                {
                    DisplayName = x.DisplayName ?? x.Name,
                    AuthenticationScheme = x.Name
                }).ToList();

            var enableLocalLogin = true;
            if (context?.Client.ClientId != null)
            {
                var client = await _clientStore.FindEnabledClientByIdAsync(context.Client.ClientId);
                if (client != null)
                {
                    enableLocalLogin = client.EnableLocalLogin && AccountOptions.AllowLocalLogin;

                    if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
                    {
                        providers = providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
                    }
                }
            }

            if (string.IsNullOrEmpty(RequesterClientAndLabsoftExternalProvider.Item1) is false)
            {
                var labsoftAccountExternalProviders = await _labsoftAccountExternalProviderService.GetByAccountDomain(
                    accountDomain: RequesterClientAndLabsoftExternalProvider.Item1);
                if (IsRegisteredExternalProvider(labsoftAccountExternalProviders) is true)
                {
                    enableLocalLogin = false;
                }
            }

            return new LoginViewModel
            {
                AllowRememberLogin = AccountOptions.AllowRememberLogin,
                EnableLocalLogin = enableLocalLogin,
                ReturnUrl = returnUrl,
                Username = context?.LoginHint,
                ExternalProviders = providers.ToArray(),
                RequesterClient = RequesterClientAndLabsoftExternalProvider.Item1,
                LabsoftExternalProvider = RequesterClientAndLabsoftExternalProvider.Item2
            };
        }

        private async Task<LoginViewModel> BuildLoginViewModelAsync(LoginInputModel model)
        {
            var vm = await BuildLoginViewModelAsync(model.ReturnUrl);
            vm.Username = model.Username;
            vm.RememberLogin = model.RememberLogin;
            return vm;
        }

        private async Task<LogoutViewModel> BuildLogoutViewModelAsync(string logoutId)
        {
            var logoutViewModel = new LogoutViewModel { LogoutId = logoutId, ShowLogoutPrompt = AccountOptions.ShowLogoutPrompt };

            if (User?.Identity.IsAuthenticated != true)
            {
                // if the user is not authenticated, then just show logged out page
                logoutViewModel.ShowLogoutPrompt = false;
                return logoutViewModel;
            }

            var context = await _interaction.GetLogoutContextAsync(logoutId);
            if (context?.ShowSignoutPrompt == false)
            {
                // it's safe to automatically sign-out
                logoutViewModel.ShowLogoutPrompt = false;
                return logoutViewModel;
            }

            // show the logout prompt. this prevents attacks where the user
            // is automatically signed out by another malicious web page.
            return logoutViewModel;
        }

        private async Task<LoggedOutViewModel> BuildLoggedOutViewModelAsync(string logoutId)
        {
            // get context information (client name, post logout redirect URI and iframe for federated signout)
            var logout = await _interaction.GetLogoutContextAsync(logoutId);

            var vm = new LoggedOutViewModel
            {
                AutomaticRedirectAfterSignOut = AccountOptions.AutomaticRedirectAfterSignOut,
                PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
                ClientName = string.IsNullOrEmpty(logout?.ClientName) ? logout?.ClientId : logout?.ClientName,
                SignOutIframeUrl = logout?.SignOutIFrameUrl,
                LogoutId = logoutId
            };

            if (User?.Identity.IsAuthenticated == true)
            {
                var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
                if (idp != null && idp != IdentityServerConstants.LocalIdentityProvider)
                {
                    var providerSupportsSignout = await HttpContext.GetSchemeSupportsSignOutAsync(idp);
                    if (providerSupportsSignout)
                    {
                        if (vm.LogoutId == null)
                        {
                            // if there's no current logout context, we need to create one
                            // this captures necessary info from the current logged in user
                            // before we signout and redirect away to the external IdP for signout
                            vm.LogoutId = await _interaction.CreateLogoutContextAsync();
                        }

                        vm.ExternalAuthenticationScheme = idp;
                    }
                }
            }

            return vm;
        }

        private static string GetRequesterClient(string returnUrl)
        {
            return GetRequesterClientAndLabsoftPortalExternalProvider(returnUrl).Item1;
        }

        private static (string, string) GetRequesterClientAndLabsoftPortalExternalProvider(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl))
            {
                return (string.Empty, string.Empty);
            }

            var returnUrlArray = returnUrl.Split('&');
            var dictionary = new Dictionary<string, string>();
            for (int i = 0; i <= returnUrlArray.Length - 1; i++)
            {
                var dictionaryValues = returnUrlArray[i].Split('=');
                dictionary.Add(dictionaryValues[0], dictionaryValues[1]);
            }
            var requesterClient = dictionary.GetValueOrDefault("requesterClient") ?? string.Empty;
            var labsoftExternalProvider = dictionary.GetValueOrDefault("labsoftExternalProvider") ?? string.Empty;

            return (requesterClient, labsoftExternalProvider);
        }

        private async Task<bool> IsExternalProviderAsyncUpdated(
            string requesterClient,
            string labsoftExternalProvider)
        {
            var labsoftAccountExternalProvider = await GetLabsoftAccountExternalProviderDto(requesterClient, labsoftExternalProvider);

            if (string.IsNullOrEmpty(labsoftAccountExternalProvider!.ExternalProviderName) is false)
            {
                return true;
            }

            return false;
        }

        private async Task<LabsoftAccountExternalProviderDto> GetLabsoftAccountExternalProviderDto(
            string requesterClient,
            string labsoftExternalProvider)
        {
            var labsoftAccountExternalProviders = await _labsoftAccountExternalProviderService.GetByAccountDomain(requesterClient);

            if (IsRegisteredExternalProvider(labsoftAccountExternalProviders) is false)
            {
                return new LabsoftAccountExternalProviderDto(
                    accountDomain: string.Empty,
                    externalProviderName: string.Empty,
                    tenantId: string.Empty,
                    clientId: string.Empty,
                    secretId: string.Empty);
            }

            LabsoftAccountExternalProviderDto labsoftAccountExternalProvider;

            if (string.IsNullOrEmpty(labsoftExternalProvider))
            {
                labsoftAccountExternalProvider = labsoftAccountExternalProviders.FirstOrDefault();
            }
            else
            {
                labsoftAccountExternalProvider = labsoftAccountExternalProviders
                    .Where(l => l.ExternalProviderName == labsoftExternalProvider)
                    .FirstOrDefault();
            }

            if(labsoftAccountExternalProvider == null)
            {
                return new LabsoftAccountExternalProviderDto(
                    accountDomain: string.Empty,
                    externalProviderName: string.Empty,
                    tenantId: string.Empty,
                    clientId: string.Empty,
                    secretId: string.Empty);
            }

            return labsoftAccountExternalProvider;
        }

        private async Task<IActionResult> CreateUserAndRedirect(
            string userName,
            string email,
            string returnUrl,
            ExternalLoginInfo info)
        {
            var user = new TUser
            {
                UserName = userName,
                Email = email
            };

            var userManager = await _userManager.FindByEmailAsync(user.Email);
            IdentityResult createResult = new IdentityResult();
            var hasAnyProblemInTheCreate = false;
            var requesterClientAndLabsoftPortalExternalProvider = GetRequesterClientAndLabsoftPortalExternalProvider(returnUrl);

            if (userManager is not null)
            {
                (hasAnyProblemInTheCreate, createResult) = await CreateUser(
                    user: userManager,
                    info: info,
                    requesterClient: requesterClientAndLabsoftPortalExternalProvider.Item1);
                if (hasAnyProblemInTheCreate)
                {
                    goto AddErrors;
                }

                await _signInManager.SignInAsync(userManager, isPersistent: false);
                return RedirectToLocal(returnUrl);
            }

            createResult = await _userManager.CreateAsync(user);
            if (createResult.Succeeded)
            {
                (hasAnyProblemInTheCreate, createResult) = await CreateUser(
                    user: user,
                    info: info,
                    requesterClient: requesterClientAndLabsoftPortalExternalProvider.Item1);
                if (hasAnyProblemInTheCreate)
                {
                    goto AddErrors;
                }
                await _signInManager.SignInAsync(user, isPersistent: false);

                return RedirectToLocal(returnUrl);
            }

            AddErrors:
            AddErrors(createResult);
            return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = email, UserName = userName });
        }

        private void TakeAzureAuthentication(
            LabsoftAccountExternalProviderDto labsoftAccountExternalProviderDto)
        {
            _authenticationBuilder
            .AddMicrosoftIdentityWebApp(options =>
            {
                options.Authority = "https://login.microsoftonline.com/common";
                options.Instance = $"https://login.microsoftonline.com/{labsoftAccountExternalProviderDto.TenantId}/v2.0";
                options.TenantId = labsoftAccountExternalProviderDto.TenantId;
                options.ClientId = labsoftAccountExternalProviderDto.ClientId;
                options.ClientSecret = labsoftAccountExternalProviderDto.SecretId;
                options.Domain = string.Empty;
                options.CallbackPath = "/signin-aad";
                options.SignInScheme = IdentityConstants.ExternalScheme;
                options.SignOutScheme = IdentityServerConstants.SignoutScheme;
                options.ResponseType = "id_token";
                options.SignedOutCallbackPath = "/signout-callback-aad";
                options.RemoteSignOutPath = "/signout-aad";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    NameClaimType = "name",
                    RoleClaimType = "role"
                };

            }, cookieScheme: null);
        }

        private bool IsRegisteredExternalProvider(
            IEnumerable<LabsoftAccountExternalProviderDto> labsoftAccountExternalProviders)
        {
            if (labsoftAccountExternalProviders == null)
            {
                return false;
            }

            if(labsoftAccountExternalProviders.Count() == 0)
            {
                return false;
            }

            var labsoftAccountExternalProvider = labsoftAccountExternalProviders
                .Where(l => string.IsNullOrEmpty(l.ClientId) ||
                    string.IsNullOrEmpty(l.SecretId))
                .FirstOrDefault();

            if (labsoftAccountExternalProvider is not null)
            {
                return false;
            }

            return true;
        }

        private async Task<(IdentityResult, bool)> CreateLoginExternalProvider(
            TUser userManager,
            ExternalLoginInfo info)
        {
            var loginInfos = await _userManager.GetLoginsAsync(userManager);
            if (loginInfos.Count == 0)
            {
                var loginInfo = new UserLoginInfo(info.LoginProvider, info.ProviderKey, info.ProviderDisplayName);
                var createResult = await _userManager.AddLoginAsync(userManager, loginInfo);
                if (!createResult.Succeeded)
                {
                    return (createResult, true);
                }
            }
            return (new IdentityResult(), false);
        }

        private async Task<(IdentityResult, bool)> CreateCompanyClaim(
            TUser userManager,
            string requesterClient)
        {
            var claims = await _userManager.GetClaimsAsync(userManager);
            var countCompanyClaim = claims
                .Where(x => x.Type == companyClaimType)
                .Where(x => x.Value == requesterClient)
                .Count();
            if (countCompanyClaim == 0)
            {
                var claimsToAdd = new List<Claim>
                {
                    new Claim(companyClaimType, requesterClient),
                };
                var createResult = await _userManager.AddClaimsAsync(userManager, claimsToAdd);
                if (!createResult.Succeeded)
                {
                    return (createResult, true);
                }
            }

            return (new IdentityResult(), false);
        }

        private void TakeGmailAuthentication(
            LabsoftAccountExternalProviderDto labsoftAccountExternalProviderDto)
        {
            _authenticationBuilder
            .AddCookie()
            .AddGoogle(options =>
            {
                options.ClientId = labsoftAccountExternalProviderDto.ClientId;
                options.ClientSecret = labsoftAccountExternalProviderDto.SecretId;
                options.CallbackPath = "/signin-google";
                options.SignInScheme = GoogleDefaults.AuthenticationScheme;
            });
        }

        private static string GetEmailFromExternalLoginInfo(ExternalLoginInfo info)
        {
            var email = info.Principal.Claims.Where(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name").FirstOrDefault().Value ?? string.Empty;
            if (IsEmail(email) is false)
            {
                email = info.Principal.Claims.Where(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").FirstOrDefault().Value ?? string.Empty;
            }
            return email;
        }

        private static bool IsEmail(string email)
        {
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            bool isValidEmail = Regex.IsMatch(email, pattern);
            return isValidEmail;
        }

        private async Task<(bool, IdentityResult)> CreateUser(
            TUser user,
            ExternalLoginInfo info,
            string requesterClient)
        {
            var createResult = new IdentityResult();
            var hasAnyProblemInTheCreate = false;

            (createResult, hasAnyProblemInTheCreate) = await CreateLoginExternalProvider(user, info);
            if (hasAnyProblemInTheCreate)
            {
                return (true, createResult);
            }

            (createResult, hasAnyProblemInTheCreate) = await CreateCompanyClaim(
                userManager: user,
                requesterClient: requesterClient);
            if (hasAnyProblemInTheCreate)
            {
                return (true, createResult);
            }

            return (false, createResult);
        }
    }
}
