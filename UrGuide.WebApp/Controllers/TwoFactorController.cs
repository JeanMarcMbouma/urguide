using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq;
using System.Threading.Tasks;
using UrGuide.Shared.Contracts;
using UrGuide.WebApp.Data;
using UrGuide.WebApp.Entities;
using UrGuide.WebApp.Models;
using UrGuide.WebApp.Services;

namespace UrGuide.WebApp.Controllers
{
    [ApiController]
    [Route("api/account/2fa")]
    [Authorize]
    [ProducesResponseType(400, Type = typeof(ErrorEnvelop<string>))]
    [ProducesResponseType(500, Type = typeof(ErrorEnvelop<string>))]
    public class TwoFactorController : Controller
    {
        private readonly ITwoFactorService _twoFactorService;
        private readonly UserManager<UrGuideUser> _userManager;
        private readonly IUserContext _userContext;
        private readonly IMemoryCache _cache;
        private readonly UrGuideAuthContext _context;
        
        public TwoFactorController(
            ITwoFactorService twoFactorService,
            UserManager<UrGuideUser> userManager,
            IUserContext userContext,
            IMemoryCache cache,
            UrGuideAuthContext context)
        {
            _twoFactorService = twoFactorService ?? throw new ArgumentNullException(nameof(twoFactorService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        
        /// <summary>
        /// Get current 2FA status
        /// </summary>
        [HttpGet("status")]
        [ProducesResponseType(200, Type = typeof(TwoFactorStatusResponse))]
        public async Task<IActionResult> GetStatus()
        {
            var user = await _userManager.FindByIdAsync(_userContext.UserId);
            if (user == null)
            {
                return BadRequest(ErrorEnvelop.Create("User not found"));
            }
            
            // Query passkey count from database
            var passkeyCount = await _context.PasskeyCredentials
                .Where(c => c.UserId == _userContext.UserId)
                .CountAsync();
            
            var response = new TwoFactorStatusResponse
            {
                IsEnabled = user.TwoFactorEnabled,
                EnabledAt = user.TwoFactorEnabledAt,
                RemainingBackupCodes = _twoFactorService.GetRemainingBackupCodesCount(user),
                PasskeyCount = passkeyCount
            };
            
            return Ok(response);
        }
        
        /// <summary>
        /// Start 2FA setup - generates QR code
        /// </summary>
        [HttpPost("enable")]
        [ProducesResponseType(200, Type = typeof(Enable2FAResponse))]
        public async Task<IActionResult> Enable()
        {
            var user = await _userManager.FindByIdAsync(_userContext.UserId);
            if (user == null)
            {
                return BadRequest(ErrorEnvelop.Create("User not found"));
            }
            
            if (user.TwoFactorEnabled)
            {
                return BadRequest(ErrorEnvelop.Create("2FA is already enabled"));
            }
            
            var (secret, qrCode, manualKey) = await _twoFactorService.GenerateQRCodeAsync(user);
            
            // Store secret in cache for verification (expires in 10 minutes)
            var cacheKey = $"2fa_setup_{user.Id}";
            _cache.Set(cacheKey, secret, TimeSpan.FromMinutes(10));
            
            var response = new Enable2FAResponse
            {
                Secret = secret,
                QRCodeBase64 = qrCode,
                ManualEntryKey = manualKey
            };
            
            return Ok(response);
        }
        
        /// <summary>
        /// Verify setup code and complete 2FA enablement
        /// </summary>
        [HttpPost("verify")]
        [ProducesResponseType(200, Type = typeof(Verify2FASetupResponse))]
        public async Task<IActionResult> VerifySetup([FromBody] Verify2FASetupRequest request)
        {
            if (string.IsNullOrEmpty(request.Code))
            {
                return BadRequest(ErrorEnvelop.Create("Code is required"));
            }
            
            var user = await _userManager.FindByIdAsync(_userContext.UserId);
            if (user == null)
            {
                return BadRequest(ErrorEnvelop.Create("User not found"));
            }
            
            if (user.TwoFactorEnabled)
            {
                return BadRequest(ErrorEnvelop.Create("2FA is already enabled"));
            }
            
            // Use the secret that was generated during the 2FA enable/setup call
            var cacheKey = $"2fa_setup_{user.Id}";
            if (!_cache.TryGetValue<string>(cacheKey, out var secret) || string.IsNullOrEmpty(secret))
            {
                return BadRequest(ErrorEnvelop.Create("2FA setup not initiated or has expired"));
            }
            
            // Temporarily store secret for verification
            user.TwoFactorSecret = secret;
            
            var isValid = await _twoFactorService.VerifyTotpCodeAsync(user, request.Code);
            if (!isValid)
            {
                // Clear the pending secret on failure
                user.TwoFactorSecret = null;
                return BadRequest(ErrorEnvelop.Create("Invalid verification code"));
            }
            
            // Enable 2FA using the same secret that was used to generate the QR code
            await _twoFactorService.EnableTwoFactorAsync(user, secret);
            
            // Generate backup codes
            var backupCodes = await _twoFactorService.GenerateBackupCodesAsync(user);
            
            // Clean up cached secret
            _cache.Remove(cacheKey);
            
            var response = new Verify2FASetupResponse
            {
                Success = true,
                BackupCodes = backupCodes
            };
            
            return Ok(response);
        }
        
        /// <summary>
        /// Disable 2FA
        /// </summary>
        [HttpPost("disable")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Disable()
        {
            var user = await _userManager.FindByIdAsync(_userContext.UserId);
            if (user == null)
            {
                return BadRequest(ErrorEnvelop.Create("User not found"));
            }
            
            if (!user.TwoFactorEnabled)
            {
                return BadRequest(ErrorEnvelop.Create("2FA is not enabled"));
            }
            
            var success = await _twoFactorService.DisableTwoFactorAsync(user);
            if (!success)
            {
                return BadRequest(ErrorEnvelop.Create("Failed to disable 2FA"));
            }
            
            return Ok(new { message = "2FA disabled successfully" });
        }
        
        /// <summary>
        /// Generate new backup codes
        /// </summary>
        [HttpPost("backup-codes/generate")]
        [ProducesResponseType(200, Type = typeof(GenerateBackupCodesResponse))]
        public async Task<IActionResult> GenerateBackupCodes()
        {
            var user = await _userManager.FindByIdAsync(_userContext.UserId);
            if (user == null)
            {
                return BadRequest(ErrorEnvelop.Create("User not found"));
            }
            
            if (!user.TwoFactorEnabled)
            {
                return BadRequest(ErrorEnvelop.Create("2FA is not enabled"));
            }
            
            var backupCodes = await _twoFactorService.GenerateBackupCodesAsync(user);
            
            var response = new GenerateBackupCodesResponse
            {
                BackupCodes = backupCodes
            };
            
            return Ok(response);
        }
        
        /// <summary>
        /// Verify a 2FA code (for authenticated users - e.g., sensitive operations)
        /// Note: For initial login 2FA, use ASP.NET Identity's TwoFactorAuthenticatorSignInAsync
        /// </summary>
        [HttpPost("verify-code")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> VerifyCode([FromBody] Verify2FACodeRequest request)
        {
            if (string.IsNullOrEmpty(request.Code))
            {
                return BadRequest(ErrorEnvelop.Create("Code is required"));
            }
            
            var user = await _userManager.FindByIdAsync(_userContext.UserId);
            if (user == null)
            {
                return BadRequest(ErrorEnvelop.Create("User not found"));
            }
            
            bool isValid = request.IsBackupCode
                ? await _twoFactorService.VerifyBackupCodeAsync(user, request.Code)
                : await _twoFactorService.VerifyTotpCodeAsync(user, request.Code);
            
            if (!isValid)
            {
                return BadRequest(ErrorEnvelop.Create("Invalid verification code"));
            }
            
            return Ok(new { message = "Code verified successfully" });
        }
    }
}
