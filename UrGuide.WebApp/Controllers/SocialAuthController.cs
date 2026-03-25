using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.WebApp.Models;
using UrGuide.WebApp.Services;

namespace UrGuide.WebApp.Controllers
{
    /// <summary>
    /// Handles social login (Google, Apple, Microsoft) and account linking/unlinking.
    /// </summary>
    [ApiController]
    [Route("api/social-auth")]
    [ProducesResponseType(400, Type = typeof(ErrorEnvelop<string>))]
    [ProducesResponseType(500, Type = typeof(ErrorEnvelop<string>))]
    public class SocialAuthController : ControllerBase
    {
        private readonly ISocialAuthService _socialAuthService;
        private readonly ILogger<SocialAuthController> _logger;

        private static readonly string[] SupportedProviders = ["Google", "Apple", "Microsoft"];

        public SocialAuthController(
            ISocialAuthService socialAuthService,
            ILogger<SocialAuthController> logger)
        {
            _socialAuthService = socialAuthService;
            _logger = logger;
        }

        /// <summary>
        /// Initiates the social login flow for the specified provider.
        /// Redirects the user to the provider's consent screen.
        /// </summary>
        /// <param name="provider">The social login provider: Google, Apple, or Microsoft.</param>
        /// <param name="returnUrl">Optional URL to redirect after successful login.</param>
        [HttpGet("login/{provider}")]
        public IActionResult Login(string provider, [FromQuery] string? returnUrl = null)
        {
            if (!IsProviderSupported(provider))
            {
                return BadRequest(ErrorEnvelop.Create([$"Unsupported provider: {provider}. Supported: Google, Apple, Microsoft"]));
            }

            var redirectUrl = Url.Action(nameof(Callback), "SocialAuth", new { provider, returnUrl });
            var properties = new AuthenticationProperties
            {
                RedirectUri = redirectUrl,
                Items = { { "provider", provider } }
            };

            return Challenge(properties, provider);
        }

        /// <summary>
        /// OAuth callback endpoint. Processes the provider response and logs in or creates the user.
        /// </summary>
        [HttpGet("callback/{provider}")]
        public async Task<IActionResult> Callback(
            string provider,
            [FromQuery] string? returnUrl,
            CancellationToken cancellationToken)
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(provider);
            if (!authenticateResult.Succeeded || authenticateResult.Principal == null)
            {
                _logger.LogWarning("Social auth callback failed for provider {Provider}", provider);
                return BadRequest(ErrorEnvelop.Create(["Authentication failed. Please try again."]));
            }

