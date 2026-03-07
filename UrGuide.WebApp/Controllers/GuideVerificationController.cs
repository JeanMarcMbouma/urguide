using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<GuideVerificationController> _logger;
        private static readonly ConcurrentDictionary<string, GuideVerificationStatusResponse> _verificationStore = new();

        public GuideVerificationController(ILogger<GuideVerificationController> logger)
        {
            _logger = logger;
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        /// <summary>
        /// Get the current KYC verification status for the authenticated guide
        /// </summary>
        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            var guideId = GetUserId();
            if (string.IsNullOrEmpty(guideId)) return Unauthorized();

            if (_verificationStore.TryGetValue(guideId, out var status))
                return Ok(status);

            return Ok(new GuideVerificationStatusResponse
            {
                GuideId = guideId,
                OverallStatus = "not_submitted",
                Documents = new List<VerificationDocumentModel>(),
            });
        }

        /// <summary>
        /// Submit a verification document
        /// </summary>
        [HttpPost("documents")]
        public IActionResult SubmitDocument([FromBody] SubmitVerificationDocumentRequest request)
        {
            try
            {
                var guideId = GetUserId();
                if (string.IsNullOrEmpty(guideId)) return Unauthorized();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (!_verificationStore.ContainsKey(guideId))
                {
                    _verificationStore.TryAdd(guideId, new GuideVerificationStatusResponse
                    {
                        GuideId = guideId,
                        OverallStatus = "pending",
                        Documents = new List<VerificationDocumentModel>(),
                        SubmittedAt = DateTime.UtcNow,
                    });
                }

                var verification = _verificationStore[guideId];
                verification.OverallStatus = "pending";

                // Remove any existing document of the same type
                verification.Documents.RemoveAll(d => d.Type == request.DocumentType);

                var doc = new VerificationDocumentModel
                {
                    Id = Guid.NewGuid().ToString(),
                    Type = request.DocumentType,
                    FileName = request.FileName,
                    Status = "submitted",
                    UploadedAt = DateTime.UtcNow,
                };
                verification.Documents.Add(doc);

                return Ok(doc);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting verification document");
                return StatusCode(500, new { error = "An error occurred while submitting the document" });
            }
        }
    }
}
