using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UrGuide.Services.Financial;
using UrGuide.Model.Financial;
using UrGuide.WebApp.Models;
using BbQ.Outcome;

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

        [HttpGet("wallet")]
        public async Task<IActionResult> GetWallet()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _financialService.GetWalletAsync(userId);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        [HttpPost("wallet/add")]
        public async Task<IActionResult> AddCoins([FromBody] AddCoinsRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _financialService.AddCoinsAsync(userId, request);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        [HttpPost("wallet/spend")]
        public async Task<IActionResult> SpendCoins([FromBody] SpendCoinsRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _financialService.SpendCoinsAsync(userId, request);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        [HttpGet("wallet/transactions")]
        public async Task<IActionResult> GetTransactions([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 20;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _financialService.GetTransactionsAsync(userId, page, pageSize);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        [HttpPost("withdrawal")]
        public async Task<IActionResult> CreateWithdrawal([FromBody] CreateWithdrawalRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _financialService.CreateWithdrawalAsync(userId, request);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        [HttpGet("withdrawal/{withdrawalId}")]
        public async Task<IActionResult> GetWithdrawal(string withdrawalId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _financialService.GetWithdrawalAsync(withdrawalId, userId);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        [HttpGet("withdrawals")]
        public async Task<IActionResult> GetUserWithdrawals([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _financialService.GetUserWithdrawalsAsync(userId, page, pageSize);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("withdrawal/{withdrawalId}/process")]
        public async Task<IActionResult> ProcessWithdrawal(string withdrawalId)
        {
            var result = await _financialService.ProcessWithdrawalAsync(withdrawalId);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("withdrawal/{withdrawalId}/complete")]
        public async Task<IActionResult> CompleteWithdrawal(string withdrawalId, [FromBody] CompleteWithdrawalRequest request)
        {
            var result = await _financialService.CompleteWithdrawalAsync(withdrawalId, request.TransactionReference);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("withdrawal/{withdrawalId}/fail")]
        public async Task<IActionResult> FailWithdrawal(string withdrawalId, [FromBody] FailWithdrawalRequest request)
        {
            var result = await _financialService.FailWithdrawalAsync(withdrawalId, request.FailureReason);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        [HttpPost("withdrawal/{withdrawalId}/cancel")]
        public async Task<IActionResult> CancelWithdrawal(string withdrawalId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _financialService.CancelWithdrawalAsync(withdrawalId, userId);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        [HttpPost("payout-schedule")]
        public async Task<IActionResult> CreatePayoutSchedule([FromBody] CreatePayoutScheduleRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _financialService.CreatePayoutScheduleAsync(userId, request);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        [HttpGet("payout-schedule")]
        public async Task<IActionResult> GetPayoutSchedule()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _financialService.GetPayoutScheduleAsync(userId);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        [HttpPut("payout-schedule")]
        public async Task<IActionResult> UpdatePayoutSchedule([FromBody] UpdatePayoutScheduleRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _financialService.UpdatePayoutScheduleAsync(userId, request);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("report")]
        public async Task<IActionResult> GenerateFinancialReport([FromBody] FinancialReportRequest request)
        {
            var result = await _financialService.GenerateFinancialReportAsync(request);
            return result.Match(
                onSuccess: value => (IActionResult)Ok(value),
                onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
        }
    }
}
