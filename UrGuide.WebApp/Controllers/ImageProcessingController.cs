using System;
using System.Security.Claims;
using System.Threading.Tasks;
using BbQ.Outcome;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UrGuide.Model.Media;
using UrGuide.Services.Media;
using UrGuide.WebApp.Models;

namespace UrGuide.WebApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/images/processing")]
    [ProducesResponseType(400, Type = typeof(ErrorEnvelop<string>))]
    [ProducesResponseType(500, Type = typeof(ErrorEnvelop<string>))]
    public class ImageProcessingController : ControllerBase
    {
        private readonly IImageProcessingService _imageProcessingService;
        private readonly ILogger<ImageProcessingController> _logger;

        public ImageProcessingController(
            IImageProcessingService imageProcessingService,
            ILogger<ImageProcessingController> logger)
        {
            _imageProcessingService = imageProcessingService;
            _logger = logger;
        }

        /// <summary>
        /// Submit an image for processing
        /// </summary>
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(ImageProcessingResult))]
        public async Task<IActionResult> ProcessImage([FromBody] ImageProcessingRequest request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var result = await _imageProcessingService.ProcessImageAsync(request);
                return result.Match(
                    onSuccess: value => (IActionResult)Ok(value),
                    onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing image");
                return StatusCode(500, ErrorEnvelop.Create(new[] { "Internal server error" }));
            }
        }

        /// <summary>
        /// Get image variants/sizes
        /// </summary>
        [HttpGet("{imageId}/variants")]
        [ProducesResponseType(200, Type = typeof(ImageVariantsDto))]
        public async Task<IActionResult> GetImageVariants(string imageId)
        {
            try
            {
                var result = await _imageProcessingService.GetImageVariantsAsync(imageId);
                return result.Match(
                    onSuccess: value => (IActionResult)Ok(value),
                    onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting image variants for {ImageId}", imageId);
                return StatusCode(500, ErrorEnvelop.Create(new[] { "Internal server error" }));
            }
        }

        /// <summary>
        /// Get processing status
        /// </summary>
        [HttpGet("{imageId}/status")]
        [ProducesResponseType(200, Type = typeof(string))]
        public async Task<IActionResult> GetProcessingStatus(string imageId)
        {
            try
            {
                var result = await _imageProcessingService.GetProcessingStatusAsync(imageId);
                return result.Match(
                    onSuccess: value => (IActionResult)Ok(new { Status = value.ToString() }),
                    onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting processing status for {ImageId}", imageId);
                return StatusCode(500, ErrorEnvelop.Create(new[] { "Internal server error" }));
            }
        }

        /// <summary>
        /// Apply watermark to an image
        /// </summary>
        [HttpPost("{imageId}/watermark")]
        [ProducesResponseType(200, Type = typeof(ImageProcessingResult))]
        public async Task<IActionResult> ApplyWatermark(string imageId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var result = await _imageProcessingService.ApplyWatermarkAsync(imageId);
                return result.Match(
                    onSuccess: value => (IActionResult)Ok(value),
                    onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying watermark to image {ImageId}", imageId);
                return StatusCode(500, ErrorEnvelop.Create(new[] { "Internal server error" }));
            }
        }

        /// <summary>
        /// Get EXIF data for an image
        /// </summary>
        [HttpGet("{imageId}/exif")]
        [ProducesResponseType(200, Type = typeof(object))]
        public async Task<IActionResult> GetExifData(string imageId)
        {
            try
            {
                var result = await _imageProcessingService.ExtractExifDataAsync(imageId);
                return result.Match(
                    onSuccess: value => (IActionResult)Ok(value),
                    onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting EXIF data for {ImageId}", imageId);
                return StatusCode(500, ErrorEnvelop.Create(new[] { "Internal server error" }));
            }
        }

        /// <summary>
        /// Get CDN URL for an image
        /// </summary>
        [HttpGet("{imageId}/cdn-url")]
        [ProducesResponseType(200, Type = typeof(object))]
        public async Task<IActionResult> GetCdnUrl(string imageId, [FromQuery] string cdnProvider)
        {
            try
            {
                var variantsResult = await _imageProcessingService.GetImageVariantsAsync(imageId);
                if (variantsResult.IsError)
                    return BadRequest(ErrorEnvelop.CreateFromOutcome(variantsResult.Errors));

                var originalUrl = variantsResult.Value?.OriginalUrl;
                if (string.IsNullOrEmpty(originalUrl))
                    return BadRequest(new ErrorEnvelop<string>(new[] { "Image not found or has no original URL" }));

                var result = await _imageProcessingService.GetCdnUrlAsync(originalUrl, cdnProvider);
                return result.Match(
                    onSuccess: value => (IActionResult)Ok(new { CdnUrl = value }),
                    onError: errors => (IActionResult)BadRequest(ErrorEnvelop.CreateFromOutcome(errors)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting CDN URL for {ImageId}", imageId);
                return StatusCode(500, ErrorEnvelop.Create(new[] { "Internal server error" }));
            }
        }
    }
}
