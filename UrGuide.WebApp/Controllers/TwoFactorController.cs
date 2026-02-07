using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using UrGuide.Shared.Contracts;
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
        
        public TwoFactorController(
            ITwoFactorService twoFactorService,
            UserManager<UrGuideUser> userManager,
            IUserContext userContext)
        {
            _twoFactorService = twoFactorService ?? throw new ArgumentNullException(nameof(twoFactorService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
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
            
            var response = new TwoFactorStatusResponse
            {
                IsEnabled = user.TwoFactorEnabled,
                EnabledAt = user.TwoFactorEnabledAt,
                RemainingBackupCodes = _twoFactorService.GetRemainingBackupCodesCount(user),
                PasskeyCount = user.PasskeyCredentials?.Count ?? 0
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
            
            // Get secret from the enable call (stored temporarily)
            // For this implementation, we'll use the request secret
            // In production, you'd store this in a cache or session
            var (secret, _, _) = await _twoFactorService.GenerateQRCodeAsync(user);
            
            // Temporarily store secret for verification
            user.TwoFactorSecret = secret;
            
            var isValid = await _twoFactorService.VerifyTotpCodeAsync(user, request.Code);
            if (!isValid)
            {
                user.TwoFactorSecret = null;
                return BadRequest(ErrorEnvelop.Create("Invalid verification code"));
            }
            
            // Enable 2FA
            await _twoFactorService.EnableTwoFactorAsync(user, secret);
            
            // Generate backup codes
            var backupCodes = await _twoFactorService.GenerateBackupCodesAsync(user);
            
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
        /// Verify a 2FA code (for login or sensitive operations)
        /// </summary>
        [HttpPost("verify-code")]
        [ProducesResponseType(200)]
        [AllowAnonymous]
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
            
            bool isValid;
            if (request.IsBackupCode)
            {
                isValid = await _twoFactorService.VerifyBackupCodeAsync(user, request.Code);
            }
            else
            {
                isValid = await _twoFactorService.VerifyTotpCodeAsync(user, request.Code);
            }
            
            if (!isValid)
            {
                return BadRequest(ErrorEnvelop.Create("Invalid verification code"));
            }
            
            return Ok(new { message = "Code verified successfully" });
        }
    }
}
