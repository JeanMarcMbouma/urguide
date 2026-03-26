using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UrGuide.Services.Gamification;
using UrGuide.Model.Gamification;
using UrGuide.WebApp.Models;
using BbQ.Outcome;

namespace UrGuide.WebApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/gamification")]
    public class GamificationController : ControllerBase
    {
        private readonly IGamificationService _gamificationService;

        public GamificationController(IGamificationService gamificationService)
        {
            _gamificationService = gamificationService;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _gamificationService.GetDashboardAsync(userId);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        [HttpGet("loyalty")]
        public async Task<IActionResult> GetLoyaltyAccount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _gamificationService.GetLoyaltyAccountAsync(userId);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        [HttpPost("loyalty/earn")]
        public async Task<IActionResult> EarnPoints([FromBody] EarnPointsRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _gamificationService.EarnPointsAsync(userId, request);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        [HttpPost("loyalty/redeem")]
        public async Task<IActionResult> RedeemPoints([FromBody] RedeemPointsRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _gamificationService.RedeemPointsAsync(userId, request);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        [HttpGet("loyalty/history")]
        public async Task<IActionResult> GetLoyaltyHistory([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _gamificationService.GetLoyaltyHistoryAsync(userId, page, pageSize);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        [HttpPost("badges")]
        public async Task<IActionResult> CreateBadge([FromBody] CreateBadgeRequest request)
        {
            var result = await _gamificationService.CreateBadgeAsync(request);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        [HttpGet("badges")]
        public async Task<IActionResult> GetAllBadges()
        {
            var result = await _gamificationService.GetAllBadgesAsync();
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        [HttpGet("badges/user")]
        public async Task<IActionResult> GetUserBadges()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _gamificationService.GetUserBadgesAsync(userId);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        [HttpPost("badges/award/{badgeId}")]
        public async Task<IActionResult> AwardBadge(string badgeId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _gamificationService.AwardBadgeAsync(userId, badgeId);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        [HttpPost("lottery")]
        public async Task<IActionResult> CreateLotteryDraw([FromBody] CreateLotteryDrawRequest request)
        {
            var result = await _gamificationService.CreateLotteryDrawAsync(request);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        [HttpGet("lottery/{drawId}")]
        public async Task<IActionResult> GetLotteryDraw(string drawId)
        {
            var result = await _gamificationService.GetLotteryDrawAsync(drawId);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        [HttpGet("lottery/active")]
        public async Task<IActionResult> GetActiveLotteries()
        {
            var result = await _gamificationService.GetActiveLotteriesAsync();
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        [HttpPost("lottery/{drawId}/enter")]
        public async Task<IActionResult> EnterLottery(string drawId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _gamificationService.EnterLotteryAsync(userId, drawId);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(new { entered = value }),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        [HttpPost("lottery/{drawId}/draw")]
        public async Task<IActionResult> DrawWinners(string drawId)
        {
            var result = await _gamificationService.DrawWinnersAsync(drawId);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        [HttpPost("achievements")]
        public async Task<IActionResult> CreateAchievement([FromBody] CreateAchievementRequest request)
        {
            var result = await _gamificationService.CreateAchievementAsync(request);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        [HttpGet("achievements")]
        public async Task<IActionResult> GetAllAchievements()
        {
            var result = await _gamificationService.GetAllAchievementsAsync();
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        [HttpGet("achievements/user")]
        public async Task<IActionResult> GetUserAchievements()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _gamificationService.GetUserAchievementsAsync(userId);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        [HttpPost("achievements/progress")]
        public async Task<IActionResult> UpdateProgress([FromBody] UpdateProgressRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _gamificationService.UpdateProgressAsync(userId, request);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }
    }
}
