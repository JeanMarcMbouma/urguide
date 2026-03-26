using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UrGuide.Services.Financial;
using UrGuide.Model.Financial;

namespace UrGuide.WebApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/financial")]
    public class FinancialController : ControllerBase
    {
        private readonly IFinancialService _financialService;

        public FinancialController(IFinancialService financialService)
        {
            _financialService = financialService;
        }

        // Coin Wallet
        [HttpGet("wallet")]
        public async Task<IActionResult> GetWallet()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var outcome = await _financialService.GetWalletAsync(userId);
            if (outcome.IsSuccessful)
                return Ok(outcome.Value);
            return BadRequest(new { errors = outcome.Errors?.Select(e => e.Message) });
        }

        [HttpPost("wallet/add")]
        public async Task<IActionResult> AddCoins([FromBody] AddCoinsRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var outcome = await _financialService.AddCoinsAsync(userId, request);
            if (outcome.IsSuccessful)
                return Ok(outcome.Value);
            return BadRequest(new { errors = outcome.Errors?.Select(e => e.Message) });
        }

        [HttpPost("wallet/spend")]
        public async Task<IActionResult> SpendCoins([FromBody] SpendCoinsRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var outcome = await _financialService.SpendCoinsAsync(userId, request);
            if (outcome.IsSuccessful)
                return Ok(outcome.Value);
            return BadRequest(new { errors = outcome.Errors?.Select(e => e.Message) });
        }

        [HttpGet("wallet/transactions")]
        public async Task<IActionResult> GetTransactions([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var outcome = await _financialService.GetTransactionsAsync(userId, page, pageSize);
            if (outcome.IsSuccessful)
                return Ok(outcome.Value);
            return BadRequest(new { errors = outcome.Errors?.Select(e => e.Message) });
        }

        // Withdrawals
        [HttpPost("withdrawal")]
        public async Task<IActionResult> CreateWithdrawal([FromBody] CreateWithdrawalRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var outcome = await _financialService.CreateWithdrawalAsync(userId, request);
            if (outcome.IsSuccessful)
                return Ok(outcome.Value);
            return BadRequest(new { errors = outcome.Errors?.Select(e => e.Message) });
        }

        [HttpGet("withdrawal/{withdrawalId}")]
        public async Task<IActionResult> GetWithdrawal(string withdrawalId)
        {
            var outcome = await _financialService.GetWithdrawalAsync(withdrawalId);
            if (outcome.IsSuccessful)
                return Ok(outcome.Value);
            return BadRequest(new { errors = outcome.Errors?.Select(e => e.Message) });
        }

        [HttpGet("withdrawals")]
        public async Task<IActionResult> GetUserWithdrawals([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var outcome = await _financialService.GetUserWithdrawalsAsync(userId, page, pageSize);
            if (outcome.IsSuccessful)
                return Ok(outcome.Value);
            return BadRequest(new { errors = outcome.Errors?.Select(e => e.Message) });
        }

        [HttpPost("withdrawal/{withdrawalId}/process")]
        public async Task<IActionResult> ProcessWithdrawal(string withdrawalId)
        {
            var outcome = await _financialService.ProcessWithdrawalAsync(withdrawalId);
            if (outcome.IsSuccessful)
                return Ok(outcome.Value);
            return BadRequest(new { errors = outcome.Errors?.Select(e => e.Message) });
        }

        [HttpPost("withdrawal/{withdrawalId}/cancel")]
        public async Task<IActionResult> CancelWithdrawal(string withdrawalId)
        {
            var outcome = await _financialService.CancelWithdrawalAsync(withdrawalId);
            if (outcome.IsSuccessful)
                return Ok(outcome.Value);
            return BadRequest(new { errors = outcome.Errors?.Select(e => e.Message) });
        }

        // Payout Schedules
        [HttpPost("payout-schedule")]
        public async Task<IActionResult> CreatePayoutSchedule([FromBody] CreatePayoutScheduleRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var outcome = await _financialService.CreatePayoutScheduleAsync(userId, request);
            if (outcome.IsSuccessful)
                return Ok(outcome.Value);
            return BadRequest(new { errors = outcome.Errors?.Select(e => e.Message) });
        }

        [HttpGet("payout-schedule")]
        public async Task<IActionResult> GetPayoutSchedule()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var outcome = await _financialService.GetPayoutScheduleAsync(userId);
            if (outcome.IsSuccessful)
                return Ok(outcome.Value);
            return BadRequest(new { errors = outcome.Errors?.Select(e => e.Message) });
        }

        [HttpPut("payout-schedule")]
        public async Task<IActionResult> UpdatePayoutSchedule([FromBody] UpdatePayoutScheduleRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var outcome = await _financialService.UpdatePayoutScheduleAsync(userId, request);
            if (outcome.IsSuccessful)
                return Ok(outcome.Value);
            return BadRequest(new { errors = outcome.Errors?.Select(e => e.Message) });
        }

        // Financial Reporting
        [HttpPost("report")]
        public async Task<IActionResult> GenerateFinancialReport([FromBody] FinancialReportRequest request)
        {
            var outcome = await _financialService.GenerateFinancialReportAsync(request);
            if (outcome.IsSuccessful)
                return Ok(outcome.Value);
            return BadRequest(new { errors = outcome.Errors?.Select(e => e.Message) });
        }
    }
}
