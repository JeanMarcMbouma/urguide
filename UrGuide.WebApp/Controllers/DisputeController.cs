using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using UrGuide.Model.Disputes;
using UrGuide.WebApp.Resources;
using UrGuide.Services.Disputes;

namespace UrGuide.WebApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DisputeController : ControllerBase
    {
        private readonly IDisputeService _disputeService;
        private readonly ILogger<DisputeController> _logger;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public DisputeController(IDisputeService disputeService, ILogger<DisputeController> logger, IStringLocalizer<SharedResource> localizer)
        {
            _disputeService = disputeService;
            _logger = logger;
            _localizer = localizer;
        }

        /// <summary>
        /// Create a new dispute
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateDispute([FromBody] CreateDisputeRequest request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var dispute = await _disputeService.CreateDisputeAsync(userId, request);
                return Ok(dispute);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid dispute request");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating dispute");
                return StatusCode(500, new { error = _localizer["Dispute_CreateError"].Value });
            }
        }

        /// <summary>
        /// Get dispute details
        /// </summary>
        [HttpGet("{disputeId}")]
        public async Task<IActionResult> GetDispute(string disputeId)
        {
            try
            {
                var dispute = await _disputeService.GetDisputeAsync(disputeId);
                return Ok(dispute);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving dispute");
                return StatusCode(500, new { error = _localizer["Dispute_RetrieveError"].Value });
            }
        }

        /// <summary>
        /// Get current user's disputes
        /// </summary>
        [HttpGet("my")]
        public async Task<IActionResult> GetMyDisputes([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var disputes = await _disputeService.GetUserDisputesAsync(userId, page, pageSize);
                return Ok(disputes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user disputes");
                return StatusCode(500, new { error = _localizer["Dispute_ListError"].Value });
            }
        }

        /// <summary>
        /// Get admin dispute queue
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("admin/queue")]
        public async Task<IActionResult> GetAdminDisputeQueue([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] int? status = null, [FromQuery] int? priority = null)
        {
            try
            {
                var disputes = await _disputeService.GetAdminDisputeQueueAsync(page, pageSize, status, priority);
                return Ok(disputes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving admin dispute queue");
                return StatusCode(500, new { error = _localizer["Dispute_QueueError"].Value });
            }
        }

        /// <summary>
        /// Submit evidence for a dispute
        /// </summary>
        [HttpPost("{disputeId}/evidence")]
        public async Task<IActionResult> SubmitEvidence(string disputeId, [FromBody] SubmitEvidenceRequest request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var evidence = await _disputeService.SubmitEvidenceAsync(userId, disputeId, request);
                return Ok(evidence);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid evidence submission");
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Evidence submission not allowed");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting evidence");
                return StatusCode(500, new { error = _localizer["Dispute_SubmitEvidenceError"].Value });
            }
        }

        /// <summary>
        /// Add a message to a dispute
        /// </summary>
        [HttpPost("{disputeId}/messages")]
        public async Task<IActionResult> AddMessage(string disputeId, [FromBody] DisputeMessageRequest request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var message = await _disputeService.AddMessageAsync(userId, disputeId, request);
                return Ok(message);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid message request");
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Message not allowed");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding message");
                return StatusCode(500, new { error = _localizer["Dispute_AddMessageError"].Value });
            }
        }

        /// <summary>
        /// Assign dispute to admin
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost("{disputeId}/assign")]
        public async Task<IActionResult> AssignDispute(string disputeId)
        {
            try
            {
                var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(adminId))
                {
                    return Unauthorized();
                }

                var success = await _disputeService.AssignDisputeAsync(adminId, disputeId);
                if (!success)
                {
                    return NotFound(new { error = _localizer["Dispute_NotFound"].Value });
                }

                return Ok(new { message = _localizer["Dispute_AssignSuccess"].Value });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning dispute");
                return StatusCode(500, new { error = _localizer["Dispute_AssignError"].Value });
            }
        }

        /// <summary>
        /// Resolve a dispute
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost("{disputeId}/resolve")]
        public async Task<IActionResult> ResolveDispute(string disputeId, [FromBody] ResolveDisputeRequest request)
        {
            try
            {
                var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(adminId))
                {
                    return Unauthorized();
                }

                var dispute = await _disputeService.ResolveDisputeAsync(adminId, disputeId, request);
                return Ok(dispute);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resolving dispute");
                return StatusCode(500, new { error = _localizer["Dispute_ResolveError"].Value });
            }
        }

        /// <summary>
        /// Escalate a dispute
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost("{disputeId}/escalate")]
        public async Task<IActionResult> EscalateDispute(string disputeId)
        {
            try
            {
                var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(adminId))
                {
                    return Unauthorized();
                }

                var success = await _disputeService.EscalateDisputeAsync(adminId, disputeId);
                if (!success)
                {
                    return NotFound(new { error = _localizer["Dispute_NotFound"].Value });
                }

                return Ok(new { message = _localizer["Dispute_EscalateSuccess"].Value });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error escalating dispute");
                return StatusCode(500, new { error = _localizer["Dispute_EscalateError"].Value });
            }
        }

        /// <summary>
        /// Get dispute statistics
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("admin/stats")]
        public async Task<IActionResult> GetDisputeStats()
        {
            try
            {
                var stats = await _disputeService.GetDisputeStatsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving dispute stats");
                return StatusCode(500, new { error = _localizer["Dispute_StatsError"].Value });
            }
        }
    }
}
