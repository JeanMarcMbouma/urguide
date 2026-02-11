using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using UrGuide.Shared.Contracts;
using UrGuide.WebApp.Entities;
using UrGuide.WebApp.Models;
using UrGuide.WebApp.Services;

namespace UrGuide.WebApp.Controllers
{
    [ApiController]
    [Route("api/account/passkey")]
    [ProducesResponseType(400, Type = typeof(ErrorEnvelop<string>))]
    [ProducesResponseType(500, Type = typeof(ErrorEnvelop<string>))]
    public class PasskeyController : Controller
    {
        private readonly IPasskeyService _passkeyService;
        private readonly UserManager<UrGuideUser> _userManager;
        private readonly IUserContext _userContext;
        private readonly SignInManager<UrGuideUser> _signInManager;
        
        public PasskeyController(
            IPasskeyService passkeyService,
            UserManager<UrGuideUser> userManager,
            IUserContext userContext,
            SignInManager<UrGuideUser> signInManager)
        {
            _passkeyService = passkeyService ?? throw new ArgumentNullException(nameof(passkeyService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        }
        
        /// <summary>
        /// Start passkey registration - get credential creation options
        /// </summary>
        [HttpPost("register/options")]
        [Authorize]
        [ProducesResponseType(200, Type = typeof(PasskeyRegistrationStartResponse))]
        public async Task<IActionResult> StartRegistration([FromBody] PasskeyRegistrationStartRequest request)
        {
            var user = await _userManager.FindByIdAsync(_userContext.UserId);
            if (user == null)
            {
                return BadRequest(ErrorEnvelop.Create("User not found"));
            }
            
            var friendlyName = request?.FriendlyName ?? "Passkey";
            var options = await _passkeyService.StartRegistrationAsync(user, friendlyName);
            
            var response = new PasskeyRegistrationStartResponse
            {
                Options = options
            };
            
            return Ok(response);
        }
        
        /// <summary>
        /// Complete passkey registration
        /// </summary>
        [HttpPost("register/complete")]
        [Authorize]
        [ProducesResponseType(200, Type = typeof(PasskeyRegistrationCompleteResponse))]
        public async Task<IActionResult> CompleteRegistration([FromBody] PasskeyRegistrationCompleteRequest request)
        {
            if (request is null || request.AttestationResponse == null)
            {
                return BadRequest(ErrorEnvelop.Create("Invalid attestation response"));
            }
            
            var user = await _userManager.FindByIdAsync(_userContext.UserId);
            if (user == null)
            {
                return BadRequest(ErrorEnvelop.Create("User not found"));
            }
            
            var friendlyName = request.FriendlyName ?? "Passkey";
            var success = await _passkeyService.CompleteRegistrationAsync(user, request.AttestationResponse, friendlyName);
            
            if (!success)
            {
                return BadRequest(ErrorEnvelop.Create("Failed to register passkey"));
            }
            
            var response = new PasskeyRegistrationCompleteResponse
            {
                Success = true,
                CredentialId = Convert.ToBase64String(request.AttestationResponse.Id)
            };
            
            return Ok(response);
        }
        
        /// <summary>
        /// Start passkey login - get assertion options
        /// </summary>
        [HttpPost("login/options")]
        [AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PasskeyLoginStartResponse))]
        public async Task<IActionResult> StartLogin([FromBody] PasskeyLoginStartRequest request)
        {
            if (string.IsNullOrEmpty(request?.UserName))
            {
                return BadRequest(ErrorEnvelop.Create("Username is required"));
            }
            
            var options = await _passkeyService.StartLoginAsync(request.UserName);
            if (options == null)
            {
                return BadRequest(ErrorEnvelop.Create("User not found or no passkeys registered"));
            }
            
            var response = new PasskeyLoginStartResponse
            {
                Options = options
            };
            
            return Ok(response);
        }
        
        /// <summary>
        /// Complete passkey login and sign in the user
        /// </summary>
        [HttpPost("login/complete")]
        [AllowAnonymous]
        [ProducesResponseType(200, Type = typeof(PasskeyLoginCompleteResponse))]
        public async Task<IActionResult> CompleteLogin([FromBody] PasskeyLoginCompleteRequest request)
        {
            if (request?.AssertionResponse == null)
            {
                return BadRequest(ErrorEnvelop.Create("Invalid assertion response"));
            }
            
            var (success, user) = await _passkeyService.CompleteLoginAsync(request.AssertionResponse);
            
            if (!success || user == null)
            {
                return BadRequest(ErrorEnvelop.Create("Failed to authenticate with passkey"));
            }
            
            // Sign in the user
            await _signInManager.SignInAsync(user, isPersistent: true);
            
            var response = new PasskeyLoginCompleteResponse
            {
                Success = true,
                UserId = user.Id
            };
            
            return Ok(response);
        }
        
        /// <summary>
        /// List all registered passkeys for the current user
        /// </summary>
        [HttpGet("list")]
        [Authorize]
        [ProducesResponseType(200, Type = typeof(PasskeyInfo[]))]
        public async Task<IActionResult> ListPasskeys()
        {
            var passkeys = await _passkeyService.GetUserPasskeysAsync(_userContext.UserId);
            
            var response = passkeys.Select(p => new PasskeyInfo
            {
                Id = p.Id,
                FriendlyName = p.FriendlyName,
                CreatedAt = p.CreatedAt,
                LastUsedAt = p.LastUsedAt
            }).ToArray();
            
            return Ok(response);
        }
        
        /// <summary>
        /// Delete a passkey
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(200)]
        public async Task<IActionResult> DeletePasskey(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(ErrorEnvelop.Create("Credential ID is required"));
            }
            
            var success = await _passkeyService.DeletePasskeyAsync(_userContext.UserId, id);
            
            if (!success)
            {
                return BadRequest(ErrorEnvelop.Create("Failed to delete passkey"));
            }
            
            return Ok(new { message = "Passkey deleted successfully" });
        }
    }
}
