using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using UrGuide.Model.Email;
using UrGuide.WebApp.Resources;
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
        private readonly IStringLocalizer<SharedResource> _localizer;

        public EmailTemplateController(IEmailTemplateService emailTemplateService, ILogger<EmailTemplateController> logger, IStringLocalizer<SharedResource> localizer)
        {
            _emailTemplateService = emailTemplateService;
            _logger = logger;
            _localizer = localizer;
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
                return StatusCode(500, new { error = _localizer["EmailTemplate_CreateError"].Value });
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
                return StatusCode(500, new { error = _localizer["EmailTemplate_UpdateError"].Value });
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
                return StatusCode(500, new { error = _localizer["EmailTemplate_RetrieveError"].Value });
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
                return StatusCode(500, new { error = _localizer["EmailTemplate_ListError"].Value });
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
                return StatusCode(500, new { error = _localizer["EmailTemplate_PreviewError"].Value });
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
                return StatusCode(500, new { error = _localizer["EmailTemplate_VersionsError"].Value });
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
                return StatusCode(500, new { error = _localizer["EmailTemplate_DeactivateError"].Value });
            }
        }
    }
}
