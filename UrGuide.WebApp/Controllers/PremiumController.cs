using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UrGuide.Services.Premium;
using UrGuide.Model.Premium;

namespace UrGuide.WebApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/premium")]
    public class PremiumController : ControllerBase
    {
        private readonly IPremiumService _premiumService;

        public PremiumController(IPremiumService premiumService)
        {
            _premiumService = premiumService;
        }

        // Subscription Plans
        [HttpPost("plans")]
        public async Task<IActionResult> CreatePlan([FromBody] CreateSubscriptionPlanRequest request)
        {
            var outcome = await _premiumService.CreatePlanAsync(request);
            if (outcome.IsSuccessful)
                return Ok(outcome.Value);
            return BadRequest(new { errors = outcome.Errors?.Select(e => e.Message) });
        }

        [HttpGet("plans")]
        public async Task<IActionResult> GetAllPlans()
        {
            var outcome = await _premiumService.GetAllPlansAsync();
            if (outcome.IsSuccessful)
                return Ok(outcome.Value);
            return BadRequest(new { errors = outcome.Errors?.Select(e => e.Message) });
        }

        [HttpGet("plans/{planId}")]
        public async Task<IActionResult> GetPlan(string planId)
        {
            var outcome = await _premiumService.GetPlanAsync(planId);
            if (outcome.IsSuccessful)
                return Ok(outcome.Value);
            return BadRequest(new { errors = outcome.Errors?.Select(e => e.Message) });
        }

        // Guide Subscriptions
        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe([FromBody] SubscribeRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var outcome = await _premiumService.SubscribeAsync(userId, request);
            if (outcome.IsSuccessful)
                return Ok(outcome.Value);
            return BadRequest(new { errors = outcome.Errors?.Select(e => e.Message) });
        }

        [HttpGet("subscription")]
        public async Task<IActionResult> GetSubscription()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var outcome = await _premiumService.GetGuideSubscriptionAsync(userId);
            if (outcome.IsSuccessful)
                return Ok(outcome.Value);
            return BadRequest(new { errors = outcome.Errors?.Select(e => e.Message) });
        }

        [HttpPost("subscription/cancel")]
        public async Task<IActionResult> CancelSubscription()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var outcome = await _premiumService.CancelSubscriptionAsync(userId);
            if (outcome.IsSuccessful)
                return Ok(new { cancelled = outcome.Value });
            return BadRequest(new { errors = outcome.Errors?.Select(e => e.Message) });
        }

        // Visibility Boosts
        [HttpPost("boosts")]
        public async Task<IActionResult> CreateBoost([FromBody] CreateVisibilityBoostRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var outcome = await _premiumService.CreateBoostAsync(userId, request);
            if (outcome.IsSuccessful)
                return Ok(outcome.Value);
            return BadRequest(new { errors = outcome.Errors?.Select(e => e.Message) });
        }

        [HttpGet("boosts")]
        public async Task<IActionResult> GetActiveBoosts()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var outcome = await _premiumService.GetActiveBoostsAsync(userId);
            if (outcome.IsSuccessful)
                return Ok(outcome.Value);
            return BadRequest(new { errors = outcome.Errors?.Select(e => e.Message) });
        }

        // Advertisements
        [HttpPost("ads")]
        public async Task<IActionResult> CreateAdvertisement([FromBody] CreateAdvertisementRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var outcome = await _premiumService.CreateAdvertisementAsync(userId, request);
            if (outcome.IsSuccessful)
                return Ok(outcome.Value);
            return BadRequest(new { errors = outcome.Errors?.Select(e => e.Message) });
        }

        [HttpGet("ads/{adId}")]
        public async Task<IActionResult> GetAdvertisement(string adId)
        {
            var outcome = await _premiumService.GetAdvertisementAsync(adId);
            if (outcome.IsSuccessful)
                return Ok(outcome.Value);
            return BadRequest(new { errors = outcome.Errors?.Select(e => e.Message) });
        }

        [HttpGet("ads")]
        public async Task<IActionResult> GetMyAds([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var outcome = await _premiumService.GetAdvertiserAdsAsync(userId, page, pageSize);
            if (outcome.IsSuccessful)
                return Ok(outcome.Value);
            return BadRequest(new { errors = outcome.Errors?.Select(e => e.Message) });
        }

        [HttpPut("ads/{adId}")]
        public async Task<IActionResult> UpdateAdvertisement(string adId, [FromBody] UpdateAdvertisementRequest request)
        {
            var outcome = await _premiumService.UpdateAdvertisementAsync(adId, request);
            if (outcome.IsSuccessful)
                return Ok(outcome.Value);
            return BadRequest(new { errors = outcome.Errors?.Select(e => e.Message) });
        }

        [HttpGet("ads/{adId}/performance")]
        public async Task<IActionResult> GetAdPerformance(string adId)
        {
            var outcome = await _premiumService.GetAdPerformanceAsync(adId);
            if (outcome.IsSuccessful)
                return Ok(outcome.Value);
            return BadRequest(new { errors = outcome.Errors?.Select(e => e.Message) });
        }

        [HttpPost("ads/{adId}/impression")]
        [AllowAnonymous]
        public async Task<IActionResult> RecordImpression(string adId)
        {
            var outcome = await _premiumService.RecordImpressionAsync(adId);
            return Ok(new { recorded = outcome.Value });
        }

        [HttpPost("ads/{adId}/click")]
        [AllowAnonymous]
        public async Task<IActionResult> RecordClick(string adId)
        {
            var outcome = await _premiumService.RecordClickAsync(adId);
            return Ok(new { recorded = outcome.Value });
        }
    }
}