            var claims = authenticateResult.Principal.Claims.ToList();
            var providerKey = claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.NameIdentifier)?.Value ?? "";
            var email = claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.Email)?.Value;
            var firstName = claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.GivenName)?.Value;
            var lastName = claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.Surname)?.Value;
            var avatarUrl = claims.FirstOrDefault(c =>
                c.Type == "urn:google:picture" || c.Type == "picture")?.Value;

            if (string.IsNullOrEmpty(providerKey))
            {
                return BadRequest(ErrorEnvelop.Create(["Could not retrieve user identifier from provider."]));
            }

            var result = await _socialAuthService.ProcessSocialLoginAsync(
                provider, providerKey, email, firstName, lastName, avatarUrl, cancellationToken);

            if (!result.Success)
            {
                return BadRequest(ErrorEnvelop.Create([result.Error ?? "Social login failed."]));
            }

            // Log the audit event
            await _socialAuthService.LogAuditEventAsync(
                result.UserId!,
                provider,
                result.IsNewAccount ? "AccountCreated" : "Login",
                result.IsNewAccount ? "New account created via social login" : null,
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                Request.Headers.UserAgent.ToString(),
                cancellationToken);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                // Redirect with token as query parameter for SPA consumption
                var separator = returnUrl.Contains('?') ? "&" : "?";
                return Redirect($"{returnUrl}{separator}token={result.Token}&isNew={result.IsNewAccount}");
            }

            return Ok(new
            {
                result.UserId,
                result.Token,
                result.IsNewAccount,
                Provider = provider
            });
        }

        /// <summary>
        /// Initiates linking a social provider to the current authenticated user.
        /// </summary>
        [Authorize]
        [HttpGet("link/{provider}")]
        public IActionResult InitiateLink(string provider, [FromQuery] string? returnUrl = null)
        {
            if (!IsProviderSupported(provider))
            {
                return BadRequest(ErrorEnvelop.Create([$"Unsupported provider: {provider}"]));
            }

            var redirectUrl = Url.Action(nameof(LinkCallback), "SocialAuth", new { provider, returnUrl });
            var properties = new AuthenticationProperties
            {
                RedirectUri = redirectUrl,
                Items = { { "provider", provider }, { "linking", "true" } }
            };

            return Challenge(properties, provider);
        }

        /// <summary>
        /// Callback for linking a social provider to the current user.
        /// </summary>
        [Authorize]
        [HttpGet("link-callback/{provider}")]
        public async Task<IActionResult> LinkCallback(
            string provider,
            [FromQuery] string? returnUrl,
            CancellationToken cancellationToken)
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(provider);
            if (!authenticateResult.Succeeded || authenticateResult.Principal == null)
            {
                return BadRequest(ErrorEnvelop.Create(["Authentication failed during linking."]));
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var claims = authenticateResult.Principal.Claims.ToList();
            var providerKey = claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.NameIdentifier)?.Value ?? "";
            var email = claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.Email)?.Value;
            var displayName = claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.Name)?.Value;
            var avatarUrl = claims.FirstOrDefault(c =>
                c.Type == "urn:google:picture" || c.Type == "picture")?.Value;

            var result = await _socialAuthService.LinkProviderAsync(
                userId, provider, providerKey, email, displayName, avatarUrl, cancellationToken);

            if (!result.Success)
            {
                return BadRequest(ErrorEnvelop.Create([result.Error ?? "Linking failed."]));
            }

            await _socialAuthService.LogAuditEventAsync(
                userId, provider, "Linked", null,
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                Request.Headers.UserAgent.ToString(),
                cancellationToken);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return Ok(new { Message = $"{provider} account linked successfully." });
        }

        /// <summary>
        /// Unlinks a social provider from the current user.
        /// </summary>
        [Authorize]
        [HttpDelete("unlink/{provider}")]
        public async Task<IActionResult> UnlinkProvider(
            string provider,
            CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            if (!IsProviderSupported(provider))
            {
                return BadRequest(ErrorEnvelop.Create([$"Unsupported provider: {provider}"]));
            }

            var result = await _socialAuthService.UnlinkProviderAsync(userId, provider, cancellationToken);

            if (!result.Success)
            {
                return BadRequest(ErrorEnvelop.Create([result.Error ?? "Unlinking failed."]));
            }

            await _socialAuthService.LogAuditEventAsync(
                userId, provider, "Unlinked", null,
                HttpContext.Connection.RemoteIpAddress?.ToString(),
                Request.Headers.UserAgent.ToString(),
                cancellationToken);

            return Ok(new { Message = $"{provider} account unlinked successfully." });
        }

        /// <summary>
        /// Returns all linked social providers for the current user.
        /// </summary>
        [Authorize]
        [HttpGet("providers")]
        public async Task<IActionResult> GetLinkedProviders(CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var providers = await _socialAuthService.GetLinkedProvidersAsync(userId, cancellationToken);
            return Ok(providers);
        }

        /// <summary>
        /// Returns audit log entries for the current user's social login activities.
        /// </summary>
        [Authorize]
        [HttpGet("audit-log")]
        public async Task<IActionResult> GetAuditLog(
            [FromQuery] int take = 50,
            CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var logs = await _socialAuthService.GetAuditLogAsync(userId, take, cancellationToken);
            return Ok(logs);
        }

        /// <summary>
        /// Returns a list of supported social login providers and their login URLs.
        /// </summary>
        [HttpGet("providers/available")]
        public IActionResult GetAvailableProviders()
        {
            var providers = SupportedProviders.Select(p => new
            {
                Name = p,
                LoginUrl = $"/api/social-auth/login/{p}",
                Icon = p.ToLowerInvariant()
            });

            return Ok(providers);
        }

        private static bool IsProviderSupported(string provider)
        {
            return SupportedProviders.Contains(provider, StringComparer.OrdinalIgnoreCase);
        }
    }
}
