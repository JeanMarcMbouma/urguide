using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Model.Users;
using UrGuide.Services.Contracts;
using UrGuide.Shared.Contracts;
using UrGuide.WebApp.Models;
using UrGuide.WebApp.Attributes;
using UrGuide.WebApp.Services;
using UrGuide.WebApp.Entities;
using UrGuide.WebApp.Resources;
using BbQ.Outcome;

namespace UrGuide.WebApp.Controllers
{
    [ApiController]
    [ProducesResponseType(400, Type = typeof(ErrorEnvelop<string>))]
    [ProducesResponseType(500, Type = typeof(ErrorEnvelop<string>))]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly IIdentityServerInteractionService _interactionService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly UserManager<UrGuideUser> _userManager;
        private readonly ITwoFactorService _twoFactorService;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public AccountController(
            IUserService userService, 
            IAuthService authService, 
            IIdentityServerInteractionService interactionService,
            IJwtTokenService jwtTokenService,
            UserManager<UrGuideUser> userManager,
            ITwoFactorService twoFactorService,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            IStringLocalizer<SharedResource> localizer)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _interactionService = interactionService ?? throw new ArgumentNullException(nameof(interactionService));
            _jwtTokenService = jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _twoFactorService = twoFactorService ?? throw new ArgumentNullException(nameof(twoFactorService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        }

        /// <summary>
        /// Login endpoint - authenticates user and returns session info
        /// </summary>
        [HttpPost("/login")]
        [RateLimit(5, "1m")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Login([FromBody] LoginModel model, CancellationToken cancellationToken, string? returnUrl = null)
        {
            var result = await _userService.LoginAsync(model, cancellationToken);
            if (result.IsError)
            {
                return BadRequest(ErrorEnvelop.Create(_localizer["Auth_InvalidCredentials"].Value));
            }
            
            var claims = new List<System.Security.Claims.Claim>
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, result.Value.Id),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, result.Value.UserName)
            };
            var identity = new System.Security.Claims.ClaimsIdentity(claims, "login");
            var principal = new System.Security.Claims.ClaimsPrincipal(identity);
            
            await HttpContext.SignInAsync(principal);

