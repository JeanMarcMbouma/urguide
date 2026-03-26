using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrGuide.Model.PushNotifications;
using UrGuide.Services.Contracts;
using UrGuide.WebApp.Models;

namespace UrGuide.WebApp.Controllers
{
    /// <summary>
    /// Admin-only CRUD API for push notification templates.
    /// Templates support {{variable_name}} placeholder substitution, multi-language
    /// variants, automatic versioning, and A/B testing via variant groups.
    /// </summary>
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/notification-templates")]
    [ProducesResponseType(500, Type = typeof(ErrorEnvelop<string>))]
    [ProducesResponseType(400, Type = typeof(ErrorEnvelop<string>))]
    public class NotificationTemplateController : Controller
    {
        public NotificationTemplateController(INotificationTemplateService templateService)
        {
            TemplateService = templateService ?? throw new ArgumentNullException(nameof(templateService));
        }

        public INotificationTemplateService TemplateService { get; }

        /// <summary>
        /// List all templates, with optional filtering by category and/or language.
        /// </summary>
        [HttpGet]
        [ProducesDefaultResponseType(typeof(List<NotificationTemplateDto>))]
        public async Task<IActionResult> GetTemplates(
            [FromQuery] string category = null,
            [FromQuery] string language = null,
            CancellationToken cancellationToken = default)
        {
            var result = await TemplateService.GetTemplatesAsync(category, language, cancellationToken);
            return result.IsError ? (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors)) : Ok(result.Value);
        }

        /// <summary>
        /// Get a template by its database ID.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesDefaultResponseType(typeof(NotificationTemplateDto))]
        public async Task<IActionResult> GetTemplateById(string id, CancellationToken cancellationToken = default)
        {
            var result = await TemplateService.GetTemplateByIdAsync(id, cancellationToken);
            return result.IsError ? (IActionResult)NotFound(ErrorEnvelop.CreateFromOutcome(result.Errors)) : Ok(result.Value);
        }

        /// <summary>
        /// Get the active template by logical name and language (defaults to "en").
        /// Falls back to English when the requested language is unavailable.
        /// </summary>
        [HttpGet("by-name/{name}")]
        [ProducesDefaultResponseType(typeof(NotificationTemplateDto))]
        public async Task<IActionResult> GetTemplateByName(
            string name,
            [FromQuery] string language = "en",
            CancellationToken cancellationToken = default)
        {
            var result = await TemplateService.GetTemplateByNameAsync(name, language, cancellationToken);
            return result.IsError ? (IActionResult)NotFound(ErrorEnvelop.CreateFromOutcome(result.Errors)) : Ok(result.Value);
        }

        /// <summary>
        /// Create a new template. Version is auto-incremented per name+language combination.
        /// </summary>
        [HttpPost]
        [ProducesDefaultResponseType(typeof(NotificationTemplateDto))]
        public async Task<IActionResult> CreateTemplate(
            [FromBody] CreateNotificationTemplateRequest request,
            CancellationToken cancellationToken = default)
        {
            var result = await TemplateService.CreateTemplateAsync(request, cancellationToken);
            return result.IsError ? (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors)) : Ok(result.Value);
        }

        /// <summary>
        /// Update a template. Creates a new versioned record; the old version stays for history.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesDefaultResponseType(typeof(NotificationTemplateDto))]
        public async Task<IActionResult> UpdateTemplate(
            string id,
            [FromBody] UpdateNotificationTemplateRequest request,
            CancellationToken cancellationToken = default)
        {
            var result = await TemplateService.UpdateTemplateAsync(id, request, cancellationToken);
            return result.IsError ? (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors)) : Ok(result.Value);
        }

        /// <summary>
        /// Soft-delete (deactivate) a template by ID.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesDefaultResponseType(typeof(bool))]
        public async Task<IActionResult> DeleteTemplate(string id, CancellationToken cancellationToken = default)
        {
            var result = await TemplateService.DeleteTemplateAsync(id, cancellationToken);
            return result.IsError ? (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors)) : NoContent();
        }

        /// <summary>
        /// Preview a rendered template with variable substitution applied.
        /// </summary>
        [HttpPost("{id}/preview")]
        public async Task<IActionResult> PreviewTemplate(
            string id,
            [FromBody] Dictionary<string, string> variables,
            CancellationToken cancellationToken = default)
        {
            var result = await TemplateService.GetTemplateByIdAsync(id, cancellationToken);
            if (result.IsError)
                return NotFound(ErrorEnvelop.CreateFromOutcome(result.Errors));

            var (title, body) = TemplateService.RenderTemplate(result.Value, variables);
            return Ok(new { title, body });
        }
    }
}
