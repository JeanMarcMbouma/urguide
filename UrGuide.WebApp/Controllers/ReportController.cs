using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using UrGuide.Model.Reports;
using UrGuide.Services.Reports;

namespace UrGuide.WebApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly IReportingService _reportingService;
        private readonly ILogger<ReportController> _logger;

        public ReportController(IReportingService reportingService, ILogger<ReportController> logger)
        {
            _reportingService = reportingService;
            _logger = logger;
        }

        /// <summary>
        /// Generate a new report
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> GenerateReport([FromBody] GenerateReportRequest request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var report = await _reportingService.GenerateReportAsync(userId, request);
                return Ok(report);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid report request");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating report");
                return StatusCode(500, new { error = "An error occurred while generating the report" });
            }
        }

        /// <summary>
        /// Get report details
        /// </summary>
        [HttpGet("{reportId}")]
        public async Task<IActionResult> GetReport(string reportId)
        {
            try
            {
                var report = await _reportingService.GetReportAsync(reportId);
                if (report == null)
                {
                    return NotFound(new { error = "Report not found" });
                }

                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving report");
                return StatusCode(500, new { error = "An error occurred while retrieving the report" });
            }
        }

        /// <summary>
        /// Get user's reports with pagination
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetUserReports([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var (items, totalCount) = await _reportingService.GetUserReportsAsync(userId, page, pageSize);
                return Ok(new { items, totalCount, page, pageSize });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user reports");
                return StatusCode(500, new { error = "An error occurred while retrieving reports" });
            }
        }

        /// <summary>
        /// Download report data as CSV
        /// </summary>
        [HttpGet("{reportId}/download")]
        public async Task<IActionResult> DownloadReport(string reportId)
        {
            try
            {
                var data = await _reportingService.GetReportDataAsync(reportId);
                if (data == null)
                {
                    return NotFound(new { error = "Report not found" });
                }

                var csvBytes = await _reportingService.ExportToCsvAsync(data);
                return File(csvBytes, "text/csv", $"{data.ReportName}.csv");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading report");
                return StatusCode(500, new { error = "An error occurred while downloading the report" });
            }
        }

        /// <summary>
        /// Create a scheduled report
        /// </summary>
        [HttpPost("schedules")]
        public async Task<IActionResult> CreateSchedule([FromBody] CreateScheduledReportRequest request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var schedule = await _reportingService.CreateScheduleAsync(userId, request);
                return Ok(schedule);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid schedule request");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating scheduled report");
                return StatusCode(500, new { error = "An error occurred while creating the schedule" });
            }
        }

        /// <summary>
        /// Get user's scheduled reports
        /// </summary>
        [HttpGet("schedules")]
        public async Task<IActionResult> GetSchedules()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var schedules = await _reportingService.GetSchedulesAsync(userId);
                return Ok(schedules);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving scheduled reports");
                return StatusCode(500, new { error = "An error occurred while retrieving schedules" });
            }
        }

        /// <summary>
        /// Update a scheduled report
        /// </summary>
        [HttpPut("schedules/{scheduleId}")]
        public async Task<IActionResult> UpdateSchedule(string scheduleId, [FromBody] CreateScheduledReportRequest request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var schedule = await _reportingService.UpdateScheduleAsync(userId, scheduleId, request);
                if (schedule == null)
                {
                    return NotFound(new { error = "Schedule not found" });
                }

                return Ok(schedule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating scheduled report");
                return StatusCode(500, new { error = "An error occurred while updating the schedule" });
            }
        }

        /// <summary>
        /// Delete a scheduled report
        /// </summary>
        [HttpDelete("schedules/{scheduleId}")]
        public async Task<IActionResult> DeleteSchedule(string scheduleId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var success = await _reportingService.DeleteScheduleAsync(userId, scheduleId);
                if (!success)
                {
                    return NotFound(new { error = "Schedule not found" });
                }

                return Ok(new { message = "Schedule deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting scheduled report");
                return StatusCode(500, new { error = "An error occurred while deleting the schedule" });
            }
        }

        /// <summary>
        /// Generate guide earnings report data
        /// </summary>
        [HttpGet("guide-earnings")]
        public async Task<IActionResult> GetGuideEarnings([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var data = await _reportingService.GenerateGuideEarningsDataAsync(userId, startDate, endDate);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating guide earnings report");
                return StatusCode(500, new { error = "An error occurred while generating the earnings report" });
            }
        }

        /// <summary>
        /// Generate booking summary report data (Admin only)
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("booking-summary")]
        public async Task<IActionResult> GetBookingSummary([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var data = await _reportingService.GenerateBookingSummaryDataAsync(startDate, endDate);
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating booking summary report");
                return StatusCode(500, new { error = "An error occurred while generating the booking summary" });
            }
        }
    }
}