            return Ok(new { returnUrl });
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        [HttpPost("/register")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Register([FromBody] CreateUserModel model,
            CancellationToken cancellationToken,
            string? returnUrl = null)
        {
            var result = await _userService.RegisterUserAsync(model, cancellationToken);
            return !result.IsError ? Ok(new { returnUrl }) : BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors));
        }

        /// <summary>
        /// Register a new guide
        /// </summary>
        [HttpPost("/newguide")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> NewGuide([FromBody] CreateGuideModel model,
            CancellationToken cancellationToken,
            string? returnUrl = null)
        {
            var result = await _userService.RegisterGuideAsync(model, cancellationToken);
            return !result.IsError ? Ok(new { returnUrl }) : BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors));
        }

        /// <summary>
        /// Confirm a user's email address
        /// </summary>
        [HttpGet("confirmEmail")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> ConfirmEmail([FromQuery] EmailConfirmationModel emailConfirmation, CancellationToken cancellationToken)
        {
            var result = await _authService.ConfirmEmailAsync(emailConfirmation, cancellationToken);
            if (!result.IsError)
                return Ok(new { message = _localizer["Auth_EmailConfirmed"].Value });
            return Forbid();
        }

        /// <summary>
        /// Request a password reset email
        /// </summary>
        [HttpGet("forgetpassword")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> ForgetPassword([FromQuery] PasswordResetRequestModel model, 
            CancellationToken cancellationToken)
        {
            await _authService.RequestPasswordResetAsync(model, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Reset password using a token
        /// </summary>
        [HttpPost("resetpassword")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model,
            CancellationToken cancellationToken)
        {
            var result = await _authService.ResetPasswordAsync(model, cancellationToken);
            return result.IsError ? BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors)) : Ok();
        }

        /// <summary>
        /// Change password for the authenticated user
        /// </summary>
        [Authorize]
        [HttpPost("changepassword")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model, 
            CancellationToken cancellationToken)
        {
            var result = await _authService.ChangePasswordAsync(model, cancellationToken);
            return result.IsError ? BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors)) : Ok();
        }

        /// <summary>
        /// Get current authenticated user details
        /// </summary>
        [Authorize]
        [HttpGet("/getdetails")]
        [ProducesDefaultResponseType(typeof(User))]
        public async Task<IActionResult> GetDetails(CancellationToken cancellationToken)
        {
            var result = await _userService.GetDetailsAsync(cancellationToken);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        /// <summary>
        /// Update guide profile
        /// </summary>
        [Authorize]
        [HttpPost("/updateguide")]
        [ProducesDefaultResponseType(typeof(bool))]
        public async Task<IActionResult> UpdateGuide([FromBody] UpdateGuideModel model, CancellationToken cancellationToken)
        {
            var result = await _userService.UpdateGuideAsync(model, cancellationToken);

            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        /// <summary>
        /// Update user profile
        /// </summary>
        [Authorize]
        [HttpPost("/updateuser")]
        [ProducesDefaultResponseType(typeof(bool))]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserModel model, CancellationToken cancellationToken)
        {
            var result = await _userService.UpdateUserAsync(model, cancellationToken);

            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        /// <summary>
        /// Sign out the current user
        /// </summary>
        [Authorize]
        [HttpGet("logout")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Signout(string? logoutId = null)
        {
            await _authService.SignOutAsync();
            await HttpContext.SignOutAsync();

            string? postLogoutRedirectUri = null;
            if (!string.IsNullOrEmpty(logoutId))
            {
                var context = await _interactionService.GetLogoutContextAsync(logoutId);
                postLogoutRedirectUri = context?.PostLogoutRedirectUri;
            }

            return Ok(new { message = _localizer["Auth_SignedOut"].Value, postLogoutRedirectUri });
        }

        /// <summary>
        /// Delete the current user's account
        /// </summary>
        [Authorize]
        [HttpDelete("delete")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken)
        {
            var r = await _userService.DeleteUserAccountAsync(cancellationToken);
            if (r.IsError)
                return BadRequest(ErrorEnvelop.CreateFromOutcome(r.Errors));

            await HttpContext.SignOutAsync();
            return Ok(new { message = _localizer["Auth_AccountDeleted"].Value });
        }

        /// <summary>
        /// Download all user data as a JSON file (GDPR export)
        /// </summary>
        [Authorize]
        [HttpGet("downloaddata")]
        [ProducesDefaultResponseType(typeof(UserDataExport))]
        public async Task<IActionResult> DownloadData(CancellationToken cancellationToken)
        {
            var result = await _userService.GetUserDataExportAsync(cancellationToken);
            if (result.IsError)
                return BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors));

            var fileName = $"urguide_user_data_{DateTime.UtcNow:yyyyMMdd_HHmmss}.json";
            var jsonContent = JsonSerializer.Serialize(result.Value, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            
            var bytes = System.Text.Encoding.UTF8.GetBytes(jsonContent);
            return File(bytes, "application/json", fileName);
        }

        // ===================================================
        // API-style Auth Endpoints for Admin Dashboard
        // ===================================================

        /// <summary>
        /// API endpoint for admin dashboard login - Uses IdentityServer token endpoint
        /// </summary>
        [HttpPost("/api/auth/login")]
        [RateLimit(5, "1m")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(ErrorEnvelop<string>))]
        public async Task<IActionResult> ApiLogin([FromBody] AdminLoginRequest request, CancellationToken cancellationToken)
        {
            var userName = request?.UserName ?? request?.Email;
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(request?.Password))
            {
                return BadRequest(ErrorEnvelop.Create(_localizer["Auth_CredentialsRequired"].Value));
            }

            try
            {
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var tokenEndpoint = $"{baseUrl}/connect/token";

                var clientId = _configuration.GetValue<string>("IdentityServer:Clients:AdminDashboard:ClientId") ?? "admin-dashboard";
                var clientSecret = _configuration.GetValue<string>("IdentityServer:Clients:AdminDashboard:ClientSecret")
                    ?? throw new InvalidOperationException("Admin dashboard client secret not configured. Set IdentityServer:Clients:AdminDashboard:ClientSecret in configuration.");

                using var httpClient = _httpClientFactory.CreateClient();
                var tokenRequest = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["client_id"] = clientId,
                    ["client_secret"] = clientSecret,
                    ["grant_type"] = "password",
                    ["username"] = userName,
                    ["password"] = request.Password,
                    ["scope"] = "openid profile api1 offline_access"
                });

                var response = await httpClient.PostAsync(tokenEndpoint, tokenRequest, cancellationToken);
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);
                    var errorMessage = errorResponse?.ContainsKey("error_description") == true
                        ? errorResponse["error_description"]?.ToString() ?? _localizer["Auth_InvalidCredentials"].Value
                        : _localizer["Auth_InvalidCredentials"].Value;
                    
                    return BadRequest(ErrorEnvelop.Create(errorMessage));
                }

                var tokenResponse = JsonSerializer.Deserialize<IdentityServerTokenResponse>(responseContent);
                
                if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken))
                {
                    return BadRequest(ErrorEnvelop.Create(_localizer["Auth_TokenObtainFailed"].Value));
                }

                var loginModel = new LoginModel { UserName = userName, Password = request.Password };
                var loginResult = await _userService.LoginAsync(loginModel, cancellationToken);
                
                if (loginResult.IsError)
                {
                    return BadRequest(ErrorEnvelop.CreateFromOutcome(loginResult.Errors));
                }

                var userDetails = await _userService.GetUserAsync(loginResult.Value.Id, cancellationToken);
                var user = await _userManager.FindByIdAsync(loginResult.Value.Id);
                var roles = user != null ? await _userManager.GetRolesAsync(user) : new List<string>();

                return Ok(new
                {
                    accessToken = tokenResponse.AccessToken,
                    refreshToken = tokenResponse.RefreshToken,
                    expiresIn = tokenResponse.ExpiresIn,
                    tokenType = tokenResponse.TokenType,
                    user = new
                    {
                        id = loginResult.Value.Id,
                        email = userName,
                        userName = loginResult.Value.UserName,
                        firstName = userDetails.IsError ? "" : userDetails.Value.FirstName,
                        lastName = userDetails.IsError ? "" : userDetails.Value.LastName,
                        roles = roles
                    }
                });
            }
            catch (HttpRequestException)
            {
                return StatusCode(500, ErrorEnvelop.Create(_localizer["Auth_InternalError"].Value));
            }
            catch (InvalidOperationException)
            {
                return StatusCode(500, ErrorEnvelop.Create(_localizer["Auth_InternalError"].Value));
            }
        }

        /// <summary>
        /// API endpoint to refresh access token using refresh token
        /// </summary>
        [HttpPost("/api/auth/refresh")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(ErrorEnvelop<string>))]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request?.RefreshToken))
            {
                return BadRequest(ErrorEnvelop.Create(_localizer["Auth_RefreshTokenRequired"].Value));
            }

            try
            {
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var tokenEndpoint = $"{baseUrl}/connect/token";

                var clientId = _configuration.GetValue<string>("IdentityServer:Clients:AdminDashboard:ClientId") ?? "admin-dashboard";
                var clientSecret = _configuration.GetValue<string>("IdentityServer:Clients:AdminDashboard:ClientSecret")
                    ?? throw new InvalidOperationException("Admin dashboard client secret not configured. Set IdentityServer:Clients:AdminDashboard:ClientSecret in configuration.");

                using var httpClient = _httpClientFactory.CreateClient();
                var tokenRequest = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["client_id"] = clientId,
                    ["client_secret"] = clientSecret,
                    ["grant_type"] = "refresh_token",
                    ["refresh_token"] = request.RefreshToken
                });

                var response = await httpClient.PostAsync(tokenEndpoint, tokenRequest, cancellationToken);
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);
                    var errorMessage = errorResponse?.ContainsKey("error_description") == true
                        ? errorResponse["error_description"]?.ToString() ?? _localizer["Auth_TokenInvalid"].Value
                        : _localizer["Auth_TokenInvalid"].Value;
                    
                    return BadRequest(ErrorEnvelop.Create(errorMessage));
                }

                var tokenResponse = JsonSerializer.Deserialize<IdentityServerTokenResponse>(responseContent);
                
                if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken))
                {
                    return BadRequest(ErrorEnvelop.Create(_localizer["Auth_TokenObtainFailed"].Value));
                }

                return Ok(new
                {
                    accessToken = tokenResponse.AccessToken,
                    refreshToken = tokenResponse.RefreshToken,
                    expiresIn = tokenResponse.ExpiresIn,
                    tokenType = tokenResponse.TokenType
                });
            }
            catch (HttpRequestException)
            {
                return StatusCode(500, ErrorEnvelop.Create(_localizer["Auth_InternalError"].Value));
            }
            catch (InvalidOperationException)
            {
                return StatusCode(500, ErrorEnvelop.Create(_localizer["Auth_InternalError"].Value));
            }
        }

        /// <summary>
        /// API endpoint for direct JWT token generation (Swagger/Testing)
        /// Generates a JWT token directly for authorized users
        /// </summary>
        [HttpPost("/api/auth/token")]
        [RateLimit(5, "1m")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(ErrorEnvelop<string>))]
        public async Task<IActionResult> GetJwtToken([FromBody] AdminLoginRequest request, CancellationToken cancellationToken)
        {
            var userName = request?.UserName ?? request?.Email;
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(request?.Password))
            {
                return BadRequest(ErrorEnvelop.Create(_localizer["Auth_CredentialsRequired"].Value));
            }

            try
            {
                var user = await _userManager.FindByNameAsync(userName) 
                    ?? await _userManager.FindByEmailAsync(userName);
                
                if (user == null)
                {
                    return BadRequest(ErrorEnvelop.Create(_localizer["Auth_InvalidCredentials"].Value));
                }

                var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
                if (!passwordValid)
                {
                    return BadRequest(ErrorEnvelop.Create(_localizer["Auth_InvalidCredentials"].Value));
                }

                var roles = await _userManager.GetRolesAsync(user);

                var token = _jwtTokenService.GenerateToken(user.Id, user.UserName ?? "", user.Email ?? "", roles.ToList());

                var userDetails = await _userService.GetUserAsync(user.Id, cancellationToken);

                return Ok(new
                {
                    accessToken = token,
                    tokenType = "Bearer",
                    expiresIn = 28800,
                    user = new
                    {
                        id = user.Id,
                        email = user.Email,
                        userName = user.UserName,
                        firstName = userDetails.IsError ? "" : userDetails.Value.FirstName,
                        lastName = userDetails.IsError ? "" : userDetails.Value.LastName,
                        roles = roles.ToArray()
                    }
                });
            }
            catch (Exception)
            {
                return StatusCode(500, ErrorEnvelop.Create(_localizer["Auth_InternalError"].Value));
            }
        }

        /// <summary>
        /// API endpoint to get current authenticated user info
        /// </summary>
        [HttpGet("/api/auth/me")]
        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetCurrentUserInfo(CancellationToken cancellationToken)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var userName = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _userService.GetUserAsync(userId, cancellationToken);
            if (result.IsError || result.Value == null)
            {
                return BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors));
            }

            var roles = User.FindAll(System.Security.Claims.ClaimTypes.Role).Select(c => c.Value).ToArray();

            return Ok(new
            {
                id = result.Value.Id,
                email = email ?? userName ?? "",
                userName = result.Value.UserName,
                firstName = result.Value.FirstName,
                lastName = result.Value.LastName,
                roles = roles
            });
        }

        /// <summary>
        /// Verify a 2FA code during the login flow.
        /// This endpoint does not require authentication since the user is mid-login.
        /// Credentials are re-validated along with the 2FA code.
        /// </summary>
        [HttpPost("/api/auth/verify-2fa")]
        [RateLimit(5, "1m")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(ErrorEnvelop<string>))]
        public async Task<IActionResult> VerifyTwoFactor([FromBody] Verify2FALoginRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request?.Code))
            {
                return BadRequest(ErrorEnvelop.Create(_localizer["Auth_CodeRequired"].Value));
            }

            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(ErrorEnvelop.Create(_localizer["Auth_CredentialsRequired"].Value));
            }

            var user = await _userManager.FindByEmailAsync(request.Email)
                ?? await _userManager.FindByNameAsync(request.Email);

            if (user == null)
            {
                return BadRequest(ErrorEnvelop.Create(_localizer["Auth_InvalidCredentials"].Value));
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!passwordValid)
            {
                return BadRequest(ErrorEnvelop.Create(_localizer["Auth_InvalidCredentials"].Value));
            }

            if (!user.TwoFactorEnabled)
            {
                return BadRequest(ErrorEnvelop.Create(_localizer["Auth_TwoFactorNotEnabled"].Value));
            }

            bool isValid = request.IsBackupCode
                ? await _twoFactorService.VerifyBackupCodeAsync(user, request.Code)
                : await _twoFactorService.VerifyTotpCodeAsync(user, request.Code);

            if (!isValid)
            {
                return BadRequest(ErrorEnvelop.Create(_localizer["Auth_TwoFactorInvalid"].Value));
            }

            return Ok(new { message = _localizer["Auth_TwoFactorVerified"].Value });
        }
    }
}
