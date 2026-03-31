using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using UrGuide.Data;
using UrGuide.WebApp.Resources;
using UrGuide.Data.Entities.Users;
using UrGuide.Model.Admin;
using UrGuide.WebApp.Models;

namespace UrGuide.WebApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/guide-verification")]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public class GuideVerificationController : ControllerBase
    {
        private readonly UrGuideContext _context;
        private readonly ILogger<GuideVerificationController> _logger;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public GuideVerificationController(UrGuideContext context, ILogger<GuideVerificationController> logger, IStringLocalizer<SharedResource> localizer)
        {
            _context = context;
            _logger = logger;
            _localizer = localizer;
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        /// <summary>
        /// Get the current KYC verification status for the authenticated guide
        /// </summary>
        [HttpGet("status")]
        public async Task<IActionResult> GetStatus(CancellationToken cancellationToken)
        {
            var guideId = GetUserId();
            if (string.IsNullOrEmpty(guideId)) return Unauthorized();

            var submission = await _context.GuideVerificationSubmissions
                .Include(s => s.Documents)
                .FirstOrDefaultAsync(s => s.GuideId == guideId, cancellationToken);

            if (submission == null)
            {
                return Ok(new GuideVerificationStatusResponse
                {
                    GuideId = guideId,
                    OverallStatus = "not_submitted",
                    Documents = new System.Collections.Generic.List<VerificationDocumentModel>(),
                });
            }

            return Ok(MapToResponse(submission));
        }

        /// <summary>
        /// Submit a verification document
        /// </summary>
        [HttpPost("documents")]
        public async Task<IActionResult> SubmitDocument([FromBody] SubmitVerificationDocumentRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var guideId = GetUserId();
                if (string.IsNullOrEmpty(guideId)) return Unauthorized();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var now = DateTime.UtcNow;

                var submission = await _context.GuideVerificationSubmissions
                    .Include(s => s.Documents)
                    .FirstOrDefaultAsync(s => s.GuideId == guideId, cancellationToken);

                if (submission == null)
                {
                    submission = new GuideVerificationSubmission
                    {
                        Id = Guid.NewGuid().ToString(),
                        GuideId = guideId,
                        Status = (int)GuideVerificationStatus.Pending,
                        SubmittedAt = now,
                        CreatedAt = now,
                        UpdatedAt = now,
                    };
                    await _context.GuideVerificationSubmissions.AddAsync(submission, cancellationToken);
                }
                else
                {
                    // If previously rejected/approved and guide is resubmitting, move back to pending
                    if (submission.Status == (int)GuideVerificationStatus.Rejected ||
                        submission.Status == (int)GuideVerificationStatus.Approved)
                    {
                        submission.Status = (int)GuideVerificationStatus.Pending;
                        submission.SubmittedAt = now;
                    }
                    submission.UpdatedAt = now;
                }

                // Remove any existing document of the same type for this submission
                var existing = submission.Documents.FirstOrDefault(d => d.DocumentType == request.DocumentType);
                if (existing != null)
                    _context.GuideVerificationDocuments.Remove(existing);

                var doc = new GuideVerificationDocument
                {
                    Id = Guid.NewGuid().ToString(),
                    SubmissionId = submission.Id,
                    DocumentType = request.DocumentType,
                    FileName = request.FileName,
                    FileBase64 = request.FileBase64,
                    Status = 0, // submitted
                    UploadedAt = now,
                    CreatedAt = now,
                };
                await _context.GuideVerificationDocuments.AddAsync(doc, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                return Ok(new VerificationDocumentModel
                {
                    Id = doc.Id,
                    Type = doc.DocumentType,
                    FileName = doc.FileName,
                    Status = "submitted",
                    UploadedAt = doc.UploadedAt,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting verification document");
                return StatusCode(500, new { error = _localizer["Verification_SubmitError"].Value });
            }
        }

        /// <summary>
        /// [Admin] Get all pending verification submissions
        /// </summary>
        [HttpGet("pending")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPendingSubmissions(CancellationToken cancellationToken)
        {
            var pending = await _context.GuideVerificationSubmissions
                .Include(s => s.Documents)
                .Include(s => s.Guide)
                .Where(s => s.Status == (int)GuideVerificationStatus.Pending || s.Status == (int)GuideVerificationStatus.UnderReview)
                .OrderBy(s => s.SubmittedAt)
                .ToListAsync(cancellationToken);

            var result = pending.Select(s => new
            {
                submissionId = s.Id,
                guideId = s.GuideId,
                status = ((GuideVerificationStatus)s.Status).ToString().ToLowerInvariant(),
                submittedAt = s.SubmittedAt,
                documentCount = s.Documents.Count,
            });

            return Ok(result);
        }

        /// <summary>
        /// [Admin] Approve a verification submission
        /// </summary>
        [HttpPost("{submissionId}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveVerification(string submissionId, [FromBody] VerificationDecisionRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var adminId = GetUserId();
                var submission = await _context.GuideVerificationSubmissions
                    .Include(s => s.Documents)
                    .FirstOrDefaultAsync(s => s.Id == submissionId, cancellationToken);

                if (submission == null)
                    return NotFound(new { error = _localizer["Verification_NotFound"].Value });

                var now = DateTime.UtcNow;
                submission.Status = (int)GuideVerificationStatus.Approved;
                submission.ReviewedAt = now;
                submission.ReviewedByAdminId = adminId;
                submission.AdminNotes = request?.Notes;
                submission.UpdatedAt = now;

                // Mark all documents as verified
                foreach (var doc in submission.Documents)
                    doc.Status = 1; // verified

                await _context.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Guide verification {SubmissionId} approved by admin {AdminId}", submissionId, adminId);

                return Ok(new { message = _localizer["Verification_ApprovedSuccess"].Value, submissionId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving verification {SubmissionId}", submissionId);
                return StatusCode(500, new { error = _localizer["Verification_ApproveError"].Value });
            }
        }

        /// <summary>
        /// [Admin] Reject a verification submission
        /// </summary>
        [HttpPost("{submissionId}/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RejectVerification(string submissionId, [FromBody] VerificationDecisionRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var adminId = GetUserId();
                var submission = await _context.GuideVerificationSubmissions
                    .FirstOrDefaultAsync(s => s.Id == submissionId, cancellationToken);

                if (submission == null)
                    return NotFound(new { error = _localizer["Verification_NotFound"].Value });

                var now = DateTime.UtcNow;
                submission.Status = (int)GuideVerificationStatus.Rejected;
                submission.ReviewedAt = now;
                submission.ReviewedByAdminId = adminId;
                submission.RejectionReason = request?.Reason;
                submission.AdminNotes = request?.Notes;
                submission.UpdatedAt = now;

                await _context.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Guide verification {SubmissionId} rejected by admin {AdminId}. Reason: {Reason}", submissionId, adminId, request?.Reason);

                return Ok(new { message = _localizer["Verification_RejectedSuccess"].Value, submissionId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting verification {SubmissionId}", submissionId);
                return StatusCode(500, new { error = _localizer["Verification_RejectError"].Value });
            }
        }

        private static GuideVerificationStatusResponse MapToResponse(GuideVerificationSubmission submission)
        {
            var statusStr = ((GuideVerificationStatus)submission.Status) switch
            {
                GuideVerificationStatus.Pending => "pending",
                GuideVerificationStatus.UnderReview => "under_review",
                GuideVerificationStatus.Approved => "verified",
                GuideVerificationStatus.Rejected => "rejected",
                _ => "not_submitted"
            };

            return new GuideVerificationStatusResponse
            {
                GuideId = submission.GuideId,
                OverallStatus = statusStr,
                SubmittedAt = submission.SubmittedAt,
                ReviewedAt = submission.ReviewedAt,
                Notes = submission.AdminNotes ?? submission.RejectionReason,
                Documents = submission.Documents.Select(d => new VerificationDocumentModel
                {
                    Id = d.Id,
                    Type = d.DocumentType,
                    FileName = d.FileName,
                    Status = d.Status == 0 ? "submitted" : d.Status == 1 ? "verified" : "rejected",
                    UploadedAt = d.UploadedAt,
                }).ToList(),
            };
        }
    }
}
