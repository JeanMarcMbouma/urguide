using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UrGuide.Services.Gamification;
using UrGuide.Model.Gamification;

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

        // Dashboard
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var outcome = await _gamificationService.GetDashboardAsync(userId);
            if (outcome.IsSuccessful)
                return Ok(outcome.Value);
            return BadRequest(new { errors = outcome.Errors?.Select(e => e.Message) });
        }

        // Loyalty
        [HttpGet("loyalty")]
        public async Task<IActionResult> GetLoyaltyAccount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var outcome = await _gamificationService.GetLoyaltyAccountAsync(userId);
            if (outcome.IsSuccessful)
                return Ok(outcome.Value);
            return BadRequest(new { errors = outcome.Errors?.Select(e => e.Message) });
        }

        [HttpPost("loyalty/earn")]
        public async Task<IActionResult> EarnPoints([FromBody] EarnPointsRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var outcome = await _gamificationService.EarnPointsAsync(userId, request);
            if (outcome.IsSuccessful)
                return Ok(outcome.Value);
            return BadRequest(new { errors = outcome.Errors?.Select(e => e.Message) });
        }

        [HttpPost("loyalty/redeem")]
        public async Task<IActionResult> RedeemPoints([FromBody] RedeemPointsRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var outcome = await _gamificationService.RedeemPointsAsync(userId, request);
            if (outcome.IsSuccessful)
                return Ok(outcome.Value);
            return BadRequest(new { errors = outcome.Errors?.Select(e => e.Message) });
        }

        [HttpGet("loyalty/history")]
        public async Task<IActionResult> GetLoyaltyHistory([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var outcome = await _gamificationService.GetLoyaltyHistoryAsync(userId, page, pageSize);
            if (outcome.IsSuccessful)
                return Ok(outcome.Value);
            return BadRequest(new { errors = outcome.Errors?.Select(e => e.Message) });
        }

        // Badges
        [HttpPost("badges")]
        public async Task<IActionResult> CreateBadge([FromBody] CreateBadgeRequest request)
        {
            var outcome = await _gamificationService.CreateBadgeAsync(request);
            if (outcome.IsSuccessful)
                return Ok(outcome.Value);
            return BadRequest(new { errors = outcome.Errors?.Select(e => e.Message) });
        }

        [HttpGet("badges")]
        public async Task<IActionResult> GetAllBadges()
        {
            var outcome = await _gamificationService.GetAllBadgesAsync();
            if (outcome.IsSuccessful)
                return Ok(outcome.Value);
            return BadRequest(new { errors = outcome.Errors?.Select(e => e.Message) });
        }

        [HttpGet("badges/user")]
        public async Task<IActionResult> GetUserBadges()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var outcome = await _gamificationService.GetUserBadgesAsync(userId);
            if (outcome.IsSuccessful)
                return Ok(outcome.Value);
            return BadRequest(new { errors = outcome.Errors?.Select(e => e.Message) });
        }

        [HttpPost("badges/award/{badgeId}")]
        public async Task<IActionResult> AwardBadge(string badgeId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var outcome = await _gamificationService.AwardBadgeAsync(userId, badgeId);
            if (outcome.IsSuccessful)
                return Ok(outcome.Value);
            return BadRequest(new { errors = outcome.Errors?.Select(e => e.Message) });
        }

        // Lottery
        [HttpPost("lottery")]
        public async Task<IActionResult> CreateLotteryDraw([FromBody] CreateLotteryDrawRequest request)
        {
            var outcome = await _gamificationService.CreateLotteryDrawAsync(request);
            if (outcome.IsSuccessful)
                return Ok(outcome.Value);
            return BadRequest(new { errors = outcome.Errors?.Select(e => e.Message) });
        }

        [HttpGet("lottery/{drawId}")]
        public async Task<IActionResult> GetLotteryDraw(string drawId)
        {
            var outcome = await _gamificationService.GetLotteryDrawAsync(drawId);
            if (outcome.IsSuccessful)
                return Ok(outcome.Value);
            return BadRequest(new { errors = outcome.Errors?.Select(e => e.Message) });
        }

        [HttpGet("lottery/active")]
        public async Task<IActionResult> GetActiveLotteries()
        {
            var outcome = await _gamificationService.GetActiveLotteriesAsync();
            if (outcome.IsSuccessful)
                return Ok(outcome.Value);
            return BadRequest(new { errors = outcome.Errors?.Select(e => e.Message) });
        }

        [HttpPost("lottery/{drawId}/enter")]
        public async Task<IActionResult> EnterLottery(string drawId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var outcome = await _gamificationService.EnterLotteryAsync(userId, drawId);
            if (outcome.IsSuccessful)
                return Ok(new { entered = outcome.Value });
            return BadRequest(new { errors = outcome.Errors?.Select(e => e.Message) });
        }

        [HttpPost("lottery/{drawId}/draw")]
        public async Task<IActionResult> DrawWinners(string drawId)
        {
            var outcome = await _gamificationService.DrawWinnersAsync(drawId);
            if (outcome.IsSuccessful)
                return Ok(outcome.Value);
            return BadRequest(new { errors = outcome.Errors?.Select(e => e.Message) });
        }

        // Achievements
        [HttpPost("achievements")]
        public async Task<IActionResult> CreateAchievement([FromBody] CreateAchievementRequest request)
        {
            var outcome = await _gamificationService.CreateAchievementAsync(request);
            if (outcome.IsSuccessful)
                return Ok(outcome.Value);
            return BadRequest(new { errors = outcome.Errors?.Select(e => e.Message) });
        }

        [HttpGet("achievements")]
        public async Task<IActionResult> GetAllAchievements()
        {
            var outcome = await _gamificationService.GetAllAchievementsAsync();
            if (outcome.IsSuccessful)
                return Ok(outcome.Value);
            return BadRequest(new { errors = outcome.Errors?.Select(e => e.Message) });
        }

        [HttpGet("achievements/user")]
        public async Task<IActionResult> GetUserAchievements()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var outcome = await _gamificationService.GetUserAchievementsAsync(userId);
            if (outcome.IsSuccessful)
                return Ok(outcome.Value);
            return BadRequest(new { errors = outcome.Errors?.Select(e => e.Message) });
        }

        [HttpPost("achievements/progress")]
        public async Task<IActionResult> UpdateProgress([FromBody] UpdateProgressRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var outcome = await _gamificationService.UpdateProgressAsync(userId, request);
            if (outcome.IsSuccessful)
                return Ok(outcome.Value);
            return BadRequest(new { errors = outcome.Errors?.Select(e => e.Message) });
        }
    }
}
