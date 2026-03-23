using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UrGuide.Model.Email;
using UrGuide.Services.Email;

namespace UrGuide.WebApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/email-templates")]
    public class EmailTemplateController : ControllerBase
    {
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly ILogger<EmailTemplateController> _logger;

        public EmailTemplateController(IEmailTemplateService emailTemplateService, ILogger<EmailTemplateController> logger)
        {
            _emailTemplateService = emailTemplateService;
            _logger = logger;
        }

        /// <summary>
        /// Create a new email template
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateTemplate([FromBody] CreateEmailTemplateRequest request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var result = await _emailTemplateService.CreateTemplateAsync(userId, request);
                if (result.IsError)
                {
                    return BadRequest(new { errors = result.Errors });
                }

                return Ok(result.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating email template");
                return StatusCode(500, new { error = "An error occurred while creating the email template" });
            }
        }

        /// <summary>
        /// Update an existing email template (creates a new version)
        /// </summary>
        [HttpPut("{templateId}")]
        public async Task<IActionResult> UpdateTemplate(string templateId, [FromBody] UpdateEmailTemplateRequest request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var result = await _emailTemplateService.UpdateTemplateAsync(userId, templateId, request);
                if (result.IsError)
                {
                    return BadRequest(new { errors = result.Errors });
                }

                return Ok(result.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating email template {TemplateId}", templateId);
                return StatusCode(500, new { error = "An error occurred while updating the email template" });
            }
        }

        /// <summary>
        /// Get a specific email template
        /// </summary>
        [HttpGet("{templateId}")]
        public async Task<IActionResult> GetTemplate(string templateId)
        {
            try
            {
                var result = await _emailTemplateService.GetTemplateAsync(templateId);
                if (result.IsError)
                {
                    return NotFound(new { errors = result.Errors });
                }

                return Ok(result.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving email template {TemplateId}", templateId);
                return StatusCode(500, new { error = "An error occurred while retrieving the email template" });
            }
        }

        /// <summary>
        /// List email templates with optional filters
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetTemplates(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string category = null,
            [FromQuery] string language = null)
        {
            try
            {
                var result = await _emailTemplateService.GetTemplatesAsync(page, pageSize, category, language);
                if (result.IsError)
                {
                    return BadRequest(new { errors = result.Errors });
                }

                return Ok(result.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing email templates");
                return StatusCode(500, new { error = "An error occurred while listing email templates" });
            }
        }

        /// <summary>
        /// Preview an email template with variable substitution
        /// </summary>
        [HttpPost("preview")]
        public async Task<IActionResult> PreviewTemplate([FromBody] EmailPreviewRequest request)
        {
            try
            {
                var result = await _emailTemplateService.PreviewTemplateAsync(request);
                if (result.IsError)
                {
                    return BadRequest(new { errors = result.Errors });
                }

                return Ok(result.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error previewing email template");
                return StatusCode(500, new { error = "An error occurred while previewing the email template" });
            }
        }

        /// <summary>
        /// Get version history for a template
        /// </summary>
        [HttpGet("{templateId}/versions")]
        public async Task<IActionResult> GetTemplateVersions(string templateId)
        {
            try
            {
                var result = await _emailTemplateService.GetTemplateVersionsAsync(templateId);
                if (result.IsError)
                {
                    return NotFound(new { errors = result.Errors });
                }

                return Ok(result.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving versions for template {TemplateId}", templateId);
                return StatusCode(500, new { error = "An error occurred while retrieving template versions" });
            }
        }

        /// <summary>
        /// Deactivate an email template (soft delete)
        /// </summary>
        [HttpDelete("{templateId}")]
        public async Task<IActionResult> DeactivateTemplate(string templateId)
        {
            try
            {
                var result = await _emailTemplateService.DeactivateTemplateAsync(templateId);
                if (result.IsError)
                {
                    return NotFound(new { errors = result.Errors });
                }

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating email template {TemplateId}", templateId);
                return StatusCode(500, new { error = "An error occurred while deactivating the email template" });
            }
        }
    }
}
