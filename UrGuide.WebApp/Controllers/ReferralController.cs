using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UrGuide.Data.Entities.Referrals;
using UrGuide.Model.Referrals;
using UrGuide.Services.Referrals;
using UrGuide.WebApp.Models;
using BbQ.Outcome;

namespace UrGuide.WebApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/referrals")]
    [ProducesResponseType(400, Type = typeof(ErrorEnvelop<string>))]
    [ProducesResponseType(500, Type = typeof(ErrorEnvelop<string>))]
    public class ReferralController : ControllerBase
    {
        private readonly IReferralService _referralService;
        private readonly ILogger<ReferralController> _logger;

        public ReferralController(IReferralService referralService, ILogger<ReferralController> logger)
        {
            _referralService = referralService;
            _logger = logger;
        }

        /// <summary>
        /// Generate a new referral code for the authenticated user
        /// </summary>
        [HttpPost("code")]
        [ProducesResponseType(200, Type = typeof(ReferralCodeDto))]
        public async Task<IActionResult> GenerateCode([FromBody] CreateReferralCodeRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _referralService.GenerateReferralCodeAsync(userId, (ReferralCodeType)request.Type);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        /// <summary>
        /// Get the authenticated user's referral code
        /// </summary>
        [HttpGet("code")]
        [ProducesResponseType(200, Type = typeof(ReferralCodeDto))]
        public async Task<IActionResult> GetCode()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _referralService.GetUserReferralCodeAsync(userId);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        /// <summary>
        /// Apply a referral code during signup
        /// </summary>
        [HttpPost("apply")]
        [ProducesResponseType(200, Type = typeof(bool))]
        public async Task<IActionResult> ApplyCode([FromBody] ApplyReferralCodeRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _referralService.ApplyReferralCodeAsync(userId, request.Code);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        /// <summary>
        /// Get referral dashboard for the authenticated user
        /// </summary>
        [HttpGet("dashboard")]
        [ProducesResponseType(200, Type = typeof(ReferralDashboardDto))]
        public async Task<IActionResult> GetDashboard()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _referralService.GetReferralDashboardAsync(userId);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        /// <summary>
        /// Get referral history for the authenticated user
        /// </summary>
        [HttpGet("history")]
        [ProducesResponseType(200, Type = typeof(List<ReferralDto>))]
        public async Task<IActionResult> GetHistory([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var codeResult = await _referralService.GetUserReferralCodeAsync(userId);
            if (codeResult.IsError)
            {
                return BadRequest(ErrorEnvelop.CreateFromOutcome(codeResult.Errors));
            }

            var result = await _referralService.GetReferralsByCodeAsync(codeResult.Value.Code, page, pageSize);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }
    }
}
