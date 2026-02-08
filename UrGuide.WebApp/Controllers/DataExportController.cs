using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UrGuide.Model.Users;
using UrGuide.Services.Contracts;
using UrGuide.WebApp.Models;

namespace UrGuide.WebApp.Controllers
{
    /// <summary>
    /// Controller for managing GDPR-compliant user data exports
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    [ProducesResponseType(400, Type = typeof(ErrorEnvelop<string>))]
    [ProducesResponseType(500, Type = typeof(ErrorEnvelop<string>))]
    public class DataExportController : ControllerBase
    {
        public DataExportController(IDataExportService dataExportService)
        {
            DataExportService = dataExportService ?? throw new ArgumentNullException(nameof(dataExportService));
        }

        public IDataExportService DataExportService { get; }

        /// <summary>
        /// Request a new data export for the authenticated user
        /// </summary>
        /// <param name="request">Export request details (format: JSON or CSV)</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Export request details including request ID</returns>
        /// <response code="200">Export request created successfully</response>
        /// <response code="400">Invalid request or pending export exists</response>
        /// <response code="401">User not authenticated</response>
        [HttpPost("request")]
        [ProducesResponseType(200, Type = typeof(DataExportResponse))]
        public async Task<IActionResult> RequestExport([FromBody] DataExportRequestModel request, CancellationToken cancellationToken)
        {
            var result = await DataExportService.RequestExportAsync(request, cancellationToken);
            
            if (result.HasError)
                return BadRequest(ErrorEnvelop.Create(result.Errors));

            return Ok(result.Data);
        }

        /// <summary>
        /// Get the status of an export request
        /// </summary>
        /// <param name="requestId">Export request ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Export request status and details</returns>
        /// <response code="200">Export status retrieved successfully</response>
        /// <response code="404">Export request not found</response>
        /// <response code="401">User not authenticated</response>
        [HttpGet("status/{requestId}")]
        [ProducesResponseType(200, Type = typeof(DataExportResponse))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetExportStatus(string requestId, CancellationToken cancellationToken)
        {
            var result = await DataExportService.GetExportStatusAsync(requestId, cancellationToken);
            
            if (result.HasError)
                return NotFound(ErrorEnvelop.Create(result.Errors));

            return Ok(result.Data);
        }

        /// <summary>
        /// Download an export file using a secure token
        /// </summary>
        /// <param name="token">Secure download token</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Export file for download</returns>
        /// <response code="200">Export file downloaded successfully</response>
        /// <response code="404">Export not found or expired</response>
        /// <response code="400">Invalid token or export not ready</response>
        [HttpGet("download/{token}")]
        [AllowAnonymous] // Allow anonymous access with secure token
        [ProducesResponseType(200)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> DownloadExport(string token, CancellationToken cancellationToken)
        {
            var result = await DataExportService.DownloadExportAsync(token, cancellationToken);
            
            if (result.HasError)
            {
                // Check if error indicates invalid token/not ready (BadRequest) vs not found
                var errorMessage = result.Errors.FirstOrDefault() ?? string.Empty;
                if (errorMessage.Contains("not ready") || errorMessage.Contains("invalid"))
                    return BadRequest(ErrorEnvelop.Create(result.Errors));
                return NotFound(ErrorEnvelop.Create(result.Errors));
            }

            var (filePath, fileName, _) = result.Data;

            // Determine content type
            var contentType = fileName.EndsWith(".json") ? "application/json" : "application/zip";

            // Return file for download - FileStreamResult will dispose the stream
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
            return File(fileStream, contentType, fileName);
        }

        /// <summary>
        /// Cancel a pending export request
        /// </summary>
        /// <param name="requestId">Export request ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Success result</returns>
        /// <response code="200">Export cancelled successfully</response>
        /// <response code="400">Cannot cancel completed or failed export</response>
        /// <response code="404">Export request not found</response>
        /// <response code="401">User not authenticated</response>
        [HttpDelete("{requestId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> CancelExport(string requestId, CancellationToken cancellationToken)
        {
            var result = await DataExportService.CancelExportAsync(requestId, cancellationToken);
            
            if (result.HasError)
                return BadRequest(ErrorEnvelop.Create(result.Errors));

            return Ok(new { message = "Export request cancelled successfully" });
        }
    }
}
