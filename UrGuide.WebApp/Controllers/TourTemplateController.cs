using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Core;
using UrGuide.Model.Templates;
using UrGuide.Services.Templates;
using UrGuide.Shared.Contracts;
using UrGuide.WebApp.Models;

namespace UrGuide.WebApp.Controllers
{
    [ApiController]
    [Route("api/tour-templates")]
    [Authorize]
    [ProducesResponseType(400, Type = typeof(ErrorEnvelop<string>))]
    [ProducesResponseType(500, Type = typeof(ErrorEnvelop<string>))]
    public class TourTemplateController : ControllerBase
    {
        private readonly ITourTemplateService _tourTemplateService;
        private readonly IUserContext _userContext;

        public TourTemplateController(ITourTemplateService tourTemplateService, IUserContext userContext)
        {
            _tourTemplateService = tourTemplateService;
            _userContext = userContext;
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(TourTemplateDto))]
        public async Task<IActionResult> CreateTemplate([FromBody] CreateTourTemplateRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ErrorEnvelop.Create(ModelState));

            var result = await _tourTemplateService.CreateTemplateAsync(_userContext.UserId, request, cancellationToken);
            return result.IsError
                ? BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors))
                : (IActionResult)Created($"/api/tour-templates/{result.Value.TemplateId}", result.Value);
        }

        [HttpPut("{templateId}")]
        [ProducesResponseType(200, Type = typeof(TourTemplateDto))]
        public async Task<IActionResult> UpdateTemplate(string templateId, [FromBody] UpdateTourTemplateRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ErrorEnvelop.Create(ModelState));

            var result = await _tourTemplateService.UpdateTemplateAsync(_userContext.UserId, templateId, request, cancellationToken);
            return result.IsError
                ? BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors))
                : (IActionResult)Ok(result.Value);
        }

        [HttpDelete("{templateId}")]
        [ProducesResponseType(200, Type = typeof(bool))]
        public async Task<IActionResult> DeleteTemplate(string templateId, CancellationToken cancellationToken)
        {
            var result = await _tourTemplateService.DeleteTemplateAsync(_userContext.UserId, templateId, cancellationToken);
            return result.IsError
                ? BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors))
                : (IActionResult)Ok(result.Value);
        }

        [HttpGet("{templateId}")]
        [ProducesResponseType(200, Type = typeof(TourTemplateDto))]
        [AllowAnonymous]
        public async Task<IActionResult> GetTemplate(string templateId, CancellationToken cancellationToken)
        {
            var result = await _tourTemplateService.GetTemplateAsync(templateId, cancellationToken);
            return result.IsError
                ? BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors))
                : (IActionResult)Ok(result.Value);
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(PagedList<TourTemplateListItem>))]
        [AllowAnonymous]
        public async Task<IActionResult> GetGuideTemplates([FromQuery] string guideId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string category = null, CancellationToken cancellationToken = default)
        {
            var effectiveGuideId = guideId;
            if (string.IsNullOrEmpty(effectiveGuideId))
            {
                effectiveGuideId = _userContext.UserId;
                if (string.IsNullOrEmpty(effectiveGuideId))
                    return BadRequest(ErrorEnvelop.Create(new[] { "guideId query parameter is required for unauthenticated requests" }));
            }
            var result = await _tourTemplateService.GetGuideTemplatesAsync(effectiveGuideId, page, pageSize, category, cancellationToken);
            return result.IsError
                ? BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors))
                : (IActionResult)Ok(result.Value);
        }

        [HttpPost("{templateId}/use-template")]
        [ProducesResponseType(200, Type = typeof(TourTemplateDto))]
        public async Task<IActionResult> GetTemplateDataForTourCreation(string templateId, CancellationToken cancellationToken)
        {
            var result = await _tourTemplateService.GetTemplateDataForTourCreationAsync(_userContext.UserId, templateId, cancellationToken);
            return result.IsError
                ? BadRequest(ErrorEnvelop.CreateFromOutcome(result.Errors))
                : (IActionResult)Ok(result.Value);
        }
    }
}
