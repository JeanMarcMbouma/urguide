using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
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

namespace UrGuide.WebApp.Controllers
{
    [ApiController]
    [ProducesResponseType(400, Type = typeof(ErrorEnvelop<string>))]
    [ProducesResponseType(500, Type = typeof(ErrorEnvelop<string>))]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {

        public AccountController(
            IUserService userService, 
            IAuthService authService, 
            IIdentityServerInteractionService interactionService,
            IJwtTokenService jwtTokenService,
            UserManager<UrGuideUser> userManager,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory)
        {
            UserService = userService ?? throw new ArgumentNullException(nameof(userService));
            AuthService = authService ?? throw new ArgumentNullException(nameof(authService));
            InteractionService = interactionService ?? throw new ArgumentNullException(nameof(interactionService));
            JwtTokenService = jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));
            UserManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            HttpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public IUserService UserService { get; }
        public IAuthService AuthService { get; }
        public IIdentityServerInteractionService InteractionService { get; }
        public IJwtTokenService JwtTokenService { get; }
        public UserManager<UrGuideUser> UserManager { get; }
        public IConfiguration Configuration { get; }
        public IHttpClientFactory HttpClientFactory { get; }

        [HttpGet("/login")]
        public IActionResult Login(string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost("/login")]
        [RateLimit(5, "1m")] // Custom rate limit: 5 login attempts per minute
        public async Task<IActionResult> Login([FromBody] LoginModel model, CancellationToken cancellationToken, string? returnUrl = null)
        {
            var result = await UserService.LoginAsync(model, cancellationToken);
            if (result.IsError)
            {
                return BadRequest(ErrorEnvelop.Create(result.Errors));
            }
            
            var claims = new List<System.Security.Claims.Claim>
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, result.Value.Id),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, result.Value.UserName)
            };
            var identity = new System.Security.Claims.ClaimsIdentity(claims, "login");
            var principal = new System.Security.Claims.ClaimsPrincipal(identity);
            
            await HttpContext.SignInAsync(principal);
            var context = InteractionService.GetAuthorizationContextAsync(returnUrl);
            if (context != null && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return Ok(returnUrl);
        }

        [HttpPost("/register")]
        public async Task<IActionResult> Register([FromBody]CreateUserModel model,
            CancellationToken cancellationToken,
            string? returnUrl = null)
        {
            var result = await UserService.RegisterUserAsync(model, cancellationToken);
            return !result.IsError ? Ok(returnUrl) : (IActionResult)BadRequest(ErrorEnvelop.Create(result.Errors));
        }

        [HttpPost("/newguide")]
        public async Task<IActionResult> NewGuide([FromBody]CreateGuideModel model,
            CancellationToken cancellationToken,
            string? returnUrl = null)
        {
            var result = await UserService.RegisterGuideAsync(model, cancellationToken);
            return !result.IsError ? Ok(returnUrl) : (IActionResult)BadRequest(ErrorEnvelop.Create(result.Errors));
        }

        [HttpGet("confirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromQuery]EmailConfirmationModel emailConfirmation, CancellationToken cancellationToken)
        {
            var result = await AuthService.ConfirmEmailAsync(emailConfirmation, cancellationToken);
            if(!result.IsError)
                return Redirect("/email-confirmed");
            return Forbid();
        }

        [HttpGet("forgetpassword")]
        public async Task<IActionResult> ForgetPassword([FromQuery]PasswordResetRequestModel model, 
            CancellationToken cancellationToken) {
            await AuthService.RequestPasswordResetAsync(model, cancellationToken);
            return Ok();
        }

        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetPassord([FromBody]ResetPasswordModel model,
            CancellationToken cancellationToken) {
            var result = await AuthService.ResetPasswordAsync(model, cancellationToken);
            return result.IsError ? BadRequest(ErrorEnvelop.Create(result.Errors)) : (IActionResult)Ok();
        }

        [Authorize]
        [HttpPost("changepassword")]
        public async Task<IActionResult> ChangePassword([FromBody]ChangePasswordModel model, 
            CancellationToken cancellationToken)
        {
            var result = await AuthService.ChangePasswordAsync(model, cancellationToken);
            return result.IsError ? BadRequest(ErrorEnvelop.Create(result.Errors)) : (IActionResult)Ok();
        }

        [Authorize]
        [HttpGet("/getdetails")]
        [ProducesDefaultResponseType(typeof(User))]
        public async Task<IActionResult> GetDetails(CancellationToken cancellationToken)
        {
            var result = await UserService.GetDetailsAsync(cancellationToken);
            return result.IsError ? BadRequest(ErrorEnvelop.Create(result.Errors)) : (IActionResult)Ok(result.Value);
        }

        [Authorize]
        [HttpPost("/updateguide")]
        [ProducesDefaultResponseType(typeof(bool))]
        public async Task<IActionResult> UpdateGuide([FromBody]UpdateGuideModel model, CancellationToken cancellationToken)
        {
            var result = await UserService.UpdateGuideAsync(model, cancellationToken);

            return result.IsError ? BadRequest(ErrorEnvelop.Create(result.Errors)) : (IActionResult)Ok(result.Value);
        }

        [Authorize]
        [HttpPost("/updateuser")]
        [ProducesDefaultResponseType(typeof(bool))]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserModel model, CancellationToken cancellationToken)
        {
            var result = await UserService.UpdateUserAsync(model, cancellationToken);

            return result.IsError ? BadRequest(ErrorEnvelop.Create(result.Errors)) : (IActionResult)Ok(result.Value);
        }

        [Authorize]
        [HttpGet("logout")]
        public async Task<IActionResult> Signout(string? returnUrl = null)
        {
            await AuthService.SignOutAsync();
            await HttpContext.SignOutAsync();
            var logoutId = Request.Query["logoutId"].ToString();

            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);
            else if (!string.IsNullOrEmpty(logoutId))
            {
                var context = await InteractionService.GetLogoutContextAsync(logoutId);
                returnUrl = context?.PostLogoutRedirectUri;
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }
            }
            return Ok();
        }

        [Authorize]
        [HttpGet("delete")]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, string? returnUrl = null)
        {
            var r = await UserService.DeleteUserAccountAsync(cancellationToken);
            if (!r.IsError)
                await HttpContext.SignOutAsync();
            else
                return BadRequest(ErrorEnvelop.Create(r.Errors));
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return Ok();
        }

        [Authorize]
        [HttpGet("downloaddata")]
        [ProducesDefaultResponseType(typeof(UserDataExport))]
        public async Task<IActionResult> DownloadData(CancellationToken cancellationToken)
        {
            var result = await UserService.GetUserDataExportAsync(cancellationToken);
            if (result.IsError)
                return BadRequest(ErrorEnvelop.Create(result.Errors));

            // Return the data as a JSON file download
            var fileName = $"urguide_user_data_{DateTime.UtcNow:yyyyMMdd_HHmmss}.json";
            var jsonContent = System.Text.Json.JsonSerializer.Serialize(result.Value, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
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
                return BadRequest(ErrorEnvelop.Create("Username/email and password are required"));
            }

            try
            {
                // Use current request's base URL since IdentityServer is in the same process
                // This ensures it works in all environments (localhost:5000, Docker, production)
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var tokenEndpoint = $"{baseUrl}/connect/token";

                // Get admin dashboard client credentials from configuration
                var clientId = Configuration.GetValue<string>("IdentityServer:Clients:AdminDashboard:ClientId") ?? "admin-dashboard";
                var clientSecret = Configuration.GetValue<string>("IdentityServer:Clients:AdminDashboard:ClientSecret")
                    ?? throw new InvalidOperationException("Admin dashboard client secret not configured. Set IdentityServer:Clients:AdminDashboard:ClientSecret in configuration.");

                // Call IdentityServer token endpoint with Resource Owner Password Credentials grant
                using var httpClient = HttpClientFactory.CreateClient();
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
                    // Parse error response
                    var errorResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);
                    var errorMessage = errorResponse?.ContainsKey("error_description") == true
                        ? errorResponse["error_description"]?.ToString() ?? "Invalid credentials"
                        : "Invalid credentials";
                    
                    return BadRequest(ErrorEnvelop.Create(errorMessage));
                }

                // Parse successful token response
                var tokenResponse = JsonSerializer.Deserialize<IdentityServerTokenResponse>(responseContent);
                
                if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken))
                {
                    return BadRequest(ErrorEnvelop.Create("Failed to obtain access token"));
                }

                // Get user details for the response
                var loginModel = new LoginModel { UserName = userName, Password = request.Password };
                var loginResult = await UserService.LoginAsync(loginModel, cancellationToken);
                
                if (loginResult.IsError)
                {
                    return BadRequest(ErrorEnvelop.Create(loginResult.Errors));
                }

                var userDetails = await UserService.GetUserAsync(loginResult.Value.Id, cancellationToken);
                var user = await UserManager.FindByIdAsync(loginResult.Value.Id);
                var roles = user != null ? await UserManager.GetRolesAsync(user) : new List<string>();

                // Return IdentityServer token with user info
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
            catch (HttpRequestException ex)
            {
                return StatusCode(500, ErrorEnvelop.Create($"Failed to communicate with authentication server: {ex.Message}"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ErrorEnvelop.Create($"Authentication failed: {ex.Message}"));
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
                return BadRequest(ErrorEnvelop.Create("Refresh token is required"));
            }

            try
            {
                // Use current request's base URL since IdentityServer is in the same process
                // This ensures it works in all environments (localhost:5000, Docker, production)
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var tokenEndpoint = $"{baseUrl}/connect/token";

                // Get admin dashboard client credentials from configuration
                var clientId = Configuration.GetValue<string>("IdentityServer:Clients:AdminDashboard:ClientId") ?? "admin-dashboard";
                var clientSecret = Configuration.GetValue<string>("IdentityServer:Clients:AdminDashboard:ClientSecret")
                    ?? throw new InvalidOperationException("Admin dashboard client secret not configured. Set IdentityServer:Clients:AdminDashboard:ClientSecret in configuration.");

                // Call IdentityServer token endpoint with refresh token grant
                using var httpClient = HttpClientFactory.CreateClient();
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
                        ? errorResponse["error_description"]?.ToString() ?? "Invalid refresh token"
                        : "Invalid refresh token";
                    
                    return BadRequest(ErrorEnvelop.Create(errorMessage));
                }

                var tokenResponse = JsonSerializer.Deserialize<IdentityServerTokenResponse>(responseContent);
                
                if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken))
                {
                    return BadRequest(ErrorEnvelop.Create("Failed to obtain access token"));
                }

                return Ok(new
                {
                    accessToken = tokenResponse.AccessToken,
                    refreshToken = tokenResponse.RefreshToken,
                    expiresIn = tokenResponse.ExpiresIn,
                    tokenType = tokenResponse.TokenType
                });
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, ErrorEnvelop.Create($"Failed to communicate with authentication server: {ex.Message}"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ErrorEnvelop.Create($"Token refresh failed: {ex.Message}"));
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
                return BadRequest(ErrorEnvelop.Create("Username/email and password are required"));
            }

            try
            {
                // Validate credentials
                var user = await UserManager.FindByNameAsync(userName) 
                    ?? await UserManager.FindByEmailAsync(userName);
                
                if (user == null)
                {
                    return BadRequest(ErrorEnvelop.Create("Invalid credentials"));
                }

                // Check password
                var passwordValid = await UserManager.CheckPasswordAsync(user, request.Password);
                if (!passwordValid)
                {
                    return BadRequest(ErrorEnvelop.Create("Invalid credentials"));
                }

                // Get user roles
                var roles = await UserManager.GetRolesAsync(user);
                var claims = new List<System.Security.Claims.Claim>
                {
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id),
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.UserName ?? ""),
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, user.Email ?? ""),
                    new System.Security.Claims.Claim("role", string.Join(",", roles))
                };

                // Add individual role claims
                foreach (var role in roles)
                {
                    claims.Add(new System.Security.Claims.Claim("role", role));
                }

                // Generate JWT token using JwtTokenService
                var token = JwtTokenService.GenerateToken(user.Id, user.UserName ?? "", user.Email ?? "", roles.ToList());

                // Get user details
                var userDetails = await UserService.GetUserAsync(user.Id, cancellationToken);

                return Ok(new
                {
                    accessToken = token,
                    tokenType = "Bearer",
                    expiresIn = 28800, // 8 hours in seconds
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
            catch (Exception ex)
            {
                return StatusCode(500, ErrorEnvelop.Create($"Token generation failed: {ex.Message}"));
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
            // Claims are transformed automatically in JWT bearer configuration
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var userName = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await UserService.GetUserAsync(userId, cancellationToken);
            if (result.IsError || result.Value == null)
            {
                return BadRequest(ErrorEnvelop.Create(result.Errors));
            }

            // Get roles from standard claim type (transformed from JWT "role" claims)
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
        /// API endpoint for 2FA verification (placeholder for future implementation)
        /// </summary>
        [HttpPost("/api/auth/verify-2fa")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(ErrorEnvelop<string>))]
        public async Task<IActionResult> VerifyTwoFactor([FromBody] dynamic request, CancellationToken cancellationToken)
        {
            // Placeholder for 2FA verification
            // This would need to be implemented based on your 2FA requirements
            await Task.CompletedTask;
            return BadRequest(ErrorEnvelop.Create("2FA verification not yet implemented for admin dashboard"));
        }
    }
}
