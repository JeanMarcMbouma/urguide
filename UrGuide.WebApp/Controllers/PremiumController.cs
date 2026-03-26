using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UrGuide.Services.Premium;
using UrGuide.Model.Premium;
using UrGuide.WebApp.Models;
using BbQ.Outcome;

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

        [HttpPost("plans")]
        public async Task<IActionResult> CreatePlan([FromBody] CreateSubscriptionPlanRequest request)
        {
            var result = await _premiumService.CreatePlanAsync(request);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        [HttpGet("plans")]
        public async Task<IActionResult> GetAllPlans()
        {
            var result = await _premiumService.GetAllPlansAsync();
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        [HttpGet("plans/{planId}")]
        public async Task<IActionResult> GetPlan(string planId)
        {
            var result = await _premiumService.GetPlanAsync(planId);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        [HttpPost("subscribe")]
        public async Task<IActionResult> Subscribe([FromBody] SubscribeRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _premiumService.SubscribeAsync(userId, request);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        [HttpGet("subscription")]
        public async Task<IActionResult> GetSubscription()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _premiumService.GetGuideSubscriptionAsync(userId);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        [HttpPost("subscription/cancel")]
        public async Task<IActionResult> CancelSubscription()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _premiumService.CancelSubscriptionAsync(userId);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(new { cancelled = value }),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        [HttpPost("boosts")]
        public async Task<IActionResult> CreateBoost([FromBody] CreateVisibilityBoostRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _premiumService.CreateBoostAsync(userId, request);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        [HttpGet("boosts")]
        public async Task<IActionResult> GetActiveBoosts()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _premiumService.GetActiveBoostsAsync(userId);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        [HttpPost("ads")]
        public async Task<IActionResult> CreateAdvertisement([FromBody] CreateAdvertisementRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _premiumService.CreateAdvertisementAsync(userId, request);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        [HttpGet("ads/{adId}")]
        public async Task<IActionResult> GetAdvertisement(string adId)
        {
            var result = await _premiumService.GetAdvertisementAsync(adId);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        [HttpGet("ads")]
        public async Task<IActionResult> GetMyAds([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _premiumService.GetAdvertiserAdsAsync(userId, page, pageSize);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        [HttpPut("ads/{adId}")]
        public async Task<IActionResult> UpdateAdvertisement(string adId, [FromBody] UpdateAdvertisementRequest request)
        {
            var result = await _premiumService.UpdateAdvertisementAsync(adId, request);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        [HttpGet("ads/{adId}/performance")]
        public async Task<IActionResult> GetAdPerformance(string adId)
        {
            var result = await _premiumService.GetAdPerformanceAsync(adId);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        [HttpPost("ads/{adId}/impression")]
        [AllowAnonymous]
        public async Task<IActionResult> RecordImpression(string adId)
        {
            var result = await _premiumService.RecordImpressionAsync(adId);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(new { recorded = value }),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        [HttpPost("ads/{adId}/click")]
        [AllowAnonymous]
        public async Task<IActionResult> RecordClick(string adId)
        {
            var result = await _premiumService.RecordClickAsync(adId);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(new { recorded = value }),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }
    }
}
